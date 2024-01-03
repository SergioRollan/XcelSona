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
using static XcelSona.MainWindow;

namespace XcelSona.NotMainWindows
{
    /// <summary>
    /// Lógica de interacción para ChooseColorWindow.xaml
    /// </summary>
    public partial class ChooseColorWindow : Window
    {
        private SolidColorBrush colorVolver;
        public SolidColorBrush color;
        double red = 255;
        double green = 255;
        double blue = 255;
        public event CambioColorEventHandler CambioColor;


        public ChooseColorWindow(SolidColorBrush c)
        {
            if(c!=null)
            {
                colorVolver = c;
                color = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                InitializeComponent();
                sliderB.Value = blue = c.Color.B;
                sliderG.Value = green = c.Color.G;
                sliderR.Value = red = c.Color.R;
            }
        }
        protected virtual void OnCambioColor(CambioColorEventArgs e)
        {
            if (this.CambioColor != null)
            {
                this.CambioColor(this, e);
            }
        }

        private void aceptarBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            aceptarBtn.Source = aceptarBtnSi.Source;
        }

        private void aceptarBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            aceptarBtn.Source = aceptarBtnNo.Source;
        }



        private void sliderR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            red = sliderR.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void sliderG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            green = sliderG.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void sliderB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blue = sliderB.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void backRedBtn_Click(object sender, RoutedEventArgs e)
        {
            sliderR.Value = sliderR.Value-1;
            red = sliderB.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void forwRedBtn_Click(object sender, RoutedEventArgs e)
        {
            sliderR.Value = sliderR.Value + 1;
            red = sliderB.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void backGreenBtn_Click(object sender, RoutedEventArgs e)
        {
            sliderG.Value = sliderG.Value - 1;
            green = sliderG.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void forwGreenBtn_Click(object sender, RoutedEventArgs e)
        {
            sliderG.Value = sliderG.Value + 1;
            green = sliderG.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void backBlueBtn_Click(object sender, RoutedEventArgs e)
        {
            sliderB.Value = sliderB.Value - 1;
            blue = sliderB.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void forwBlueBtn_Click(object sender, RoutedEventArgs e)
        {
            sliderB.Value = sliderB.Value + 1;
            blue = sliderB.Value;
            byte b1 = (byte)red;
            byte b2 = (byte)green;
            byte b3 = (byte)blue;
            color = new SolidColorBrush(Color.FromRgb(b1, b2, b3));
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
        }

        private void aceptarBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void cancelarBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            cancelarBtn.Source = cancelarBtnSi.Source;
        }

        private void cancelarBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            color = colorVolver;
            CambioColorEventArgs argumentos = new CambioColorEventArgs();
            argumentos.ColorCambiado = color.Color;
            OnCambioColor(argumentos);
            Close();
        }

        private void cancelarBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            cancelarBtn.Source = cancelarBtnNo.Source;
        }
    }
}
