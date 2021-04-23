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
using GlycanQuant.Engine.Algorithm;
using SpectrumData;
using SpectrumData.Reader;
using GlycanGUI.Algorithm.CurveFitting;
using GlycanQuant.Model.Util;

namespace GlycanQuantApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path = "";
        private NormalizerEngine engine = new NormalizerEngine();
        private QuantEngine quanter = new QuantEngine();
        private int readingCounter;
        Counter counter = new Counter();
        ISpectrumReader spectrumReader = new ThermoRawSpectrumReader();

        public MainWindow()
        {
            InitializeComponent();
            InitCounter();
        }

        public void InitCounter()
        {
            counter.progressChange += ReadProgressChanged;
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            Window subWindow = new ConfigureWindow();
            subWindow.Show();
        }

        private void MSMSFileNames_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileNamesDialog = new OpenFileDialog();
            fileNamesDialog.Filter = "Raw File|*.raw";
            fileNamesDialog.Title = "Open a MS File";
            fileNamesDialog.Multiselect = true;
            fileNamesDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (fileNamesDialog.ShowDialog() == true)
            {
                foreach (string filename in fileNamesDialog.FileNames)
                {
                    if (!SearchingParameters.Access.MSMSFiles.Contains(filename))
                    {
                        lbFiles.Items.Add(filename);
                        SearchingParameters.Access.MSMSFiles.Add(filename);
                    }
                }

            }
        }

        private void DeselectFiles_Click(object sender, RoutedEventArgs e)
        {
            if (lbFiles.SelectedItem != null)
            {
                string filename = lbFiles.SelectedItem.ToString();
                lbFiles.Items.Remove(lbFiles.SelectedItem);
                if (SearchingParameters.Access.MSMSFiles.Contains(filename))
                    SearchingParameters.Access.MSMSFiles.Remove(filename);
            }
        }

        private async void PeakArea_Click(object sender, RoutedEventArgs e)
        {
            if (SearchingParameters.Access.MSMSFiles.Count == 0)
            {
                MessageBox.Show("Please choose MS/MS files");
                return;
            }

            // init ions 
            List<double> ions = new List<double>();
            if (SearchingParameters.Access.Hydrogen)
                ions.Add(Calculator.proton);
            if (SearchingParameters.Access.Potassium)
                ions.Add(Calculator.potassium);
            if (SearchingParameters.Access.Ammonium)
                ions.Add(Calculator.ammonium);
            Calculator.To.SetChargeIons(ions);

            await Task.Run(() =>
            {
                foreach(string path in SearchingParameters.Access.MSMSFiles)
                {
                    readingCounter = 0;
                    quanter.Run(path, counter, spectrumReader, engine);
                }
            });
        }

        private void GUI_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileNameDialog = new OpenFileDialog();
            fileNameDialog.Filter = "Raw File|*.raw";
            fileNameDialog.Title = "Open a MS File";

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
            await Task.Run(() =>
            {
                engine.Run(path, counter, spectrumReader);
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
            if (text == "Poly")
            {
                engine.Fitter = new PolynomialFitting();
            }
            else if (text == "Log")
            {
                engine.Fitter = new LogarithmicFitting();
            }
        }
    }

    public class ProgressingEventArgs : EventArgs
    {
        public int Total { get; set; }
    }

    public class Counter
    {
        public event EventHandler<ProgressingEventArgs> progressChange;

        protected virtual void OnProgressChanged(ProgressingEventArgs e)
        {
            EventHandler<ProgressingEventArgs> handler = progressChange;
            handler?.Invoke(this, e);
        }

        public void Add(int total)
        {
            ProgressingEventArgs e = new ProgressingEventArgs
            {
                Total = total
            };
            OnProgressChanged(e);
        }
    }
}
