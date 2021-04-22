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
            RetentionTol.Text = SearchingParameters.Access.retentionRange.ToString();
            if (SearchingParameters.Access.ToleranceBy
                == ToleranceBy.Dalton)
            {
                MS1TolByDalton.IsChecked = true;
            }
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

            if (double.TryParse(RetentionTol.Text, out double reTol))
            {
                ConfigureParameters.Access.retentionRange = reTol;
            }
            else
            {
                MessageBox.Show("Retention value is invalid!");
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
