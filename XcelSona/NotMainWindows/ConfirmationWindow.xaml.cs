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

namespace XcelSona
{
    /// <summary>
    /// Lógica de interacción para ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow : Window
    {
        public ConfirmationWindow()
        {
            InitializeComponent();
        }

        private void aceptarBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            cancelarBtn.Source = cancelarBtnNo.Source;
            aceptarBtn.Source = aceptarBtnSi.Source;
        }

        private void aceptarBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            aceptarBtn.Source = aceptarBtnNo.Source;
        }

        private void aceptarBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void cancelarBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            aceptarBtn.Source = aceptarBtnNo.Source;
            cancelarBtn.Source = cancelarBtnSi.Source;
        }

        private void cancelarBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            cancelarBtn.Source = cancelarBtnNo.Source;
        }

        private void cancelarBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch (Exception)
            {
                return;
            }
        }
        
    }
}
