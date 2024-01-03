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
    /// Lógica de interacción para VentanaTipoGrafica.xaml
    /// </summary>
    public partial class TypeGraphWindow : Window
    {
        ViewModel viewm;
        int eleccion;
        public int resultado;
        public TypeGraphWindow(ViewModel vm)
        {
            resultado = 1;
            viewm = vm;
            InitializeComponent();
        }

        private void lineasimg_KeyDown(object sender, KeyEventArgs e)
        {
            int whereis = (lineasImg.Source == lineasImgSi.Source) ? 1 : (verticalesImg.Source == verticalesImgSi.Source) ? 2 : (mixImg.Source == mixImgSi.Source) ? 3 : (barrasImg.Source == barrasImgSi.Source) ? 4 : (aceptarImg.Source==aceptarImgSi.Source) ? 6:(cancelarImg.Source==cancelarImgSi.Source)?7:8;
            int tecla = (e.Key == Key.Up) ? 1 : (e.Key == Key.Down) ? 2 : (e.Key == Key.Enter) ? 3 : (e.Key==Key.Right) ? 4 : (e.Key==Key.Left)? 5 : 6;

            if (tecla == 4)
            {
                if (whereis < 6)
                {
                    eleccion = whereis;
                    lineasImg.Source = lineasImgNo.Source;
                    verticalesImg.Source = verticalesImgNo.Source;
                    mixImg.Source = mixImgNo.Source;
                    barrasImg.Source = barrasImgNo.Source;
                    aceptarImg.Source = aceptarImgSi.Source;
                }
                else if (whereis == 6) 
                { 
                    aceptarImg.Source = aceptarImgNo.Source;
                    cancelarImg.Source = cancelarImgSi.Source; 
                }
                else if (whereis == 7)
                {
                    cancelarImg.Source = cancelarImgNo.Source;
                    switch (eleccion)
                    {
                        case 1: lineasImg.Source = lineasImgSi.Source; break;
                        case 2: verticalesImg.Source = verticalesImgSi.Source; break;
                        case 3: mixImg.Source = mixImgSi.Source; break;
                        case 4: barrasImg.Source = barrasImgSi.Source;  break;
                    }
                }
                else lineasImg.Source = lineasImgSi.Source; //whereis vale 8
                return;
            }
            if (tecla == 5)
            {
                if (whereis < 6)
                {
                    eleccion = whereis;
                    lineasImg.Source = lineasImgNo.Source;
                    verticalesImg.Source = verticalesImgNo.Source;
                    mixImg.Source = mixImgNo.Source;
                    barrasImg.Source = barrasImgNo.Source;
                    cancelarImg.Source = cancelarImgSi.Source;
                }
                else if (whereis == 6) 
                { 
                    aceptarImg.Source = aceptarImgNo.Source;
                    switch (eleccion)
                    {
                        case 1: lineasImg.Source = lineasImgSi.Source; break;
                        case 2: verticalesImg.Source = verticalesImgSi.Source; break;
                        case 3: mixImg.Source = mixImgSi.Source; break;
                        case 4: barrasImg.Source = barrasImgSi.Source; break;
                    }
                }
                else if (whereis == 7) { cancelarImg.Source = cancelarImgNo.Source; aceptarImg.Source = aceptarImgSi.Source; }
                else lineasImg.Source = lineasImgSi.Source; //whereis vale 8
                return;
            }

            switch (whereis)
            {
                case 1:
                    switch (tecla)
                    {
                        case 1: lineasImg.Source = lineasImgNo.Source; barrasImg.Source = barrasImgSi.Source; resultado=5; break;
                        case 2: lineasImg.Source = lineasImgNo.Source; verticalesImg.Source = verticalesImgSi.Source; resultado = 2; break;
                        case 3: finalImg.Source = graf1.Source; break;
                        case 6: return;
                    }
                    break;
                case 2:
                    switch (tecla)
                    {
                        case 1: verticalesImg.Source = verticalesImgNo.Source; lineasImg.Source = lineasImgSi.Source; resultado = 1; break;
                        case 2: verticalesImg.Source = verticalesImgNo.Source; mixImg.Source = mixImgSi.Source; resultado = 3; break;
                        case 3: finalImg.Source = graf2.Source; break;
                        case 6: return;
                    }
                    break;
                case 3:
                    switch (tecla)
                    {
                        case 1: mixImg.Source = mixImgNo.Source; verticalesImg.Source = verticalesImgSi.Source; resultado = 2; break;
                        case 2: mixImg.Source = mixImgNo.Source; barrasImg.Source = barrasImgSi.Source; resultado = 4; break;
                        case 3: finalImg.Source = graf3.Source; break;
                        case 6: return;
                    }
                    break;
                case 4:
                    switch (tecla)
                    {
                        case 1: barrasImg.Source = barrasImgNo.Source; mixImg.Source = mixImgSi.Source; resultado = 3; break;
                        case 2: barrasImg.Source = barrasImgNo.Source; lineasImg.Source = lineasImgSi.Source; resultado = 5; break;
                        case 3: finalImg.Source = graf4.Source; break;
                        case 6: return;
                    }
                    break;
                case 6: if (tecla == 3) try
                        {
                            DialogResult = true;
                        }catch (Exception)
                        {
                            return;
                        }
                        break;
                case 7:
                    if (tecla == 3) try
                        {
                            DialogResult = false;
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    break;
                case 8: if (tecla != 3 || tecla != 6) lineasImg.Source = lineasImgSi.Source; resultado = 1; break;
            }
        }


        private void lineasimg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            finalImg.Source = graf1.Source;
            resultado = 1;
        }

        private void lineasimg_MouseEnter(object sender, MouseEventArgs e)
        {
            lineasImg.Source = lineasImgSi.Source;
            verticalesImg.Source = verticalesImgNo.Source;
            mixImg.Source = mixImgNo.Source;
            barrasImg.Source = barrasImgNo.Source;
            aceptarImg.Source = aceptarImgNo.Source;
            cancelarImg.Source = cancelarImgNo.Source;
        }

        private void lineasimg_MouseLeave(object sender, MouseEventArgs e)
        {
            lineasImg.Source = lineasImgNo.Source;
        }


        private void verticalesimg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            finalImg.Source = graf2.Source;
            resultado = 2;
        }

        private void verticalesimg_MouseEnter(object sender, MouseEventArgs e)
        {
            lineasImg.Source = lineasImgNo.Source;
            verticalesImg.Source = verticalesImgSi.Source;
            mixImg.Source = mixImgNo.Source;
            barrasImg.Source = barrasImgNo.Source;
            aceptarImg.Source = aceptarImgNo.Source;
            cancelarImg.Source = cancelarImgNo.Source;
        }

        private void verticalesimg_MouseLeave(object sender, MouseEventArgs e)
        {
            verticalesImg.Source = verticalesImgNo.Source;
        }

        private void miximg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            finalImg.Source = graf3.Source;
            resultado = 3;
        }

        private void miximg_MouseEnter(object sender, MouseEventArgs e)
        {
            lineasImg.Source = lineasImgNo.Source;
            verticalesImg.Source = verticalesImgNo.Source;
            mixImg.Source = mixImgSi.Source;
            barrasImg.Source = barrasImgNo.Source;
            aceptarImg.Source = aceptarImgNo.Source;
            cancelarImg.Source = cancelarImgNo.Source;
        }

        private void miximg_MouseLeave(object sender, MouseEventArgs e)
        {
            mixImg.Source = mixImgNo.Source;
        }

        private void barrasimg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            finalImg.Source = graf4.Source;
            resultado = 4;
        }

        private void barrasimg_MouseEnter(object sender, MouseEventArgs e)
        {
            lineasImg.Source = lineasImgNo.Source;
            verticalesImg.Source = verticalesImgNo.Source;
            mixImg.Source = mixImgNo.Source;
            barrasImg.Source = barrasImgSi.Source;
            aceptarImg.Source = aceptarImgNo.Source;
            cancelarImg.Source = cancelarImgNo.Source;
        }

        private void barrasimg_MouseLeave(object sender, MouseEventArgs e)
        {
            barrasImg.Source = barrasImgNo.Source;
        }

        private void aceptarImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void aceptarImg_MouseEnter(object sender, MouseEventArgs e)
        {
            lineasImg.Source = lineasImgNo.Source;
            verticalesImg.Source = verticalesImgNo.Source;
            mixImg.Source = mixImgNo.Source;
            barrasImg.Source = barrasImgNo.Source;
            aceptarImg.Source = aceptarImgSi.Source;
            cancelarImg.Source = cancelarImgNo.Source;
        }

        private void aceptarImg_MouseLeave(object sender, MouseEventArgs e)
        {
            aceptarImg.Source = aceptarImgNo.Source;
        }

        private void cancelarImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void cancelarImg_MouseEnter(object sender, MouseEventArgs e)
        {
            lineasImg.Source = lineasImgNo.Source;
            verticalesImg.Source = verticalesImgNo.Source;
            mixImg.Source = mixImgNo.Source;
            barrasImg.Source = barrasImgNo.Source;
            aceptarImg.Source = aceptarImgNo.Source;
            cancelarImg.Source = cancelarImgSi.Source;
        }

        private void cancelarImg_MouseLeave(object sender, MouseEventArgs e)
        {
            cancelarImg.Source = cancelarImgNo.Source;
        }
    }
}
