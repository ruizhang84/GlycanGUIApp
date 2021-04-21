using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace GlycoGUIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path = "";
        private NormalizerEngine engine = new NormalizerEngine();
        private int readingCounter;
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

            await Task.Run(() =>
            {
                engine.Run(path, counter);

            });

            Window subWindow = new CurveWindow(engine);
            subWindow.Show();

            RTtime.Visibility = Visibility.Visible;
            NormalizedTime.Visibility = Visibility.Visible;
            convert.Visibility = Visibility.Visible;
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

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double rt = 0.0;
            if (double.TryParse(RTtime.Text, out rt))
            {
                rt = Math.Round(Math.Max(0, engine.Normalize(rt)), 4);
                NormalizedTime.Text = rt.ToString();
            }
        }
    }
}
