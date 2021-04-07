using GlycanQuant.Engine.Algorithm;
using GlycanQuant.Engine.Builder;
using GlycanQuant.Engine.Builder.NGlycans;
using GlycanQuant.Engine.Quant;
using GlycanQuant.Engine.Quant.XIC;
using GlycanQuant.Engine.Search;
using GlycanQuant.Engine.Search.Envelope;
using GlycanQuant.Engine.Search.NGlycans;
using GlycanQuant.Spectrum.Process;
using GlycanQuant.Spectrum.Process.PeakPicking;
using Microsoft.Win32;
using SpectrumData;
using SpectrumData.Reader;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GlycanQuantApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path = "";
        private ISpectrumReader spectrumReader 
            = new ThermoRawSpectrumReader();
        IProcess spectrumProcessor = new LocalNeighborPicking();
        ISpectrumSearch spectrumSearch { get; set; }
        List<IGlycanPeak> glycans { get; set; }

        List<int> validScan = new List<int>();

        private DrawingVisualizer visualizer = new DrawingVisualizer();
        private NormalizerEngine engine = new NormalizerEngine();
        private int readingCounter;

        public MainWindow()
        {
            InitializeComponent();

            // build up searcher
            NGlycanTheoryPeaksBuilder builder = new NGlycanTheoryPeaksBuilder();
            builder.SetBuildType(true, false, false);
            glycans = builder.Build();

            IResultFactory factory = new NGlycanResultFactory();
            EnvelopeProcess envelopeProcess = new EnvelopeProcess(10, ToleranceBy.PPM);
            MonoisotopicSearcher monoisotopicSearcher = new MonoisotopicSearcher(factory);
            IProcess spectrumProcess = new LocalNeighborPicking();
            spectrumSearch = new NGlycanSpectrumSearch(glycans,
                spectrumProcess, envelopeProcess, monoisotopicSearcher);
        }

        private void MSMSFileName_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileNameDialog = new OpenFileDialog();
            fileNameDialog.Filter = "Raw File|*.raw";
            fileNameDialog.Title = "Open a MS2 File";

            if (fileNameDialog.ShowDialog() == true)
            {
                Binding fileNameBinding = new Binding();
                fileNameBinding.Path = new PropertyPath("FileName");
                fileNameBinding.Source = fileNameDialog;
                fileNameBinding.Mode = BindingMode.OneWay;
                displayFileName.SetBinding(TextBox.TextProperty, fileNameBinding);
                path = displayFileName.Text;
                spectrumReader.Init(path);

                int start = spectrumReader.GetFirstScan();
                int end = spectrumReader.GetLastScan();

                for(int i = start; i <= end; i++)
                {
                    if (spectrumReader.GetMSnOrder(i) == 1)
                        validScan.Add(i);
                }
                ScanScroll.Minimum = 0;
                ScanScroll.Maximum = validScan.Count-1;
            }
        }
        private void Visualize_Click(object sender, RoutedEventArgs e)
        {
            if (path.Length == 0)
            {
                MessageBox.Show("Please input the spectrum file (*.Raw) path!");
                return;
            }

            int scan;
            int.TryParse(ScanNumber.Text, out scan);
            if (scan > 0 && scan < spectrumReader.GetLastScan() &&
                spectrumReader.GetMSnOrder(scan) == 1)
            {
                ISpectrum spectrum = spectrumProcessor.Process(spectrumReader.GetSpectrum(scan));
                double rt = spectrumReader.GetRetentionTime(scan);
                double index = Math.Round(engine.Normalize(rt), 2);
                string output = "Retention time: " + Math.Round(rt, 2).ToString() + " (min) ";
                if (index > 0)
                    output += "GUI index " + index.ToString();
                RTtime.Text = output;
                // search
                List<IResult> results = spectrumSearch.Search(spectrum);

                //// draw curve
                canvas.Children.Clear();
                if (spectrum.GetPeaks().Count > 0)
                    canvas.Children.Add(new VisualHost
                    { Visual = visualizer.CreateDrawingVisual(spectrum, results) });
                PeakArea.Visibility = Visibility.Visible;

            }
            else
            {
                MessageBox.Show("Please input a valid scan number!");
                canvas.Children.Clear();
                PeakArea.Visibility = Visibility.Collapsed;
                return;
            }
        }
        private void Area_Click(object sender, RoutedEventArgs e)
        {
            if (path.Length == 0)
            {
                MessageBox.Show("Please input the spectrum file (*.Raw) path!");
                return;
            }

            int scan;
            int.TryParse(ScanNumber.Text, out scan);
            if (scan > 0 && scan < spectrumReader.GetLastScan() &&
                spectrumReader.GetMSnOrder(scan) == 1)
            {
                ISpectrum spectrum = spectrumReader.GetSpectrum(scan);
                List<IResult> results = spectrumSearch.Search(spectrum);
                double rt = spectrumReader.GetRetentionTime(scan);

                ConcurrentDictionary<IResult, double> quant = new ConcurrentDictionary<IResult, double>();
                Parallel.ForEach(results, (r) =>
                {
                    IXIC xicer = new TIQ3XIC(spectrumReader, 0.01, ToleranceBy.Dalton);
                    double area = xicer.Area(r);
                    quant[r] = area;
                });

                string outputPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path),  
                    System.IO.Path.GetFileNameWithoutExtension(path) + ".csv");
                using (FileStream ostrm = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(ostrm))
                    {
                        writer.WriteLine("scan,time,glycan,area");
                        foreach(IResult r in quant.Keys)
                        {
                            List<string> output = new List<string>()
                            {
                                scan.ToString(), rt.ToString(), 
                                r.Glycan().GetGlycan().Name(), quant[r].ToString()
                            };

                            writer.WriteLine(string.Join(",", output));
                        }
                        writer.Flush();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please input a valid scan number!");
                return;
            }
        }
        private async void Normalize_Click(object sender, RoutedEventArgs e)
        {
            if (path.Length == 0)
            {
                MessageBox.Show("Please input the spectrum file (*.Raw) path!");
                return;
            }
            normalize.IsEnabled = false;
            readingCounter = 0;
            Counter counter = new Counter();
            counter.progressChange += ReadProgressChanged;

            spectrumReader.Init(path);

            await Task.Run(() =>
            {
                engine.Run(spectrumReader, counter);

            });
        }

        private void Readingprogress(int total)
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new ThreadStart(() =>
                {
                    ReadingStatus.Value = readingCounter * 1.0 / total * 1000.0;
                }));
        }
        private void ReadProgressChanged(object sender, ProgressingEventArgs e)
        {
            Interlocked.Increment(ref readingCounter);
            Readingprogress(e.Total);
        }
        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (path.Length != 0)
            {
                int scan = validScan[(int) e.NewValue];

                ScanNumber.Text = scan.ToString();
                double rt = spectrumReader.GetRetentionTime(scan);
                double index = Math.Round(engine.Normalize(rt), 2);
                string output = "Retention time: " + Math.Round(rt, 2).ToString() + " (min) ";
                if (index > 0)
                   output += "GUI index " + index.ToString();
                RTtime.Text = output;
            }
        }
    }

    public class VisualHost : UIElement
    {
        public Visual Visual { get; set; }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException("index");
            return Visual;
        }
    }
}
