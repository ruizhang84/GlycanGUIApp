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

namespace GlycanQuantApp
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

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            Window subWindow = new ConfigureWindow();
            subWindow.Show();
        }

        private void MSMSFileNames_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileNamesDialog = new OpenFileDialog();
            fileNamesDialog.Filter = "Raw File|*.raw|MGF File|*.mgf";
            fileNamesDialog.Title = "Open a MS2 File";
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

        private void PeakArea_Click(object sender, RoutedEventArgs e)
        {
            if (SearchingParameters.Access.MSMSFiles.Count == 0)
            {
                MessageBox.Show("Please choose MS/MS files");
            }
            else
            {
            }
        }

        private void GUI_Click(object sender, RoutedEventArgs e)
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
