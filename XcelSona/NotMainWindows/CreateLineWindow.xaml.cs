using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using XcelSona.NotMainWindows;

namespace XcelSona
{
    /// <summary>
    /// Lógica de interacción para createWin.xaml
    /// </summary>
    public partial class CreateLineWindow : Window
    {
        public ViewModel viewm;
        public MainWindow mainw;
        bool haEliminado=false;
        public CreateLineWindow(ViewModel vm, MainWindow mw)
        {
            this.viewm = vm;
            this.mainw = mw;
            InitializeComponent();
            listaPuntos.ItemsSource = viewm.getListaPuntitos();
        }



        private void aceptPuntosImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void aceptPuntosImg_MouseEnter(object sender, MouseEventArgs e)
        {
            aceptPuntosImg.Source = aceptPuntosImgSi.Source;
        }

        private void aceptPuntosImg_MouseLeave(object sender, MouseEventArgs e)
        {
            aceptPuntosImg.Source = aceptPuntosImgNo.Source;
        }

        private void xpnd1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!haEliminado)
            {
                mainw.cambiarPintura();
                ConfirmationWindow cw = new ConfirmationWindow();
                cw.Height = ViewModel.totalHeight / 3.3;
                cw.Width = ViewModel.totalWidth / 3.3;
                cw.ShowDialog();
                if (cw.DialogResult == true)
                {
                    tabcontlNew.Items.Remove(tabcontlNew.Items.GetItemAt(1));
                    xpnd1.Header = " Introduzca los puntos";
                    haEliminado = true;
                    viewm.TipoPuntos = true;
                }
            }
        }

        private void xpnd2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!haEliminado)
            {
                mainw.cambiarPintura();
                ConfirmationWindow cw = new ConfirmationWindow();
                cw.Height = ViewModel.totalHeight / 3.5;
                cw.Width = ViewModel.totalWidth / 3.5;
                cw.ShowDialog();
                if (cw.DialogResult == true)
                {
                    tabcontlNew.Items.Remove(tabcontlNew.Items.GetItemAt(0));
                    xpnd2.Header = " Introduzca el polinomio";
                    haEliminado = true;
                    viewm.TipoPuntos = false;
                }
            }
        }

        private void createWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && aceptPuntosImg.Source == aceptPuntosImgSi.Source) Close();
            if (e.Key == Key.Down || e.Key == Key.Up) aceptPuntosImg.Source = (aceptPuntosImg.Source == aceptPuntosImgSi.Source) ? aceptPuntosImgNo.Source : aceptPuntosImgSi.Source;
            if (e.Key == Key.Left || e.Key == Key.Right) tabcontlNew.SelectedIndex = (tabcontlNew.SelectedIndex == 1) ? 0 : 1;
        }

        private void btnNewPnt_Click(object sender, RoutedEventArgs e)
        {
            if (xCoorTxt.Text.Equals(string.Empty) || yCoorTxt.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe introducir un número en ambos campos", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            viewm.añadirPunto(new Point(doubleParse(xCoorTxt.Text), doubleParse(yCoorTxt.Text)));
            xCoorTxt.Text = string.Empty;
            yCoorTxt.Text = string.Empty;
            mainw.actualizarDibujo();
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (listaPuntos.SelectedItems.Count != 1)
            {
                MessageBox.Show("Solo puede tener seleccionado un punto", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            viewm.btnUpDwn(sender, e, listaPuntos, true);
            mainw.actualizarDibujo();
        }

        private void btnDwn_Click(object sender, RoutedEventArgs e)
        {
            if (listaPuntos.SelectedItems.Count != 1)
            {
                MessageBox.Show("Solo puede tener seleccionado un punto", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            viewm.btnUpDwn(sender, e, listaPuntos, false);
            mainw.actualizarDibujo();
        }

        private void btnDlete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewm.btnDlete_Click(sender, e, listaPuntos);
            }
            catch (Exception)
            {
                MessageBox.Show("Debe seleccionar al menos un punto", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            mainw.actualizarDibujo();
        }
        private void btnDleteAll_Click(object sender, RoutedEventArgs e) { viewm.btnDleteAll_Click(sender, e); }

        private void btnSortX_Click(object sender, RoutedEventArgs e)
        {
            viewm.ordenarPorX(sender, e);
            mainw.actualizarDibujo();
        }

        private void btnSortY_Click(object sender, RoutedEventArgs e)
        {
            viewm.ordenarPorY(sender, e);
            mainw.actualizarDibujo();
        }

        private void gradoTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(gradoMaxTxt.Text) || string.IsNullOrEmpty(gradoMinTxt.Text)) return;
            try
            {
                if (int.Parse(gradoMinTxt.Text) > int.Parse(gradoMaxTxt.Text)) return;
                generarListaTextBox(int.Parse(gradoMinTxt.Text), int.Parse(gradoMaxTxt.Text));
            }
            catch (Exception)
            {
                return;
            }
        }

        private void generarListaTextBox(int ordenmin, int ordenmax)
        {
            NumericTextBox tb;
            spCoefs.Children.Clear();
            RowDefinition rd;
            viewm.PuedoHacerPolin = false;
            for (int j = ordenmin; j <= ordenmax; j++)
            {
                rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                spCoefs.RowDefinitions.Add(rd);
            }

            for (int j = ordenmax,m=0; j >= ordenmin; j--,m++)
            {
                char[] buf = j.ToString().ToCharArray();
                for (int k = 0; k < buf.Length; k++) if (buf[k].Equals('-')) buf[k] = '_';
                tb = new NumericTextBox();
                tb.Margin = new Thickness(40, 5, 40, 5);
                tb.Name = "exp" + new string(buf);
                tb.TextChanged += Tb_TextChanged;
                tb.FontFamily = new FontFamily("Segoe UI");
                spCoefs.Children.Add(tb);
                Grid.SetRow(tb, m);
            }
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool firstCall = true;
            Dictionary<int, double> mapa = new Dictionary<int, double>();
            StringBuilder respuestafinal = new StringBuilder();
            try
            {
                foreach (TextBox tb in spCoefs.Children)
                {
                    if (string.IsNullOrEmpty(tb.Text)) continue;
                    char[] buf = tb.Name.ToCharArray();
                    bool porMenosUno = false;
                    int cifra = 0, exx = 0; double basex;
                    for (int j = buf.Length-1; j >2 ; j--)
                        if (buf[j] == '_') porMenosUno = true;
                        else exx += (int)(Char.GetNumericValue(buf[j]) * Math.Pow(10, cifra++));
                    exx *= (porMenosUno) ? -1 : 1;
                    basex = double.Parse(tb.Text);
                    if (firstCall) { firstCall = false; respuestafinal.Append(basex).Append("x^").Append(exx); }
                    else respuestafinal.Append(" + ").Append(basex).Append("x^").Append(exx);
                    mapa.Add(exx, basex);
                }
                polinomioTxt.Text = respuestafinal.ToString();
                mainw.RangoMaxPolin = double.Parse(rangoMaxTxt.Text);
                mainw.RangoMinPolin = double.Parse(rangoMinTxt.Text);
                viewm.actualizarPolinomio(sender, e, mapa);
                comprobarSiEstaTodo();
                if (!viewm.PuedoHacerPolin) return;
                mainw.actualizarDibujo();
            }
            catch (Exception) 
            {
                return;
            }
        }

        private void comprobarSiEstaTodo()
        {
            viewm.PuedoHacerPolin = false;
            if (string.IsNullOrEmpty(rangoMaxTxt.Text)) return;
            if (string.IsNullOrEmpty(rangoMinTxt.Text)) return;
            try
            {
                double a = double.Parse(rangoMaxTxt.Text);
                double b = double.Parse(rangoMinTxt.Text);
                if (a <= b) return;
            }
            catch (Exception){ return; }
            if (string.IsNullOrEmpty(gradoMaxTxt.Text)) return;
            if (string.IsNullOrEmpty(gradoMinTxt.Text)) return;
            try
            {
                double a = double.Parse(gradoMaxTxt.Text);
                double b = double.Parse(gradoMinTxt.Text);
                if (a < b) return;
            }
            catch (Exception) { return; }
            foreach (TextBox tbx in spCoefs.Children)
                if (!string.IsNullOrEmpty(tbx.Text)) viewm.PuedoHacerPolin = true;
        }

        private void rangoMaxTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(rangoMinTxt.Text))
                {
                    viewm.PuedoHacerPolin = false;
                    return;
                }
                mainw.RangoMaxPolin = double.Parse(rangoMaxTxt.Text);
                comprobarSiEstaTodo();
                if (!viewm.PuedoHacerPolin) return;
                Tb_TextChanged(sender, e);
                mainw.actualizarDibujo();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void rangoMinTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(rangoMinTxt.Text))
                {
                    viewm.PuedoHacerPolin = false;
                    return;
                }
                mainw.RangoMinPolin = double.Parse(rangoMinTxt.Text);
                comprobarSiEstaTodo();
                if (!viewm.PuedoHacerPolin) return;
                Tb_TextChanged(sender, e);
                mainw.actualizarDibujo();
            }
            catch (Exception)
            {
                return;
            }
        }

        private static double doubleParse(object value)
        {
            double result;

            string doubleAsString = value.ToString();
            IEnumerable<char> doubleAsCharList = doubleAsString.ToList();

            if (doubleAsCharList.Where(ch => ch == '.' || ch == ',').Count() <= 1)
                double.TryParse(doubleAsString.Replace(',', '.'), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            else
            {
                if (doubleAsCharList.Where(ch => ch == '.').Count() <= 1 && doubleAsCharList.Where(ch => ch == ',').Count() > 1)
                {
                    double.TryParse(doubleAsString.Replace(",", string.Empty),
                        System.Globalization.NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out result);
                }
                else if (doubleAsCharList.Where(ch => ch == ',').Count() <= 1 && doubleAsCharList.Where(ch => ch == '.').Count() > 1) 
                    double.TryParse(doubleAsString.Replace(".", string.Empty).Replace(',', '.'), System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result);
                else throw new Exception("");
            }

            return result;
        }

        private void borrarCM_Click(object sender, RoutedEventArgs e)
        {
            this.btnDleteAll_Click(sender, e);
        }

        private void add00_Click(object sender, RoutedEventArgs e)
        {
            viewm.añadirPunto(new Point(0,0));
            mainw.actualizarDibujo();
        }

        private void add10_Click(object sender, RoutedEventArgs e)
        {
            viewm.añadirPunto(new Point(1, 0));
            mainw.actualizarDibujo();
        }

        private void add01_Click(object sender, RoutedEventArgs e)
        {
            viewm.añadirPunto(new Point(0, 1));
            mainw.actualizarDibujo();
        }

        private void add_10_Click(object sender, RoutedEventArgs e)
        {
            viewm.añadirPunto(new Point(-1, 0));
            mainw.actualizarDibujo();
        }

        private void add0_1_Click(object sender, RoutedEventArgs e)
        {
            viewm.añadirPunto(new Point(0, -1));
            mainw.actualizarDibujo();
        }

        private void add10C_Click(object sender, RoutedEventArgs e)
        {
            viewm.añadirPunto(new Point(100, 0));
            mainw.actualizarDibujo();
        }

        private void add01C_Click(object sender, RoutedEventArgs e)
        {
            viewm.añadirPunto(new Point(0, 100));
            mainw.actualizarDibujo();
        }

        private void ordxCM_Click(object sender, RoutedEventArgs e)
        {
            this.btnSortX_Click(sender, e);
        }

        private void ordyCM_Click(object sender, RoutedEventArgs e)
        {
            this.btnSortY_Click(sender, e);
        }
    }
}
