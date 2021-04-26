using GlycanQuant.Engine.Algorithm;
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
using System.Windows.Shapes;

namespace GlycanQuantApp
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    public partial class ConfigureWindow : Window
    {
        public ConfigureWindow()
        {
            InitializeComponent();
            MS1Tol.Text = SearchingParameters.Access.Tolerance.ToString();
            PeakThreshold.Text = SearchingParameters.Access.Threshold.ToString();
            PeakCutoff.Text = SearchingParameters.Access.PeakCutoff.ToString();
            if (SearchingParameters.Access.ToleranceBy
                == ToleranceBy.Dalton)
            {
                MS1TolByDalton.IsChecked = true;
            }

            if (SearchingParameters.Access.Ammonium)
                Ammonium.IsChecked = true;
            if (SearchingParameters.Access.Hydrogen)
                Hydrogen.IsChecked = true;
            if (SearchingParameters.Access.Sodium)
                Sodium.IsChecked = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (SaveChange())
            {
                SearchingParameters.Access.Update();
                Close();
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private bool SaveChange()
        {
            if (double.TryParse(MS1Tol.Text, out double tol))
            {
                ConfigureParameters.Access.Tolerance = tol;
            }
            else
            {
                MessageBox.Show("MS tolerance value is invalid!");
                return false;
            }

            if (double.TryParse(PeakThreshold.Text, out double thresh))
            {
                ConfigureParameters.Access.Threshold = thresh;
            }
            else
            {
                MessageBox.Show("Peak threshold is invalid!");
                return false;
            }

            if (double.TryParse(PeakCutoff.Text, out double pkCut))
            {
                ConfigureParameters.Access.PeakCutoff = pkCut;
            }
            else
            {
                MessageBox.Show("Peak cutoff is invalid!");
                return false;
            }


            if (int.TryParse(MaxCharge.Text, out int charge))
            {
                ConfigureParameters.Access.MaxCharage = charge;
            }
            else
            {
                MessageBox.Show("Retention value is invalid!");
                return false;
            }


            if (double.TryParse(Cutoff.Text, out double cutoff))
            {
                ConfigureParameters.Access.Cutoff = cutoff;
            }
            else
            {
                MessageBox.Show("Retention value is invalid!");
                return false;
            }

            if (Hydrogen.IsChecked == false &&
                            Sodium.IsChecked == false && Ammonium.IsChecked == false)
            {
                MessageBox.Show("Choose at least one Ion type!");
                return false;
            }
            else
            {
                ConfigureParameters.Access.Hydrogen = Hydrogen.IsChecked == true;
                ConfigureParameters.Access.Sodium = Sodium.IsChecked == true;
                ConfigureParameters.Access.Ammonium = Ammonium.IsChecked == true;
            }

            return true;
        }

        private void MS1TolByPPM_Checked(object sender, RoutedEventArgs e)
        {
            ConfigureParameters.Access.ToleranceBy = ToleranceBy.PPM;
        }

        private void MS1TolByDalton_Checked(object sender, RoutedEventArgs e)
        {
            ConfigureParameters.Access.ToleranceBy = ToleranceBy.Dalton;
        }
    }
}
