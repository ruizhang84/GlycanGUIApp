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
        public MainWindow()
        {
            InitializeComponent();
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

        private void Visualize_Click(object sender, RoutedEventArgs e)
        {
            if (path.Length == 0)
            {
                MessageBox.Show("Please input the spectrum file (*.Raw) path!");
                return;
            }

            int scan;
            int.TryParse(ScanNumber.Text, out scan);
            if (scan > 0)
            {
                ISpectrum spectrum = spectrumReader.GetSpectrum(scan);
                //// draw curve
                canvas.Children.Add(new VisualHost
                { Visual = CreateDrawingVisual(spectrum) });

            }
            else
            {
                MessageBox.Show("Please input a valid scan number!");
                return;
            }
        }

        private DrawingVisual CreateDrawingVisual(ISpectrum spectrum)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            // Create a drawing and draw it in the DrawingContext.
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                Pen pen = new Pen(Brushes.Black, 1);
                Rect rect =
                    new Rect(new Point(40, 40), new Point(660, 300)); // range of (600, 260)
                drawingContext.DrawRectangle(Brushes.Transparent, pen, rect);
                List<IPeak> peaks = spectrum.GetPeaks();

                List<double> x = peaks.Select(p => p.GetMZ()).ToList();
                List<double> y = peaks.Select(p => p.GetIntensity()).ToList();

                // draw curve
                List<Point> points = new List<Point>();
                for (int i = 0; i < x.Count; i++)
                {
                    points.Add(new Point(x[i], y[i]));
                }
                Pen curve = new Pen(Brushes.Red, 1);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    drawingContext.DrawLine(curve, points[i], points[i + 1]);
                }
            }
            return drawingVisual;
        }

        private void Area_Click(object sender, RoutedEventArgs e)
        {
            if (path.Length == 0)
            {
                MessageBox.Show("Please input the spectrum file (*.Raw) path!");
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
