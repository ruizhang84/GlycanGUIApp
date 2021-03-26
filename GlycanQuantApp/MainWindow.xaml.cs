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
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        double tolerance = 10;
        ToleranceBy by = ToleranceBy.PPM;

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
            }
        }

        private int BinarySearchPoints(List<double> peaks, double mz)
        {
            int start = 0;
            int end = peaks.Count - 1;

            while (start + 1 < end)
            {
                int mid = end + (start - end) / 2;
                if (Math.Abs(peaks[mid] - mz) < 0.001)
                {
                    return mid;
                }
                else if (peaks[mid] > mz)
                {
                    end = mid - 1;
                }
                else
                {
                    start = mid;
                }
            }

            return start;
        }

        private DrawingVisual CreateDrawingVisual(ISpectrum spectrum, List<IResult> results,
            double x1 = 40, double y1 = 40, double x2 = 660, int y2 = 300)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            // Create a drawing and draw it in the DrawingContext.
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                double delta = 10;
                Pen pen = new Pen(Brushes.Black, 1);
                Rect rect =
                    new Rect(new Point(x1 - delta, y1 - delta), 
                    new Point(x2 + delta, y2 + delta)); // range of (600, 260)
                drawingContext.DrawRectangle(Brushes.Transparent, pen, rect);

                // process peaks
                List<IPeak> peaks = spectrum.GetPeaks();
                double deltaX = x2 - x1;
                double deltaY = y2 - y1;
                double minX = peaks.Min(p => p.GetMZ());
                double maxX = peaks.Max(p => p.GetMZ());
                List<double> x = peaks.Select(p => (p.GetMZ() - minX) / (maxX - minX) * deltaX + x1).ToList();
                double maxY = peaks.Max(p => p.GetIntensity());
                List<double> y = peaks.Select(p => y2 - p.GetIntensity() / maxY * deltaY).ToList();

                // draw points
                Pen p = new Pen(Brushes.Red, 3);
                List<double> mzList = results.Select(r => r.GetMZ()).ToList();

                for (int i = 0; i < mzList.Count; i++)
                {
                    double xi = (mzList[i] - minX) / (maxX - minX) * deltaX + x1;
                    double yi = y[BinarySearchPoints(mzList, xi)];
                    Point point = new Point(xi, yi);
                    drawingContext.DrawEllipse(Brushes.Red, p, point, 1, 1);
                }

                // draw curve
                List<Point> points = new List<Point>();
                for (int i = 0; i < x.Count; i++)
                {
                    points.Add(new Point(x[i], y[i]));
                }
                Pen curve = new Pen(Brushes.Blue, 1);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    drawingContext.DrawLine(curve, points[i], points[i + 1]);
                }
            }
            return drawingVisual;
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
                RTtime.Text = "Retention time: " + Math.Round(rt, 2).ToString() + " (min)";
                // search
                List<IResult> results = spectrumSearch.Search(spectrum);

                //// draw curve
                canvas.Children.Clear();
                if (spectrum.GetPeaks().Count > 0)
                    canvas.Children.Add(new VisualHost
                    { Visual = CreateDrawingVisual(spectrum, results) });
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

                Parallel.ForEach(results, (r) =>
                {
                    IResultFactory factory = new NGlycanResultFactory();
                    EnvelopeProcess envelopeProcess = new EnvelopeProcess(tolerance, by);
                    MonoisotopicSearcher monoisotopicSearcher = new MonoisotopicSearcher(factory);
                    IProcess spectrumProcess = new LocalNeighborPicking();
                    ISpectrumSearch search = new NGlycanSpectrumSearch(glycans,
                        spectrumProcess, envelopeProcess, monoisotopicSearcher);

                    IAreaCalculator areaCalculator = new TrapezoidalRule();
                    IXIC xicer = new NGlycanXIC(areaCalculator, spectrumReader, search);

                    Console.WriteLine(r.Glycan().GetGlycan().Name());

                    double area = xicer.Area(r);
                    Console.WriteLine(area);
                });

            }
            else
            {
                MessageBox.Show("Please input a valid scan number!");
                return;
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
