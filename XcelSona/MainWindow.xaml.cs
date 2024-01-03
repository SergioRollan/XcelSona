using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;
using XcelSona.NotMainWindows;

namespace XcelSona
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ViewModel viewm;
        private double rangoMinPolin, rangoMaxPolin;
        private Polyline pintura;
        private Collection<Polyline> pinturasvert;
        private Collection<Rectangle> barras;
        //private MatrixTransform mt;
        private Color colorTrazo;
        private Color colorFondo;
        private Rectangle segador;
        private Point origensegador;
        private bool estaSegando;
        CreateLineWindow clw;
        public delegate void CambioColorEventHandler(object sender, CambioColorEventArgs e);


        public MainWindow()
        {
            viewm = new ViewModel();
            colorTrazo = Color.FromRgb(255, 255, 255);
            pintura = new Polyline();
            pinturasvert = new Collection<Polyline>();
            barras = new Collection<Rectangle>();
            segador = new Rectangle();
            estaSegando = false;
            //mt = new MatrixTransform();
            MaxHeight = ViewModel.totalMaxHeight;
            MaxWidth = ViewModel.totalMaxWidth;
            InitializeComponent();
        }


        public double RangoMinPolin
        {
            get { return rangoMinPolin; }
            set { rangoMinPolin = value; }
        }
        public double RangoMaxPolin
        {
            get { return rangoMaxPolin; }
            set { rangoMaxPolin = value; }
        }

        public Color ColorTrazo
        {
            get { return colorTrazo; }
            set { colorTrazo = value; }
        }

        private void graficaImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TypeGraphWindow tgw = new TypeGraphWindow(viewm);
            tgw.Width = ViewModel.totalWidth / 2;
            tgw.Height = ViewModel.totalHeight / 2;
            tgw.ShowDialog();
            if (tgw.DialogResult == true)
            {
                viewm.setTipoGrafica(tgw.resultado);
                actualizarDibujo();
            }
        }

        private void crearImg_MouseLeftButtonDown(object sender, EventArgs e)
        {
            if (clw != null && IsOpen(clw)) clw.Close();
            clw = new CreateLineWindow(viewm,this);
            clw.ResizeMode = System.Windows.ResizeMode.NoResize;
            clw.Show();
        }

        private void importarImg_MouseLeftButtonDown(object sender, EventArgs e)
        {
            //ChooseFileNameWindow cfnw = new ChooseFileNameWindow();
            //cfnw.ShowDialog();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XcelSona files (*.xs5)|*.xs5|All files (*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    if (viewm.importarImg_MouseLeftButtonDown(sender, e, elGranCanvas, ofd.FileName))
                         MessageBox.Show("Archivo importando con éxito", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Information);
                    else MessageBox.Show("El archivo no tiene un formato válido.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show("Ha ocurrido un error importando el archivo.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void exportarImg_MouseLeftButtonDown(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XcelSona file (*.xs5)|*.xs5|PNG file (*.png)|*.png";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    string ruta = sfd.FileName;
                    if (ruta.EndsWith(".png"))
                    {
                        viewm.exportarImg_MouseLeftButtonDownPNG(sender, e, elGranCanvas, ruta);
                        MessageBox.Show("Archivo PNG creado con éxito.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (ruta.EndsWith(".xs5"))
                    {
                        viewm.exportarImg_MouseLeftButtonDownXS5(sender, e, elGranCanvas, ruta);
                        MessageBox.Show("Archivo creado con éxito.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Information);
                    }else MessageBox.Show("Formato no reconocido", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception)
                {
                    MessageBox.Show("Ha ocurrido un error exportando el archivo.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

        }

        private void vaciarImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { vaciarDibujo(); }

        private void mainWin_KeyDown(object sender, KeyEventArgs e)
        {
            int whereis = (graficaImg.Source == graficaImgSi.Source) ? 1 : (vaciarImg.Source == vaciarImgSi.Source) ? 2 : (exportarImg.Source == exportarImgSi.Source) ? 3 : (importarImg.Source == importarImgSi.Source) ? 4 : (crearImg.Source == crearImgSi.Source) ? 5 : (terminarImg.Source==terminarImgSi.Source) ? 6 : 7;
            int tecla = (e.Key==Key.Up) ? 1 : (e.Key==Key.Down) ? 2 : (e.Key==Key.Enter) ? 3 : 4;
            switch (whereis)
            {
                case 1:
                    switch (tecla)
                    {
                        case 1: graficaImg.Source = graficaImgNo.Source; vaciarImg.Source = vaciarImgSi.Source; break;
                        case 2: graficaImg.Source = graficaImgNo.Source; terminarImg.Source = terminarImgSi.Source; break;
                        case 3: graficaImg_MouseLeftButtonDown(sender,null); break;
                        case 4: return;
                    }
                    break;
                case 2:
                    switch (tecla)
                    {
                        case 1: vaciarImg.Source = vaciarImgNo.Source; exportarImg.Source = exportarImgSi.Source; break;
                        case 2: vaciarImg.Source = vaciarImgNo.Source; graficaImg.Source = graficaImgSi.Source; break;
                        case 3: vaciarDibujo(); break;
                        case 4: return;
                    }
                    break;
                case 3:
                    switch (tecla)
                    {
                        case 1: exportarImg.Source = exportarImgNo.Source; importarImg.Source = importarImgSi.Source; break;
                        case 2: exportarImg.Source = exportarImgNo.Source; vaciarImg.Source = vaciarImgSi.Source; break;
                        case 3: exportarImg_MouseLeftButtonDown(sender, e); break;
                        case 4: return;
                    }
                    break;
                case 4:
                    switch (tecla)
                    {
                        case 1: importarImg.Source = importarImgNo.Source; crearImg.Source = crearImgSi.Source; break;
                        case 2: importarImg.Source = importarImgNo.Source; exportarImg.Source = exportarImgSi.Source; break;
                        case 3: importarImg_MouseLeftButtonDown(sender, e); break;
                        case 4: return;
                    }
                    break;
                case 5:
                    switch (tecla)
                    {
                        case 1: crearImg.Source = crearImgNo.Source; terminarImg.Source = terminarImgSi.Source; break;
                        case 2: crearImg.Source = crearImgNo.Source; importarImg.Source = importarImgSi.Source; break;
                        case 3: crearImg_MouseLeftButtonDown(sender, e); break;
                        case 4: return;
                    }
                    break;
                case 6:
                    switch (tecla)
                    {
                        case 1: terminarImg.Source = terminarImgNo.Source; graficaImg.Source = graficaImgSi.Source; break;
                        case 2: terminarImg.Source = terminarImgNo.Source; crearImg.Source = crearImgSi.Source; break;
                        case 3: terminarImg_MouseLeftButtonDown(sender, e); break;
                        case 4: return;
                    }
                    break;
                case 7:
                    if (tecla == 1 || tecla == 2) crearImg.Source = crearImgSi.Source;
                    break;
            }
        }


        private void crearImg_MouseEnter(object sender, MouseEventArgs e)
        {
            crearImg.Source = crearImgSi.Source; importarImg.Source = importarImgNo.Source; exportarImg.Source = exportarImgNo.Source; vaciarImg.Source = vaciarImgNo.Source; graficaImg.Source = graficaImgNo.Source; terminarImg.Source = terminarImgNo.Source;
        }

        private void crearImg_MouseLeave(object sender, MouseEventArgs e) { crearImg.Source = crearImgNo.Source; }

        private void importarImg_MouseEnter(object sender, MouseEventArgs e)
        {
            crearImg.Source = crearImgNo.Source; importarImg.Source = importarImgSi.Source; exportarImg.Source = exportarImgNo.Source; vaciarImg.Source = vaciarImgNo.Source; graficaImg.Source = graficaImgNo.Source; terminarImg.Source = terminarImgNo.Source;
        }

        private void importarImg_MouseLeave(object sender, MouseEventArgs e) { importarImg.Source = importarImgNo.Source; }

        private void exportarImg_MouseEnter(object sender, MouseEventArgs e)
        {
            crearImg.Source = crearImgNo.Source; importarImg.Source = importarImgNo.Source; exportarImg.Source = exportarImgSi.Source; vaciarImg.Source = vaciarImgNo.Source; graficaImg.Source = graficaImgNo.Source; terminarImg.Source = terminarImgNo.Source;
        }

        private void exportarImg_MouseLeave(object sender, MouseEventArgs e) { exportarImg.Source = exportarImgNo.Source; }

        private void vaciarImg_MouseEnter(object sender, MouseEventArgs e)
        {
            crearImg.Source = crearImgNo.Source; importarImg.Source = importarImgNo.Source; exportarImg.Source = exportarImgNo.Source; vaciarImg.Source = vaciarImgSi.Source; graficaImg.Source = graficaImgNo.Source; terminarImg.Source = terminarImgNo.Source;
        }

        private void vaciarImg_MouseLeave(object sender, MouseEventArgs e) { vaciarImg.Source = vaciarImgNo.Source; }

        private void graficaImg_MouseEnter(object sender, MouseEventArgs e)
        {
            crearImg.Source = crearImgNo.Source; importarImg.Source = importarImgNo.Source; exportarImg.Source = exportarImgNo.Source; vaciarImg.Source = vaciarImgNo.Source; graficaImg.Source = graficaImgSi.Source; terminarImg.Source = terminarImgNo.Source;
        }

        private void graficaImg_MouseLeave(object sender, MouseEventArgs e) { graficaImg.Source = graficaImgNo.Source; }

        private void terminarImg_MouseLeftButtonDown(object sender, EventArgs e) { Application.Current.Shutdown(); }

        private void terminarImg_MouseEnter(object sender, MouseEventArgs e)
        {
            crearImg.Source = crearImgNo.Source; importarImg.Source = importarImgNo.Source; exportarImg.Source = exportarImgNo.Source; vaciarImg.Source = vaciarImgNo.Source; graficaImg.Source = graficaImgNo.Source; terminarImg.Source = terminarImgSi.Source;
        }

        private void terminarImg_MouseLeave(object sender, MouseEventArgs e) { terminarImg.Source = terminarImgNo.Source; }

        private void fillClrBtn_Click(object sender, RoutedEventArgs e)
        {
            ChooseColorWindow ccw = new ChooseColorWindow((SolidColorBrush)(elGranCanvas.Background));
            ccw.CambioColor += CambioFondo;
            ccw.Owner = this;
            ccw.Show();

        }

        private void lineClrBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChooseColorWindow ccw = new ChooseColorWindow((SolidColorBrush)(pintura.Stroke));
                ccw.CambioColor += CambioTrazo;
                ccw.Owner = this;
                ccw.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Debe crear una línea primero.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        internal void vaciarDibujo() { elGranCanvas.Children.Clear(); }

        void CambioTrazo(object sender, CambioColorEventArgs e)
        {
            colorTrazo = e.ColorCambiado;
            colorTrazo.A = 255;
            try
            {
                pintura.Stroke = new SolidColorBrush(colorTrazo);
            }
            catch (Exception) { }
            try
            {
                foreach (Polyline pln in pinturasvert) pln.Stroke = new SolidColorBrush(colorTrazo);
            }
            catch (Exception) { }
            try
            {
                foreach (Rectangle rct in barras)
                {
                    rct.Stroke = new SolidColorBrush(colorTrazo);
                    rct.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                }
            }
            catch (Exception) { }
        }

        void CambioFondo(object sender, CambioColorEventArgs e)
        {
            colorFondo = e.ColorCambiado;
            colorFondo.A = 255;
            SolidColorBrush brocha = new SolidColorBrush(colorFondo);
            elGranCanvas.Background = brocha;
        }

        internal void cambiarPintura()
        {

            //Quitar bindings
            BindingOperations.ClearBinding(pintura, Polyline.StrokeThicknessProperty);
            pintura.StrokeThickness = thicknessSlider.Value;
            foreach (Polyline pl in pinturasvert)
            {
                BindingOperations.ClearBinding(pl, Polyline.StrokeThicknessProperty);
                pl.StrokeThickness = thicknessSlider.Value;
            }
            foreach (Rectangle rct in barras)
            {
                BindingOperations.ClearBinding(rct, Rectangle.StrokeThicknessProperty);
                rct.StrokeThickness = thicknessSlider.Value;
            }

            //Quitar asignaciones
            viewm.reset();
            pintura = new Polyline();
            pinturasvert = new Collection<Polyline>();
            barras = new Collection<Rectangle>();

        }

        public void actualizarDibujo()
        {
            if (viewm.noHayPuntitos()) return;
            try
            {
                if (viewm.TipoPuntos) //la grafica es con los puntos
                {
                         if (viewm.hayQueHacerPolilinea())
                    {
                        Collection<Point> c = viewm.getListaPuntitos();
                        PointCollection ptc = new PointCollection();
                        foreach (Point p in c) ptc.Add(new Point(p.X, p.Y));

                        double ancho = elGranCanvas.ActualWidth;
                        double alto = elGranCanvas.ActualHeight;

                        elGranCanvas.Children.Remove(pintura);
                        foreach (Polyline pnt in pinturasvert) elGranCanvas.Children.Remove(pnt);
                        foreach (Rectangle rg in barras) elGranCanvas.Children.Remove(rg);

                        Binding bindingST = new Binding();
                        bindingST.Source = thicknessSlider;
                        bindingST.Path = new PropertyPath("Value");
                        bindingST.Mode = BindingMode.Default;

                        pintura.SetBinding(Polyline.StrokeThicknessProperty, bindingST);


                        //Empezamos a traducir a coordenadas de pantalla
                        double xpantmax = elGranCanvas.ActualWidth,
                               ypantmax = elGranCanvas.ActualHeight,
                               xpantmin = 0,
                               ypantmin = 0;
                        double xrealmax, xrealmin, yrealmax, yrealmin;
                        xrealmax = xrealmin = ptc[0].X;
                        yrealmax = yrealmin = ptc[0].Y;
                        foreach (Point p in ptc)
                        {
                            if (p.X < xrealmin) xrealmin = p.X;
                            if (p.X > xrealmax) xrealmax = p.X;
                            if (p.Y < yrealmin) yrealmin = p.Y;
                            if (p.Y > yrealmax) yrealmax = p.Y;
                        }


                        if (viewm.esGrafica())
                        {
                            //No hacemos nada
                        }
                        else if (viewm.es1Cuad())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax < 0) xrealmax = 0;
                            else if (xrealmin > 0) xrealmin = 0;
                            //Miramos a ver las Y
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax < 0) yrealmax = 0;
                            else if (yrealmin > 0) yrealmin = 0;
                        }
                        else if (viewm.es4Cuads())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax <= 0) xrealmax = -xrealmin;
                            else if (xrealmin >= 0) xrealmin = -xrealmax;
                            //Miramos a ver las Y
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax <= 0) yrealmax = -yrealmin;
                            else if (yrealmin >= 0) yrealmin = -yrealmax;
                        }
                        else return;


                        xrealmax += 1; xrealmin -= 1;
                        yrealmax += 0.5; yrealmin -= 0.5;
                        //Estas dos cuentas no se hacen con valores relativos para ayudar a la visualización de las dimensiones de la linea


                        for (int i = 0; i < ptc.Count; i++)
                        {
                            Point p = ptc[i];
                            p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                            p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                            ptc[i] = p;
                        }
                        //Ya hemos traducido a coordenadas de pantalla

                        pintura.Points = new PointCollection(ptc);
                        pintura.Stroke = new SolidColorBrush(colorTrazo);
                        DoubleCollection dbc = new DoubleCollection();

                        switch (tipoTrazo.Text)
                        {
                            case " CONTINUO":
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 1":
                                dbc.Add(1);
                                dbc.Add(1);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 2":
                                dbc.Add(1);
                                dbc.Add(4);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 3":
                                dbc.Add(4);
                                dbc.Add(1);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " ESTILO EJE":
                                dbc.Add(4);
                                dbc.Add(4);
                                dbc.Add(1);
                                dbc.Add(4);
                                pintura.StrokeDashArray = dbc;
                                break;
                        }
                        switch (tipoRelleno.Text)
                        {
                            case " FLAT":
                                pintura.StrokeStartLineCap = PenLineCap.Flat;
                                pintura.StrokeEndLineCap = PenLineCap.Flat;
                                pintura.StrokeDashCap = PenLineCap.Flat;
                                break;
                            case " TRIANGLE":
                                pintura.StrokeStartLineCap = PenLineCap.Triangle;
                                pintura.StrokeEndLineCap = PenLineCap.Triangle;
                                pintura.StrokeDashCap = PenLineCap.Triangle;
                                break;
                            case " SQUARE":
                                pintura.StrokeStartLineCap = PenLineCap.Square;
                                pintura.StrokeEndLineCap = PenLineCap.Square;
                                pintura.StrokeDashCap = PenLineCap.Square;
                                break;
                            case " ROUND":
                                pintura.StrokeStartLineCap = PenLineCap.Round;
                                pintura.StrokeEndLineCap = PenLineCap.Round;
                                pintura.StrokeDashCap = PenLineCap.Round;
                                break;
                        }
                        switch (tipoVertice.Text)
                        {
                            case " BEVEL":
                                pintura.StrokeLineJoin = PenLineJoin.Bevel;
                                break;
                            case " MITER":
                                pintura.StrokeLineJoin = PenLineJoin.Miter;
                                break;
                            case " ROUND":
                                pintura.StrokeLineJoin = PenLineJoin.Round;
                                break;
                        }
                        elGranCanvas.Children.Add(pintura);
                    }
                    else if (viewm.hayQueHacerVertical())
                    {
                        Collection<Point> c = viewm.getListaPuntitos();
                        PointCollection ptc = new PointCollection();
                        foreach (Point p in c) ptc.Add(new Point(p.X, p.Y));

                        double ancho = elGranCanvas.ActualWidth;
                        double alto = elGranCanvas.ActualHeight;

                        elGranCanvas.Children.Remove(pintura);
                        foreach (Polyline pnt in pinturasvert) elGranCanvas.Children.Remove(pnt);
                        foreach (Rectangle rg in barras) elGranCanvas.Children.Remove(rg);


                        Binding bindingST;


                        //Calculamos las coordenadas de pantalla de los puntos antes de hacer las barritas
                        double xpantmax = elGranCanvas.ActualWidth,
                               ypantmax = elGranCanvas.ActualHeight,
                               xpantmin = 0,
                               ypantmin = 0;
                        double xrealmax, xrealmin, yrealmax, yrealmin;
                        xrealmax = xrealmin = ptc[0].X;
                        yrealmax = yrealmin = ptc[0].Y;
                        foreach (Point p in ptc)
                        {
                            if (p.X < xrealmin) xrealmin = p.X;
                            if (p.X > xrealmax) xrealmax = p.X;
                            if (p.Y < yrealmin) yrealmin = p.Y;
                            if (p.Y > yrealmax) yrealmax = p.Y;
                        }
                        double yPtObj=0;

                        if (viewm.esGrafica())
                        {
                            if (yrealmin > 0) yPtObj = yrealmin-0.1;
                        }
                        else if (viewm.es1Cuad())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax < 0) xrealmax = 0;
                            else if (xrealmin > 0) xrealmin = 0;
                            //Miramos a ver las Y, y asignamos yPtObj
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax < 0) yrealmax = 0;
                            else if (yrealmin > 0) yrealmin = 0;
                        }
                        else if (viewm.es4Cuads())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax <= 0) xrealmax = -xrealmin;
                            else if (xrealmin >= 0) xrealmin = -xrealmax;
                            //Miramos a ver las Y, y asignamos yPtObj
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax <= 0) yrealmax = -yrealmin;
                            else if (yrealmin >= 0) yrealmin = -yrealmax;
                        }
                        else return;



                        xrealmax += 1; xrealmin -= 1;
                        yrealmax += 0.5; yrealmin -= 0.5;
                        //Estas dos cuentas no se hacen con valores relativos para ayudar a la visualización de las dimensiones de la linea

                        pinturasvert.Clear();
                        Polyline plnTemp;
                        yPtObj = (ypantmin - ypantmax) * ((yPtObj - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                        for (int i = 0; i < ptc.Count; i++)
                        {
                            Point p = ptc[i];
                            p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                            p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                            ptc[i] = p;
                            plnTemp = new Polyline();

                            bindingST = new Binding();
                            bindingST.Source = thicknessSlider;
                            bindingST.Path = new PropertyPath("Value");
                            bindingST.Mode = BindingMode.Default;
                            plnTemp.SetBinding(Polyline.StrokeThicknessProperty, bindingST);

                            plnTemp.Points = new PointCollection();
                            plnTemp.Points.Add(new Point(p.X,p.Y));
                            plnTemp.Points.Add(new Point(p.X, yPtObj));
                            plnTemp.Stroke = new SolidColorBrush(colorTrazo);
                            DoubleCollection dbc = new DoubleCollection();

                            switch (tipoTrazo.Text)
                            {
                                case " CONTINUO":
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 1":
                                    dbc.Add(1);
                                    dbc.Add(1);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 2":
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 3":
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " ESTILO EJE":
                                    dbc.Add(4);
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                            }
                            switch (tipoRelleno.Text)
                            {
                                case " FLAT":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Flat;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Flat;
                                    plnTemp.StrokeDashCap = PenLineCap.Flat;
                                    break;
                                case " TRIANGLE":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Triangle;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Triangle;
                                    plnTemp.StrokeDashCap = PenLineCap.Triangle;
                                    break;
                                case " SQUARE":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Square;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Square;
                                    plnTemp.StrokeDashCap = PenLineCap.Square;
                                    break;
                                case " ROUND":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Round;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Round;
                                    plnTemp.StrokeDashCap = PenLineCap.Round;
                                    break;
                            }
                            switch (tipoVertice.Text)
                            {
                                case " BEVEL":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Bevel;
                                    break;
                                case " MITER":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Miter;
                                    break;
                                case " ROUND":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Round;
                                    break;
                            }
                            pinturasvert.Add(plnTemp);
                        }
                        pintura.Stroke = new SolidColorBrush(colorTrazo);
                        foreach (Polyline pln in pinturasvert) elGranCanvas.Children.Add(pln);

                    }
                    else if (viewm.hayQueHacerMix())
                    {
                        Collection<Point> c = viewm.getListaPuntitos();
                        PointCollection ptc = new PointCollection();
                        foreach (Point p in c) ptc.Add(new Point(p.X, p.Y));

                        double ancho = elGranCanvas.ActualWidth;
                        double alto = elGranCanvas.ActualHeight;

                        elGranCanvas.Children.Remove(pintura);
                        foreach (Polyline pnt in pinturasvert) elGranCanvas.Children.Remove(pnt);
                        foreach (Rectangle rg in barras) elGranCanvas.Children.Remove(rg);

                        Binding bindingST = new Binding();
                        bindingST.Source = thicknessSlider;
                        bindingST.Path = new PropertyPath("Value");
                        bindingST.Mode = BindingMode.Default;

                        pintura.SetBinding(Polyline.StrokeThicknessProperty, bindingST);


                        //Empezamos a traducir a coordenadas de pantalla
                        double xpantmax = elGranCanvas.ActualWidth,
                               ypantmax = elGranCanvas.ActualHeight,
                               xpantmin = 0,
                               ypantmin = 0;
                        double xrealmax, xrealmin, yrealmax, yrealmin;
                        xrealmax = xrealmin = ptc[0].X;
                        yrealmax = yrealmin = ptc[0].Y;
                        foreach (Point p in ptc)
                        {
                            if (p.X < xrealmin) xrealmin = p.X;
                            if (p.X > xrealmax) xrealmax = p.X;
                            if (p.Y < yrealmin) yrealmin = p.Y;
                            if (p.Y > yrealmax) yrealmax = p.Y;
                        }

                        double yPtObj = 0;
                        if (viewm.esGrafica())
                        {
                            if (yrealmin > 0) yPtObj = yrealmin;
                        }
                        else if (viewm.es1Cuad())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax < 0) xrealmax = 0;
                            else if (xrealmin > 0) xrealmin = 0;
                            //Miramos a ver las Y
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax < 0) yrealmax = 0;
                            else if (yrealmin > 0) yrealmin = 0;
                        }
                        else if (viewm.es4Cuads())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax <= 0) xrealmax = -xrealmin;
                            else if (xrealmin >= 0) xrealmin = -xrealmax;
                            //Miramos a ver las Y
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax <= 0) yrealmax = -yrealmin;
                            else if (yrealmin >= 0) yrealmin = -yrealmax;
                        }
                        else return;


                        xrealmax += 1; xrealmin -= 1;
                        yrealmax += 0.5; yrealmin -= 0.5;
                        //Estas dos cuentas no se hacen con valores relativos para ayudar a la visualización de las dimensiones de la linea


                        for (int i = 0; i < ptc.Count; i++)
                        {
                            Point p = ptc[i];
                            p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                            p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                            ptc[i] = p;
                        }
                        yPtObj = (ypantmin - ypantmax) * ((yPtObj - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                        //Ya hemos traducido a coordenadas de pantalla

                        pintura.Points = new PointCollection(ptc);
                        pintura.Stroke = new SolidColorBrush(colorTrazo);
                        DoubleCollection dbc = new DoubleCollection();

                        switch (tipoTrazo.Text)
                        {
                            case " CONTINUO":
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 1":
                                dbc.Add(1);
                                dbc.Add(1);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 2":
                                dbc.Add(1);
                                dbc.Add(4);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 3":
                                dbc.Add(4);
                                dbc.Add(1);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " ESTILO EJE":
                                dbc.Add(4);
                                dbc.Add(4);
                                dbc.Add(1);
                                dbc.Add(4);
                                pintura.StrokeDashArray = dbc;
                                break;
                        }
                        switch (tipoRelleno.Text)
                        {
                            case " FLAT":
                                pintura.StrokeStartLineCap = PenLineCap.Flat;
                                pintura.StrokeEndLineCap = PenLineCap.Flat;
                                pintura.StrokeDashCap = PenLineCap.Flat;
                                break;
                            case " TRIANGLE":
                                pintura.StrokeStartLineCap = PenLineCap.Triangle;
                                pintura.StrokeEndLineCap = PenLineCap.Triangle;
                                pintura.StrokeDashCap = PenLineCap.Triangle;
                                break;
                            case " SQUARE":
                                pintura.StrokeStartLineCap = PenLineCap.Square;
                                pintura.StrokeEndLineCap = PenLineCap.Square;
                                pintura.StrokeDashCap = PenLineCap.Square;
                                break;
                            case " ROUND":
                                pintura.StrokeStartLineCap = PenLineCap.Round;
                                pintura.StrokeEndLineCap = PenLineCap.Round;
                                pintura.StrokeDashCap = PenLineCap.Round;
                                break;
                        }
                        switch (tipoVertice.Text)
                        {
                            case " BEVEL":
                                pintura.StrokeLineJoin = PenLineJoin.Bevel;
                                break;
                            case " MITER":
                                pintura.StrokeLineJoin = PenLineJoin.Miter;
                                break;
                            case " ROUND":
                                pintura.StrokeLineJoin = PenLineJoin.Round;
                                break;
                        }
                        elGranCanvas.Children.Add(pintura);


                        pinturasvert.Clear();
                        Polyline plnTemp;
                        for (int i = 0; i < ptc.Count; i++)
                        {
                            Point p = ptc[i];
                            plnTemp = new Polyline();

                            bindingST = new Binding();
                            bindingST.Source = thicknessSlider;
                            bindingST.Path = new PropertyPath("Value");
                            bindingST.Mode = BindingMode.Default;
                            plnTemp.SetBinding(Polyline.StrokeThicknessProperty, bindingST);

                            plnTemp.Points = new PointCollection();
                            plnTemp.Points.Add(new Point(p.X, p.Y));
                            plnTemp.Points.Add(new Point(p.X, yPtObj));
                            plnTemp.Stroke = new SolidColorBrush(colorTrazo);
                            dbc = new DoubleCollection();

                            switch (tipoTrazo.Text)
                            {
                                case " CONTINUO":
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 1":
                                    dbc.Add(1);
                                    dbc.Add(1);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 2":
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 3":
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " ESTILO EJE":
                                    dbc.Add(4);
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                            }
                            switch (tipoRelleno.Text)
                            {
                                case " FLAT":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Flat;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Flat;
                                    plnTemp.StrokeDashCap = PenLineCap.Flat;
                                    break;
                                case " TRIANGLE":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Triangle;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Triangle;
                                    plnTemp.StrokeDashCap = PenLineCap.Triangle;
                                    break;
                                case " SQUARE":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Square;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Square;
                                    plnTemp.StrokeDashCap = PenLineCap.Square;
                                    break;
                                case " ROUND":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Round;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Round;
                                    plnTemp.StrokeDashCap = PenLineCap.Round;
                                    break;
                            }
                            switch (tipoVertice.Text)
                            {
                                case " BEVEL":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Bevel;
                                    break;
                                case " MITER":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Miter;
                                    break;
                                case " ROUND":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Round;
                                    break;
                            }
                            pinturasvert.Add(plnTemp);
                         }
                        foreach (Polyline pln in pinturasvert)  elGranCanvas.Children.Add(pln);
                        }
                    else //hayQueHacerBarras()
                     {
                        Collection<Point> c = viewm.getListaPuntitos();
                        PointCollection ptc = new PointCollection();
                        foreach (Point p in c) ptc.Add(new Point(p.X, p.Y));

                        double ancho = elGranCanvas.ActualWidth;
                        double alto = elGranCanvas.ActualHeight;

                        elGranCanvas.Children.Remove(pintura);
                        foreach (Polyline pnt in pinturasvert) elGranCanvas.Children.Remove(pnt);
                        foreach (Rectangle rg in barras) elGranCanvas.Children.Remove(rg);

                        Binding bindingST;
                        double xpantmax = elGranCanvas.ActualWidth,
                               ypantmax = elGranCanvas.ActualHeight,
                               xpantmin = 0,
                               ypantmin = 0;
                        double xrealmax, xrealmin, yrealmax, yrealmin;
                        xrealmax = xrealmin = ptc[0].X;
                        yrealmax = yrealmin = ptc[0].Y;
                        foreach (Point p in ptc)
                        {
                            if (p.X < xrealmin) xrealmin = p.X;
                            if (p.X > xrealmax) xrealmax = p.X;
                            if (p.Y < yrealmin) yrealmin = p.Y;
                            if (p.Y > yrealmax) yrealmax = p.Y;
                        }
                        double yPtObj = 0;

                        if (viewm.esGrafica())
                        {
                            if (yrealmin > 0) yPtObj = yrealmin - 0.1;
                        }
                        else if (viewm.es1Cuad())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax < 0) xrealmax = 0;
                            else if (xrealmin > 0) xrealmin = 0;
                            //Miramos a ver las Y, y asignamos yPtObj
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax < 0) yrealmax = 0;
                            else if (yrealmin > 0) yrealmin = 0;
                        }
                        else if (viewm.es4Cuads())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax <= 0) xrealmax = -xrealmin;
                            else if (xrealmin >= 0) xrealmin = -xrealmax;
                            //Miramos a ver las Y, y asignamos yPtObj
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax <= 0) yrealmax = -yrealmin;
                            else if (yrealmin >= 0) yrealmin = -yrealmax;
                        }
                        else return;



                        xrealmax += 1; xrealmin -= 1;
                        yrealmax += 0.5; yrealmin -= 0.5;
                        //Estas dos cuentas no se hacen con valores relativos para ayudar a la visualización de las dimensiones de la linea

                        barras.Clear();
                        Rectangle rctg;
                        yPtObj = (ypantmin - ypantmax) * ((yPtObj - yrealmin) / (yrealmax - yrealmin)) + ypantmax;

                        //Ordenamos por X la lista de puntos, en la temporal para que no se modifique el orden de la lista original
                        ObservableCollection<Point> listaOrd = new ObservableCollection<Point>();
                        double num;
                        while (ptc.Count != 0)
                        {
                            num = ptc[0].X;
                            foreach (Point p in ptc) if (p.X < num) num = p.X;
                            for (int j = 0; j < ptc.Count; j++) if (ptc[j].X == num) { listaOrd.Add(ptc[j]); ptc.RemoveAt(j--); }
                        }
                        foreach (Point p in listaOrd) ptc.Add(p);

                        for (int i = 0; i < ptc.Count; i++)
                        {
                            Point p = ptc[i];
                            p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                            p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                            Point psig;
                            if (i==(ptc.Count-1))
                            {
                                psig = new Point(0, 0);
                                psig.X = xpantmax - 1;
                            }
                            else
                            {
                                psig = new Point(ptc[i + 1].X, 0);
                                psig.X = (xpantmax - xpantmin) * ((psig.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin; //La Y no la vamos a usar
                            }


                            rctg = new Rectangle();
                            rctg.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                            rctg.Stroke = new SolidColorBrush(ColorTrazo);

                            /*
                             A-------B
                             |       |
                             |       |
                             |       |
                             C-------D
                             */

                            Point A = p;
                            Point B = psig;
                            Point C = new Point(0, yPtObj);

                            
                            Canvas.SetLeft(rctg,            A.X);
                            Canvas.SetTop (rctg,           (A.Y < C.Y) ? A.Y : C.Y);        // *
                            rctg.Width =                    B.X - A.X;                
                            rctg.Height =          Math.Abs(A.Y - C.Y);                     // *
                            // *Ese valor absoluto y esa comprobación están para las Y negativas, que tendrán el rectángulo puesto al revés, A y B debajo de C y D


                            bindingST = new Binding();
                            bindingST.Source = thicknessSlider;
                            bindingST.Path = new PropertyPath("Value");
                            bindingST.Mode = BindingMode.Default;
                            rctg.SetBinding(Rectangle.StrokeThicknessProperty, bindingST);


                            DoubleCollection dbc = new DoubleCollection();
                            switch (tipoTrazo.Text)
                            {
                                case " CONTINUO":
                                    rctg.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 1":
                                    dbc.Add(1);
                                    dbc.Add(1);
                                    rctg.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 2":
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    rctg.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 3":
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    rctg.StrokeDashArray = dbc;
                                    break;
                                case " ESTILO EJE":
                                    dbc.Add(4);
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    rctg.StrokeDashArray = dbc;
                                    break;
                            }
                            switch (tipoRelleno.Text)
                            {
                                case " FLAT":
                                    rctg.StrokeStartLineCap = PenLineCap.Flat;
                                    rctg.StrokeEndLineCap = PenLineCap.Flat;
                                    rctg.StrokeDashCap = PenLineCap.Flat;
                                    break;
                                case " TRIANGLE":
                                    rctg.StrokeStartLineCap = PenLineCap.Triangle;
                                    rctg.StrokeEndLineCap = PenLineCap.Triangle;
                                    rctg.StrokeDashCap = PenLineCap.Triangle;
                                    break;
                                case " SQUARE":
                                    rctg.StrokeStartLineCap = PenLineCap.Square;
                                    rctg.StrokeEndLineCap = PenLineCap.Square;
                                    rctg.StrokeDashCap = PenLineCap.Square;
                                    break;
                                case " ROUND":
                                    rctg.StrokeStartLineCap = PenLineCap.Round;
                                    rctg.StrokeEndLineCap = PenLineCap.Round;
                                    rctg.StrokeDashCap = PenLineCap.Round;
                                    break;
                            }
                            switch (tipoVertice.Text)
                            {
                                case " BEVEL":
                                    rctg.StrokeLineJoin = PenLineJoin.Bevel;
                                    break;
                                case " MITER":
                                    rctg.StrokeLineJoin = PenLineJoin.Miter;
                                    break;
                                case " ROUND":
                                    rctg.StrokeLineJoin = PenLineJoin.Round;
                                    break;
                            }
                            barras.Add(rctg);
                        }
                        pintura.Stroke = new SolidColorBrush(ColorTrazo);
                        foreach (Rectangle r in barras) elGranCanvas.Children.Add(r);
                    }
                }
                else //la grafica es con polinomio
                {
                    if (viewm.hayQueHacerPolilinea())
                    {
                        double ancho = elGranCanvas.ActualWidth;
                        double alto = elGranCanvas.ActualHeight;

                        Collection<Point> c = viewm.valoresPolinomio(this.rangoMinPolin, this.rangoMaxPolin, ancho * 0.95);
                        PointCollection ptc = new PointCollection();
                        foreach (Point p in c) if (!double.IsInfinity(p.Y)) ptc.Add(new Point(p.X, p.Y));


                        elGranCanvas.Children.Remove(pintura);
                        foreach (Polyline pnt in pinturasvert) elGranCanvas.Children.Remove(pnt);
                        foreach (Rectangle rg in barras) elGranCanvas.Children.Remove(rg);


                        Binding bindingST = new Binding();
                        bindingST.Source = thicknessSlider;
                        bindingST.Path = new PropertyPath("Value");
                        bindingST.Mode = BindingMode.Default;

                        pintura.SetBinding(Polyline.StrokeThicknessProperty, bindingST);


                        //Empezamos a traducir a coordenadas de pantalla
                        double xpantmax = elGranCanvas.ActualWidth,
                               ypantmax = elGranCanvas.ActualHeight,
                               xpantmin = 0,
                               ypantmin = 0;
                        double xrealmax, xrealmin, yrealmax, yrealmin;
                        xrealmax = xrealmin = ptc[0].X;
                        yrealmax = yrealmin = ptc[0].Y;
                        foreach (Point p in ptc)
                        {
                            if (p.X < xrealmin) xrealmin = p.X;
                            if (p.X > xrealmax) xrealmax = p.X;
                            if (p.Y < yrealmin) yrealmin = p.Y;
                            if (p.Y > yrealmax) yrealmax = p.Y;
                        }


                        if (viewm.esGrafica())
                        {
                            //No hacemos nada
                        }
                        else if (viewm.es1Cuad())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax < 0) xrealmax = 0;
                            else if (xrealmin > 0) xrealmin = 0;
                            //Miramos a ver las Y
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax < 0) yrealmax = 0;
                            else if (yrealmin > 0) yrealmin = 0;
                        }
                        else if (viewm.es4Cuads())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax <= 0) xrealmax = -xrealmin;
                            else if (xrealmin >= 0) xrealmin = -xrealmax;
                            //Miramos a ver las Y
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax <= 0) yrealmax = -yrealmin;
                            else if (yrealmin >= 0) yrealmin = -yrealmax;
                        }
                        else return;


                        xrealmax += 1; xrealmin -= 1;
                        yrealmax += 0.5; yrealmin -= 0.5;
                        //Estas dos cuentas no se hacen con valores relativos para ayudar a la visualización de las dimensiones de la linea


                        for (int i = 0; i < ptc.Count; i++)
                        {
                            Point p = ptc[i];
                            p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                            p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                            ptc[i] = p;
                        }
                        //Ya hemos traducido a coordenadas de pantalla

                        pintura.Points = new PointCollection(ptc);
                        pintura.Stroke = new SolidColorBrush(colorTrazo);
                        DoubleCollection dbc = new DoubleCollection();

                        switch (tipoTrazo.Text)
                        {
                            case " CONTINUO":
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 1":
                                dbc.Add(1);
                                dbc.Add(1);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 2":
                                dbc.Add(1);
                                dbc.Add(4);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " DISCONTINUO 3":
                                dbc.Add(4);
                                dbc.Add(1);
                                pintura.StrokeDashArray = dbc;
                                break;
                            case " ESTILO EJE":
                                dbc.Add(4);
                                dbc.Add(4);
                                dbc.Add(1);
                                dbc.Add(4);
                                pintura.StrokeDashArray = dbc;
                                break;
                        }
                        switch (tipoRelleno.Text)
                        {
                            case " FLAT":
                                pintura.StrokeStartLineCap = PenLineCap.Flat;
                                pintura.StrokeEndLineCap = PenLineCap.Flat;
                                pintura.StrokeDashCap = PenLineCap.Flat;
                                break;
                            case " TRIANGLE":
                                pintura.StrokeStartLineCap = PenLineCap.Triangle;
                                pintura.StrokeEndLineCap = PenLineCap.Triangle;
                                pintura.StrokeDashCap = PenLineCap.Triangle;
                                break;
                            case " SQUARE":
                                pintura.StrokeStartLineCap = PenLineCap.Square;
                                pintura.StrokeEndLineCap = PenLineCap.Square;
                                pintura.StrokeDashCap = PenLineCap.Square;
                                break;
                            case " ROUND":
                                pintura.StrokeStartLineCap = PenLineCap.Round;
                                pintura.StrokeEndLineCap = PenLineCap.Round;
                                pintura.StrokeDashCap = PenLineCap.Round;
                                break;
                        }
                        switch (tipoVertice.Text)
                        {
                            case " BEVEL":
                                pintura.StrokeLineJoin = PenLineJoin.Bevel;
                                break;
                            case " MITER":
                                pintura.StrokeLineJoin = PenLineJoin.Miter;
                                break;
                            case " ROUND":
                                pintura.StrokeLineJoin = PenLineJoin.Round;
                                break;
                        }
                        elGranCanvas.Children.Add(pintura);


                    }
                    else //los tres modos restantes saldrán exactamente igual con puntos continuos
                    {

                        double ancho = elGranCanvas.ActualWidth;
                        double alto = elGranCanvas.ActualHeight;

                        Collection<Point> c = viewm.valoresPolinomio(this.rangoMinPolin, this.rangoMaxPolin, ancho * 0.95);
                        PointCollection ptc = new PointCollection();
                        foreach (Point p in c) ptc.Add(new Point(p.X, p.Y));


                        elGranCanvas.Children.Remove(pintura);
                        foreach (Polyline pnt in pinturasvert) elGranCanvas.Children.Remove(pnt);
                        foreach (Rectangle rg in barras) elGranCanvas.Children.Remove(rg);


                        Binding bindingST;


                        //Calculamos las coordenadas de pantalla de los puntos antes de hacer las barritas
                        double xpantmax = elGranCanvas.ActualWidth,
                               ypantmax = elGranCanvas.ActualHeight,
                               xpantmin = 0,
                               ypantmin = 0;
                        double xrealmax, xrealmin, yrealmax, yrealmin;
                        xrealmax = xrealmin = ptc[0].X;
                        yrealmax = yrealmin = ptc[0].Y;
                        foreach (Point p in ptc)
                        {
                            if (p.X < xrealmin) xrealmin = p.X;
                            if (p.X > xrealmax) xrealmax = p.X;
                            if (p.Y < yrealmin) yrealmin = p.Y;
                            if (p.Y > yrealmax) yrealmax = p.Y;
                        }
                        double yPtObj = 0;

                        if (viewm.esGrafica())
                        {
                            if (yrealmin > 0) yPtObj = yrealmin;
                        }
                        else if (viewm.es1Cuad())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax < 0) xrealmax = 0;
                            else if (xrealmin > 0) xrealmin = 0;
                            //Miramos a ver las Y, y asignamos yPtObj
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax < 0) yrealmax = 0;
                            else if (yrealmin > 0) yrealmin = 0;
                        }
                        else if (viewm.es4Cuads())
                        {
                            //Miramos a ver las X
                            if (xrealmax > 0 && xrealmin < 0)
                                if (Math.Abs(xrealmax) > Math.Abs(xrealmin))
                                    xrealmin = -xrealmax;
                                else
                                    xrealmax = -xrealmin;
                            else if (xrealmax <= 0) xrealmax = -xrealmin;
                            else if (xrealmin >= 0) xrealmin = -xrealmax;
                            //Miramos a ver las Y, y asignamos yPtObj
                            if (yrealmax > 0 && yrealmin < 0)
                                if (Math.Abs(yrealmax) > Math.Abs(yrealmin))
                                    yrealmin = -yrealmax;
                                else
                                    yrealmax = -yrealmin;
                            else if (yrealmax <= 0) yrealmax = -yrealmin;
                            else if (yrealmin >= 0) yrealmin = -yrealmax;
                        }
                        else return;



                        xrealmax += 1; xrealmin -= 1;
                        yrealmax += 0.5; yrealmin -= 0.5;
                        //Estas dos cuentas no se hacen con valores relativos para ayudar a la visualización de las dimensiones de la linea

                        pinturasvert.Clear();
                        Polyline plnTemp;
                        yPtObj = (ypantmin - ypantmax) * ((yPtObj - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                        for (int i = 0; i < ptc.Count; i++)
                        {
                            Point p = ptc[i];
                            p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                            p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                            ptc[i] = p;
                            plnTemp = new Polyline();

                            bindingST = new Binding();
                            bindingST.Source = thicknessSlider;
                            bindingST.Path = new PropertyPath("Value");
                            bindingST.Mode = BindingMode.Default;
                            plnTemp.SetBinding(Polyline.StrokeThicknessProperty, bindingST);

                            plnTemp.Points = new PointCollection();
                            plnTemp.Points.Add(new Point(p.X, p.Y));
                            plnTemp.Points.Add(new Point(p.X, yPtObj));
                            plnTemp.Stroke = new SolidColorBrush(colorTrazo);
                            DoubleCollection dbc = new DoubleCollection();

                            switch (tipoTrazo.Text)
                            {
                                case " CONTINUO":
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 1":
                                    dbc.Add(1);
                                    dbc.Add(1);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 2":
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " DISCONTINUO 3":
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                                case " ESTILO EJE":
                                    dbc.Add(4);
                                    dbc.Add(4);
                                    dbc.Add(1);
                                    dbc.Add(4);
                                    plnTemp.StrokeDashArray = dbc;
                                    break;
                            }
                            switch (tipoRelleno.Text)
                            {
                                case " FLAT":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Flat;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Flat;
                                    plnTemp.StrokeDashCap = PenLineCap.Flat;
                                    break;
                                case " TRIANGLE":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Triangle;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Triangle;
                                    plnTemp.StrokeDashCap = PenLineCap.Triangle;
                                    break;
                                case " SQUARE":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Square;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Square;
                                    plnTemp.StrokeDashCap = PenLineCap.Square;
                                    break;
                                case " ROUND":
                                    plnTemp.StrokeStartLineCap = PenLineCap.Round;
                                    plnTemp.StrokeEndLineCap = PenLineCap.Round;
                                    plnTemp.StrokeDashCap = PenLineCap.Round;
                                    break;
                            }
                            switch (tipoVertice.Text)
                            {
                                case " BEVEL":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Bevel;
                                    break;
                                case " MITER":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Miter;
                                    break;
                                case " ROUND":
                                    plnTemp.StrokeLineJoin = PenLineJoin.Round;
                                    break;
                            }
                            pinturasvert.Add(plnTemp);
                        }
                        pintura.Stroke = new SolidColorBrush(colorTrazo);
                        foreach (Polyline pln in pinturasvert) elGranCanvas.Children.Add(pln);

                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Debe crear una linea primero.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void tipoTrazo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            DoubleCollection dbc = new DoubleCollection();
            switch (((ComboBoxItem)e.AddedItems[0]).Content.ToString())
            {
                case " CONTINUO":
                    pintura.StrokeDashArray = dbc;
                    foreach (Polyline pln in pinturasvert) pln.StrokeDashArray = dbc;
                    foreach (Rectangle rg in barras) rg.StrokeDashArray = dbc;
                    break;
                case " DISCONTINUO 1":
                    dbc.Add(1);
                    dbc.Add(1);
                    pintura.StrokeDashArray = dbc;
                    foreach (Polyline pln in pinturasvert) pln.StrokeDashArray = dbc;
                    foreach (Rectangle rg in barras) rg.StrokeDashArray = dbc;
                    break;
                case " DISCONTINUO 2":
                    dbc.Add(1);
                    dbc.Add(4);
                    pintura.StrokeDashArray = dbc;
                    foreach (Polyline pln in pinturasvert) pln.StrokeDashArray = dbc;
                    foreach (Rectangle rg in barras) rg.StrokeDashArray = dbc;
                    break;
                case " DISCONTINUO 3":
                    dbc.Add(4);
                    dbc.Add(1);
                    pintura.StrokeDashArray = dbc;
                    foreach (Polyline pln in pinturasvert) pln.StrokeDashArray = dbc;
                    foreach (Rectangle rg in barras) rg.StrokeDashArray = dbc;
                    break;
                case " ESTILO EJE":
                    dbc.Add(4);
                    dbc.Add(4);
                    dbc.Add(1);
                    dbc.Add(4);
                    pintura.StrokeDashArray = dbc;
                    foreach (Polyline pln in pinturasvert) pln.StrokeDashArray = dbc;
                    foreach (Rectangle rg in barras) rg.StrokeDashArray = dbc;
                    break;
            }
        }

        private void tipoRelleno_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBoxItem)e.AddedItems[0]).Content.ToString())
            {
                case " FLAT":
                    pintura.StrokeStartLineCap = PenLineCap.Flat;
                    pintura.StrokeEndLineCap = PenLineCap.Flat;
                    pintura.StrokeDashCap = PenLineCap.Flat;
                    foreach (Polyline pln in pinturasvert)
                    {
                        pln.StrokeStartLineCap = PenLineCap.Flat;
                        pln.StrokeEndLineCap = PenLineCap.Flat;
                        pln.StrokeDashCap = PenLineCap.Flat;
                    }
                    foreach (Rectangle rg in barras)
                    {
                        rg.StrokeStartLineCap = PenLineCap.Flat;
                        rg.StrokeEndLineCap = PenLineCap.Flat;
                        rg.StrokeDashCap = PenLineCap.Flat;
                    }
                    break;
                case " TRIANGLE":
                    pintura.StrokeStartLineCap = PenLineCap.Triangle;
                    pintura.StrokeEndLineCap = PenLineCap.Triangle;
                    pintura.StrokeDashCap = PenLineCap.Triangle;
                    foreach (Polyline pln in pinturasvert)
                    {
                        pln.StrokeStartLineCap = PenLineCap.Triangle;
                        pln.StrokeEndLineCap = PenLineCap.Triangle;
                        pln.StrokeDashCap = PenLineCap.Triangle;
                    }
                    foreach (Rectangle rg in barras)
                    {
                        rg.StrokeStartLineCap = PenLineCap.Triangle;
                        rg.StrokeEndLineCap = PenLineCap.Triangle;
                        rg.StrokeDashCap = PenLineCap.Triangle;
                    }
                    break;
                case " SQUARE":
                    pintura.StrokeStartLineCap = PenLineCap.Square;
                    pintura.StrokeEndLineCap = PenLineCap.Square;
                    pintura.StrokeDashCap = PenLineCap.Square;
                    foreach (Polyline pln in pinturasvert)
                    {
                        pln.StrokeStartLineCap = PenLineCap.Square;
                        pln.StrokeEndLineCap = PenLineCap.Square;
                        pln.StrokeDashCap = PenLineCap.Square;
                    }
                    foreach (Rectangle rg in barras)
                    {
                        rg.StrokeStartLineCap = PenLineCap.Square;
                        rg.StrokeEndLineCap = PenLineCap.Square;
                        rg.StrokeDashCap = PenLineCap.Square;
                    }
                    break;
                case " ROUND":
                    pintura.StrokeStartLineCap = PenLineCap.Round;
                    pintura.StrokeEndLineCap = PenLineCap.Round;
                    pintura.StrokeDashCap = PenLineCap.Round;
                    foreach (Polyline pln in pinturasvert)
                    {
                        pln.StrokeStartLineCap = PenLineCap.Round;
                        pln.StrokeEndLineCap = PenLineCap.Round;
                        pln.StrokeDashCap = PenLineCap.Round;
                    }
                    foreach (Rectangle rg in barras)
                    {
                        rg.StrokeStartLineCap = PenLineCap.Round;
                        rg.StrokeEndLineCap = PenLineCap.Round;
                        rg.StrokeDashCap = PenLineCap.Round;
                    }
                    break;
            }
        }

        private void tipoVertice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBoxItem)e.AddedItems[0]).Content.ToString())
            {
                case " BEVEL":
                    pintura.StrokeLineJoin = PenLineJoin.Bevel;
                    foreach (Polyline pln in pinturasvert) pln.StrokeLineJoin = PenLineJoin.Bevel;
                    foreach (Rectangle rg in barras) rg.StrokeLineJoin = PenLineJoin.Bevel;
                    break;
                case " MITER":
                    pintura.StrokeLineJoin = PenLineJoin.Miter;
                    foreach (Polyline pln in pinturasvert) pln.StrokeLineJoin = PenLineJoin.Miter;
                    foreach (Rectangle rg in barras) rg.StrokeLineJoin = PenLineJoin.Miter;
                    break;
                case " ROUND":
                    pintura.StrokeLineJoin = PenLineJoin.Round;
                    foreach (Polyline pln in pinturasvert) pln.StrokeLineJoin = PenLineJoin.Round;
                    foreach (Rectangle rg in barras) rg.StrokeLineJoin = PenLineJoin.Round;
                    break;
            }
        }
        private void tipoCentrado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBoxItem)e.AddedItems[0]).Content.ToString())
            {
                case " GRÁFICA":
                    viewm.ponerAGrAfica();
                    actualizarDibujo();
                    break;
                case " EJES, 1 CUAD":
                    viewm.ponerA1Cuad();
                    actualizarDibujo();
                    break;
                case " EJES, 4 CUADS":
                    viewm.ponerA4Cuads();
                    actualizarDibujo();
                    break;
            }
        }

        private void btnAxis_Click(object sender, RoutedEventArgs e)
        {
            Polyline ejeX = new Polyline(), ejeY = new Polyline();
            Point origen = new Point(0, 0);
            PointCollection pcX = new PointCollection();
            PointCollection pcY = new PointCollection();
            pcX.Add(new Point(-100, 0));
            pcX.Add(new Point( 100, 0));
            pcY.Add(new Point(0, -100));
            pcY.Add(new Point(0,  100));

            double xpantmax = elGranCanvas.ActualWidth,
                   ypantmax = elGranCanvas.ActualHeight,
                   xpantmin = 0,
                   ypantmin = 0;
            double xrealmax, xrealmin, yrealmax, yrealmin;

            xrealmax = yrealmax =  100.5;
            xrealmin = yrealmin = -100.5;

            for (int i = 0; i < pcX.Count; i++)
            {
                Point p = pcX[i];
                p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                pcX[i] = p;
            }
            for (int i = 0; i < pcY.Count; i++)
            {
                Point p = pcY[i];
                p.X = (xpantmax - xpantmin) * ((p.X - xrealmin) / (xrealmax - xrealmin)) + xpantmin;
                p.Y = (ypantmin - ypantmax) * ((p.Y - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                pcY[i] = p;
            }

            ejeX.Points = pcX;
            ejeY.Points = pcY;
            ejeX.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            ejeY.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            DoubleCollection dbc = new DoubleCollection();
            dbc.Add(4);
            dbc.Add(4);
            dbc.Add(1);
            dbc.Add(4);
            ejeX.StrokeDashArray = dbc;
            ejeY.StrokeDashArray = dbc;
            ejeX.StrokeThickness = 2;
            ejeY.StrokeThickness = 2;
            ejeX.StrokeStartLineCap = PenLineCap.Square;
            ejeY.StrokeStartLineCap = PenLineCap.Square;

            elGranCanvas.Children.Add(ejeX);
            elGranCanvas.Children.Add(ejeY);
        }


        private void elGranCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            origensegador = Mouse.GetPosition(elGranCanvas);
            Canvas.SetLeft(segador, origensegador.X);
            Canvas.SetTop(segador, origensegador.Y);
            segador.Width = 0;
            segador.Height = 0;
            segador.Stroke = new SolidColorBrush(Color.FromRgb(0,0,0));
            segador.StrokeThickness = 1.5;
            segador.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            elGranCanvas.Children.Add(segador);
            estaSegando = true;
        }

        private void elGranCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (estaSegando)
            {
                Point pt = Mouse.GetPosition(elGranCanvas);
                elGranCanvas.Children.Remove(segador);

                Point A = new Point(origensegador.X, origensegador.Y);
                Point D = new Point(pt.X, pt.Y);
                Point B = new Point(D.X, A.Y);
                Point C = new Point(A.X, D.Y);
                Point pTemp;

                /*
                 A-------B
                 |       |
                 |       |
                 |       |
                 C-------D
                 */

                if (D.X < C.X)
                {
                    //Intercambiamos valores de C y D
                    pTemp = C;
                    C = D;
                    D = pTemp;
                    //Intercambiamos valores de A y B
                    pTemp = A;
                    A = B;
                    B = pTemp;
                }

                if (D.Y < B.Y)
                {
                    //Intercambiamos valores de B y D
                    pTemp = B;
                    B = D;
                    D = pTemp;
                    //Intercambiamos valores de A y C
                    pTemp = A;
                    A = C;
                    C = pTemp;
                }



                Canvas.SetLeft(segador, A.X);
                Canvas.SetTop(segador, A.Y);
                segador.Width = B.X - A.X;
                segador.Height = C.Y - A.Y;
                elGranCanvas.Children.Add(segador);
            }
        }

        private void elGranCanvas_MouseLeave(object sender, EventArgs e)
        {
            elGranCanvas_MouseUp(sender, e);
        }

        private void elGranCanvas_MouseUp(object sender, EventArgs e)
        {
            Polyline plnTemp;
            Collection<Polyline> meter = new Collection<Polyline>();
            Collection<Polyline> sacar = new Collection<Polyline>();
            Collection<Rectangle> sacarr = new Collection<Rectangle>();
            try
            {
                Point A, D;
                A = new Point(Canvas.GetLeft(segador), Canvas.GetTop(segador));
                D = new Point(Canvas.GetLeft(segador) + segador.Width, Canvas.GetTop(segador) + segador.Height);
                PointCollection pc;
                if (estaSegando)
                {
                    bool noHayNada = true;
                    for (int ij = 0; ij < elGranCanvas.Children.Count; ij++)
                    {
                        Object obj = elGranCanvas.Children[ij];
                        if (obj is Polyline)
                        {
                            Polyline pln = (Polyline)obj;
                            plnTemp = new Polyline();
                            pc = new PointCollection();
                            foreach (Point p in pln.Points)
                            {
                                if (!(p.X < A.X || p.X > D.X || p.Y < A.Y || p.Y > D.Y))
                                {
                                    pc.Add(p);
                                    noHayNada = false;
                                }
                            }
                            
                            BindingOperations.ClearBinding(pln, Polyline.StrokeThicknessProperty);
                            pln.StrokeThickness = thicknessSlider.Value;

                            plnTemp.StrokeThickness = pln.StrokeThickness;
                            plnTemp.Stroke = pln.Stroke;
                            plnTemp.StrokeLineJoin = pln.StrokeLineJoin;
                            plnTemp.StrokeStartLineCap = pln.StrokeStartLineCap;
                            plnTemp.StrokeEndLineCap = pln.StrokeEndLineCap;
                            plnTemp.StrokeDashArray = pln.StrokeDashArray;
                            plnTemp.Points = pc;

                            sacar.Add(pln);
                            meter.Add(plnTemp);
                        }
                        else if (obj is Rectangle && (Rectangle)obj != segador)
                        {
                            Rectangle rctg = (Rectangle)obj;
                            double thicc = rctg.StrokeThickness;
                            BindingOperations.ClearBinding(rctg,Rectangle.StrokeThicknessProperty);
                            rctg.StrokeThickness = thicc;
                            Point A1, D1;
                            A1 = new Point(Canvas.GetLeft(rctg), Canvas.GetTop(rctg));
                            D1 = new Point(Canvas.GetLeft(rctg) + rctg.Width, Canvas.GetTop(rctg) + rctg.Height);
                            if (A1.X < A.X || A1.X > D.X || A1.Y < A.Y || A1.Y > D.Y || D1.X < A.X || D1.X > D.X || D1.Y < A.Y || D1.Y > D.Y)
                            {
                                sacarr.Add(rctg);
                                noHayNada = false;
                            }
                        }
                    }

                    elGranCanvas.Children.Remove(segador);
                    segador = new Rectangle();

                    if (clw != null && IsOpen(clw)) clw.Close();
                    cambiarPintura();
                    estaSegando = false;
                    if (noHayNada) return;


                    foreach (Polyline pln in sacar) elGranCanvas.Children.Remove(pln);
                    foreach (Rectangle rctg in sacarr) elGranCanvas.Children.Remove(rctg);
                    foreach (Polyline pln in meter) elGranCanvas.Children.Add(pln);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error purgando puntos.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnDupl_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                Polyline pln = new Polyline();
                Rectangle rg;
                if (viewm.hayQueHacerPolilinea() || viewm.hayQueHacerMix())
                {
                    pln = new Polyline();
                    pln.Points = pintura.Points;
                    pln.Stroke = pintura.Stroke;
                    pln.StrokeThickness = pintura.StrokeThickness;
                    pln.StrokeDashArray = pintura.StrokeDashArray;
                    pln.StrokeStartLineCap = pintura.StrokeStartLineCap;
                    pln.StrokeEndLineCap = pintura.StrokeEndLineCap;
                    pln.StrokeDashCap = pintura.StrokeDashCap;
                    pln.StrokeLineJoin = pintura.StrokeLineJoin;
                    elGranCanvas.Children.Add(pln);
                }

                if (viewm.hayQueHacerVertical() || viewm.hayQueHacerMix())
                {
                    foreach (Polyline plnt in pinturasvert)
                    {
                        pln = new Polyline();
                        pln.Points = plnt.Points;
                        pln.Stroke = plnt.Stroke;
                        pln.StrokeThickness = plnt.StrokeThickness;
                        pln.StrokeDashArray = plnt.StrokeDashArray;
                        pln.StrokeStartLineCap = plnt.StrokeStartLineCap;
                        pln.StrokeEndLineCap = plnt.StrokeEndLineCap;
                        pln.StrokeDashCap = plnt.StrokeDashCap;
                        pln.StrokeLineJoin = plnt.StrokeLineJoin;
                        elGranCanvas.Children.Add(pln);
                    }
                }

                if(!viewm.hayQueHacerVertical() && !viewm.hayQueHacerPolilinea() && !viewm.hayQueHacerMix())
                {
                    foreach (Rectangle rctg in barras)
                    {
                        rg = new Rectangle();
                        Canvas.SetLeft(rg, Canvas.GetLeft(rctg));
                        Canvas.SetTop(rg, Canvas.GetTop(rctg));
                        rg.Width = rctg.Width;
                        rg.Height = rctg.Height;
                        rg.Fill = rctg.Fill;
                        rg.Stroke = rctg.Stroke;
                        rg.StrokeThickness = rctg.StrokeThickness;
                        rg.StrokeDashArray = rctg.StrokeDashArray;
                        rg.StrokeStartLineCap = rctg.StrokeStartLineCap;
                        rg.StrokeEndLineCap = rctg.StrokeEndLineCap;
                        rg.StrokeDashCap = rctg.StrokeDashCap;
                        rg.StrokeLineJoin = rctg.StrokeLineJoin;
                        elGranCanvas.Children.Add(rg);
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Debe crear una línea primero.", "XcelSona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        public bool IsOpen(Window window) { return Application.Current.Windows.Cast<Window>().Any(x => x == window); }

    }
}
