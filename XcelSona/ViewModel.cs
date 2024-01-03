using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using XcelSona.Model;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Shapes;
using System.Globalization;

namespace XcelSona
{
    public class ViewModel : INotifyPropertyChanged
    {
        public enum tiposGraf
        {
            polilinea,
            vertical,
            mix,
            barras
        }
        private tiposGraf tipoActual = tiposGraf.polilinea;
        private Polinomio polynom;
        private ColeccionPuntos puntitos;
        public enum tipoCentro
        {
            grafica,
            eje1cuadrante,
            eje4cuadrantes
        }
        private tipoCentro tipoCtAct = tipoCentro.grafica;
        public static double totalWidth = SystemParameters.PrimaryScreenWidth;
        public static double totalHeight = SystemParameters.PrimaryScreenHeight;
        public static double totalMaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        public static double totalMaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        private bool tipoPuntos; //true es puntos, false es polinomio
        private bool puedoHacerPolin;
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModel()
        {
            polynom = new Polinomio();
            puntitos = new ColeccionPuntos();
            tipoPuntos = false;
        }

        internal void añadirPunto(Point point) { puntitos.Add(point); }


        public bool PuedoHacerPolin
        {
            get { return puedoHacerPolin; }
            set { puedoHacerPolin = value; }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void setTipoGrafica(int resultado)
        {
            switch (resultado)
            {
                case 1: this.tipoActual = tiposGraf.polilinea; break;
                case 2: this.tipoActual = tiposGraf.vertical; break;
                case 3: this.tipoActual = tiposGraf.mix; break;
                case 4: this.tipoActual = tiposGraf.barras; break;
            }
        }

        public Boolean TipoPuntos
        {
            get { return tipoPuntos; }
            set { tipoPuntos = value; OnPropertyChanged("TipoPuntos"); }
        }



        internal void exportarImg_MouseLeftButtonDownPNG(object sender, EventArgs e, Canvas elGranCanvas, string ruta)
        {
            if (!File.Exists(ruta)) File.CreateText(ruta).Dispose();

            Rect bounds = VisualTreeHelper.GetDescendantBounds(elGranCanvas);
            double dpi = 96d;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(elGranCanvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            MemoryStream ms = new MemoryStream();

            pngEncoder.Save(ms);
            ms.Close();

            File.WriteAllBytes(ruta, ms.ToArray());
            
        }

        internal void exportarImg_MouseLeftButtonDownXS5(object sender, EventArgs e, Canvas elGranCanvas, string ruta)
        {
            if (!File.Exists(ruta)) File.CreateText(ruta).Dispose();

            StreamWriter sw = new StreamWriter(ruta);
            foreach (Object obj in elGranCanvas.Children)
            {
                if (obj is Polyline)
                {
                    Polyline pln = (Polyline)obj;

                    sw.Write("1\t");
                    sw.Write(string.Format("{0,3}", ((SolidColorBrush)pln.Stroke).Color.R) + "\t");
                    sw.Write(string.Format("{0,3}", ((SolidColorBrush)pln.Stroke).Color.G) + "\t");
                    sw.Write(string.Format("{0,3}", ((SolidColorBrush)pln.Stroke).Color.B) + "\t");
                    sw.Write(string.Format("{0,5:F2}", (pln.StrokeThickness)) + "\t");

                    DoubleCollection dbc = pln.StrokeDashArray;
                    if (dbc.Count == 0) sw.Write("0\t");
                    else if (dbc.Count == 4) sw.Write("4\t");
                    else if (dbc[0] == 1 && dbc[1] == 1) sw.Write("1\t");
                    else if (dbc[0] == 1 && dbc[1] == 4) sw.Write("2\t");
                    else if (dbc[0] == 4 && dbc[1] == 1) sw.Write("3\t");
                    else throw new Exception("");

                    if (pln.StrokeStartLineCap == PenLineCap.Flat) sw.Write("0\t");
                    else if (pln.StrokeStartLineCap == PenLineCap.Triangle) sw.Write("1\t");
                    else if (pln.StrokeStartLineCap == PenLineCap.Square) sw.Write("2\t");
                    else if (pln.StrokeStartLineCap == PenLineCap.Round) sw.Write("3\t");
                    else throw new Exception("");

                    if (pln.StrokeLineJoin == PenLineJoin.Bevel) sw.Write("0");
                    else if (pln.StrokeLineJoin == PenLineJoin.Miter) sw.Write("1");
                    else if (pln.StrokeLineJoin == PenLineJoin.Round) sw.Write("2");
                    else throw new Exception("");

                    foreach (Point p in pln.Points) sw.Write("\t" + string.Format("{0,7:F2}", p.X) + "\t" + string.Format("{0,7:F2}", p.Y));
                    sw.Write("\n");
                }
                else if (obj is Rectangle)
                {
                    Rectangle rctg = (Rectangle)obj;

                    sw.Write("2\t");
                    sw.Write(string.Format("{0,3}", ((SolidColorBrush)rctg.Stroke).Color.R) + "\t");
                    sw.Write(string.Format("{0,3}", ((SolidColorBrush)rctg.Stroke).Color.G) + "\t");
                    sw.Write(string.Format("{0,3}", ((SolidColorBrush)rctg.Stroke).Color.B) + "\t");
                    sw.Write(string.Format("{0,5:F2}", (rctg.StrokeThickness)) + "\t");

                    DoubleCollection dbc = rctg.StrokeDashArray;

                    if (dbc.Count == 0) sw.Write("0\t");
                    else if (dbc.Count == 4) sw.Write("4\t");
                    else if (dbc[0] == 1 && dbc[1] == 1) sw.Write("1\t");
                    else if (dbc[0] == 1 && dbc[1] == 4) sw.Write("2\t");
                    else if (dbc[0] == 4 && dbc[1] == 1) sw.Write("3\t");
                    else throw new Exception("");

                    if (rctg.StrokeStartLineCap == PenLineCap.Flat) sw.Write("0\t");
                    else if (rctg.StrokeStartLineCap == PenLineCap.Triangle) sw.Write("1\t");
                    else if (rctg.StrokeStartLineCap == PenLineCap.Square) sw.Write("2\t");
                    else if (rctg.StrokeStartLineCap == PenLineCap.Round) sw.Write("3\t");
                    else throw new Exception("");

                    if (rctg.StrokeLineJoin == PenLineJoin.Bevel) sw.Write("0\t");
                    else if (rctg.StrokeLineJoin == PenLineJoin.Miter) sw.Write("1\t");
                    else if (rctg.StrokeLineJoin == PenLineJoin.Round) sw.Write("2\t");
                    else throw new Exception("");

                    sw.Write(string.Format("{0,7:F2}", Canvas.GetLeft(rctg)) + "\t" + string.Format("{0,7:F2}", Canvas.GetTop(rctg)) + "\t" + string.Format("{0,7:F2}", rctg.Width) + "\t" + string.Format("{0,7:F2}", rctg.Height));
                    sw.Write("\n");
                }
                else throw new Exception(""); ;
            }
            sw.Write("\n");
            sw.Close();
            
        }


        internal bool importarImg_MouseLeftButtonDown(object sender, EventArgs e, Canvas elGranCanvas, string ruta)
        {
            if (!ruta.EndsWith(".xs5")) return false;
            System.IO.StreamReader sr = new StreamReader(ruta);

            //Variables que usaremos durante las iteraciones
            Collection<Polyline> pc = new Collection<Polyline>();
            Collection<Rectangle> rc = new Collection<Rectangle>();
            Polyline ptemp;
            Rectangle rtemp;
            byte r, g, b;
            double stcc;
            int strokeDash, strokeCap, strokeJoin;
            PointCollection pC;
            DoubleCollection dbc;

            //Y comenzamos
            string linea = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string[] nums = linea.Split('\t');

                char codigo = nums[0][0];
                switch (codigo)
                {
                    case '1':
                        if(nums.Length < 8) throw new Exception("");

                        //Recogemos los valores
                        r = byte.Parse(nums[1].Trim());
                        g = byte.Parse(nums[2].Trim());
                        b = byte.Parse(nums[3].Trim());
                        stcc = doubleParse(nums[4].Trim());
                        strokeDash = int.Parse(nums[5]);
                        strokeCap = int.Parse(nums[6]);
                        strokeJoin = int.Parse(nums[7]);
                        pC = new PointCollection();
                        for(int ij=8; ij < nums.Length; ij+=2)
                            pC.Add(new Point(doubleParse(nums[ij]), doubleParse(nums[ij+1])));

                        //Asignamos al child
                        ptemp = new Polyline();
                        ptemp.Stroke = new SolidColorBrush(Color.FromRgb((byte)r,(byte)g,(byte)b));
                        ptemp.StrokeThickness = stcc;
                        dbc = new DoubleCollection();
                        switch (strokeDash)
                        {
                            case 0:
                                ptemp.StrokeDashArray = dbc;
                                break;
                            case 1:
                                dbc.Add(1);
                                dbc.Add(1);
                                ptemp.StrokeDashArray = dbc;
                                break;
                            case 2:
                                dbc.Add(1);
                                dbc.Add(4);
                                ptemp.StrokeDashArray = dbc;
                                break;
                            case 3:
                                dbc.Add(4);
                                dbc.Add(1);
                                ptemp.StrokeDashArray = dbc;
                                break;
                            case 4:
                                dbc.Add(4);
                                dbc.Add(4);
                                dbc.Add(1);
                                dbc.Add(4);
                                ptemp.StrokeDashArray = dbc;
                                break;
                            default: throw new Exception("");
                        }
                        switch (strokeCap)
                        {
                            case 0:
                                ptemp.StrokeStartLineCap = PenLineCap.Flat;
                                ptemp.StrokeEndLineCap = PenLineCap.Flat;
                                ptemp.StrokeDashCap = PenLineCap.Flat;
                                break;
                            case 1:
                                ptemp.StrokeStartLineCap = PenLineCap.Triangle;
                                ptemp.StrokeEndLineCap = PenLineCap.Triangle;
                                ptemp.StrokeDashCap = PenLineCap.Triangle;
                                break;
                            case 2:
                                ptemp.StrokeStartLineCap = PenLineCap.Square;
                                ptemp.StrokeEndLineCap = PenLineCap.Square;
                                ptemp.StrokeDashCap = PenLineCap.Square;
                                break;
                            case 3:
                                ptemp.StrokeStartLineCap = PenLineCap.Round;
                                ptemp.StrokeEndLineCap = PenLineCap.Round;
                                ptemp.StrokeDashCap = PenLineCap.Round;
                                break;
                            default: throw new Exception("");
                        }
                        switch (strokeJoin)
                        {
                            case 0:
                                ptemp.StrokeLineJoin = PenLineJoin.Bevel;
                                break;
                            case 1:
                                ptemp.StrokeLineJoin = PenLineJoin.Miter;
                                break;
                            case 2:
                                ptemp.StrokeLineJoin = PenLineJoin.Round;
                                break;
                            default: throw new Exception("");
                        }
                        ptemp.Points = pC;
                        pc.Add(ptemp);
                        break;



                    case '2': 
                        if(nums.Length != 12) throw new Exception("");

                        //Recogemos los valores
                        r = byte.Parse(nums[1].Trim());
                        g = byte.Parse(nums[2].Trim());
                        b = byte.Parse(nums[3].Trim());
                        stcc = doubleParse(nums[4].Trim());
                        strokeDash = int.Parse(nums[5]);
                        strokeCap = int.Parse(nums[6]);
                        strokeJoin = int.Parse(nums[7]);

                        pC = new PointCollection();

                        //Asignamos al child
                        rtemp = new Rectangle();
                        Canvas.SetLeft(rtemp, doubleParse(nums[8]));
                        Canvas.SetTop(rtemp, doubleParse(nums[9]));
                        rtemp.Width = doubleParse(nums[10]);
                        rtemp.Height = doubleParse(nums[11]);
                        rtemp.Stroke = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
                        rtemp.StrokeThickness = stcc;
                        dbc = new DoubleCollection();
                        switch (strokeDash)
                        {
                            case 0:
                                rtemp.StrokeDashArray = dbc;
                                break;
                            case 1:
                                dbc.Add(1);
                                dbc.Add(1);
                                rtemp.StrokeDashArray = dbc;
                                break;
                            case 2:
                                dbc.Add(1);
                                //dbc.Add(4);
                                rtemp.StrokeDashArray = dbc;
                                break;
                            case 3:
                                dbc.Add(4);
                                dbc.Add(1);
                                rtemp.StrokeDashArray = dbc;
                                break;
                            case 4:
                                dbc.Add(4);
                                dbc.Add(4);
                                dbc.Add(1);
                                dbc.Add(4);
                                rtemp.StrokeDashArray = dbc;
                                break;
                            default: throw new Exception("");
                        }
                        switch (strokeCap)
                        {
                            case 0:
                                rtemp.StrokeStartLineCap = PenLineCap.Flat;
                                rtemp.StrokeEndLineCap = PenLineCap.Flat;
                                rtemp.StrokeDashCap = PenLineCap.Flat;
                                break;
                            case 1:
                                rtemp.StrokeStartLineCap = PenLineCap.Triangle;
                                rtemp.StrokeEndLineCap = PenLineCap.Triangle;
                                rtemp.StrokeDashCap = PenLineCap.Triangle;
                                break;
                            case 2:
                                rtemp.StrokeStartLineCap = PenLineCap.Square;
                                rtemp.StrokeEndLineCap = PenLineCap.Square;
                                rtemp.StrokeDashCap = PenLineCap.Square;
                                break;
                            case 3:
                                rtemp.StrokeStartLineCap = PenLineCap.Round;
                                rtemp.StrokeEndLineCap = PenLineCap.Round;
                                rtemp.StrokeDashCap = PenLineCap.Round;
                                break;
                            default: throw new Exception("");
                        }
                        switch (strokeJoin)
                        {
                            case 0:
                                rtemp.StrokeLineJoin = PenLineJoin.Bevel;
                                break;
                            case 1:
                                rtemp.StrokeLineJoin = PenLineJoin.Miter;
                                break;
                            case 2:
                                rtemp.StrokeLineJoin = PenLineJoin.Round;
                                break;
                            default: throw new Exception("");
                        }
                        rc.Add(rtemp);
                        break;
                    default: throw new Exception("");
                }
                linea = sr.ReadLine();
            }
            sr.Close();
            //Ya hemos leido a todos los futuros children sin errores, asi que podemos asignar ya cada child tranquilamente
            foreach (Polyline pln in pc) elGranCanvas.Children.Add(pln);
            foreach (Rectangle rt in rc) elGranCanvas.Children.Add(rt);
            //Y terminamos
            return true;
        }

        internal void reset()
        {
            polynom.Coeficientes.Clear();
            puntitos.Clear();
        }

        internal void btnDlete_Click(object sender, RoutedEventArgs e, ListView listaPuntos)
        {
            if (listaPuntos.SelectedItems.Count == 0)  throw new Exception(""); 


            List<Point> listaTemp = new List<Point>();




            for (int i = 0; i < listaPuntos.SelectedItems.Count; i++) puntitos.Remove((Point)listaPuntos.SelectedItems[i]);

            foreach (Point p in listaPuntos.Items)
                listaTemp.Add(p);
            int tam = puntitos.Count;
            for (int i = 0; i < tam; i++) puntitos.RemoveAt(0);
            foreach (Point p in listaTemp) puntitos.Add(p);
        }

        internal void btnDleteAll_Click(object sender, RoutedEventArgs e)
        {
            int tam = puntitos.Count;
            for (int j = 0; j < tam; j++) puntitos.RemoveAt(0);
        }

        internal void btnUpDwn(object sender, RoutedEventArgs e, ListView listaPuntos, bool subir)
        {
            int index = listaPuntos.Items.IndexOf(listaPuntos.SelectedItem);
            int nindex = (subir) ? index - 1 : index + 1;
            if (nindex < 0 || nindex > puntitos.Count - 1) return;
            try
            {
                puntitos.Move(index, nindex);
            }
            catch (Exception)
            {
                return;
            }
        }

        internal void ordenarPorX(object sender, RoutedEventArgs e)
        {
            if (puntitos.Count == 0) return;
            if (estaOrdenadaAscX(puntitos)) ordenarPorXDes(sender, e); else ordenarPorXAsc(sender, e);
        }

        private void ordenarPorXAsc(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Point> listaOrd = new ObservableCollection<Point>();
            double num;
            while (puntitos.Count != 0)
            {
                num = puntitos[0].X;
                foreach (Point p in puntitos) if (p.X < num) num = p.X;
                for (int j = 0; j < puntitos.Count; j++) if (puntitos[j].X == num) { listaOrd.Add(puntitos[j]); puntitos.RemoveAt(j--); }
            }
            foreach (Point p in listaOrd) puntitos.Add(p);

        }

        private void ordenarPorXDes(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Point> listaOrd = new ObservableCollection<Point>();
            double num;
            while (puntitos.Count != 0)
            {
                num = puntitos[0].X;
                foreach (Point p in puntitos) if (p.X > num) num = p.X;
                for (int j = 0; j < puntitos.Count; j++) if (puntitos[j].X == num) { listaOrd.Add(puntitos[j]); puntitos.RemoveAt(j--); }
            }
            foreach (Point p in listaOrd) puntitos.Add(p);
        }


        internal void ordenarPorY(object sender, RoutedEventArgs e)
        {
            if (puntitos.Count == 0) return;
            if (estaOrdenadaAscY(puntitos)) ordenarPorYDes(sender, e); else ordenarPorYAsc(sender, e);
        }

        private void ordenarPorYAsc(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Point> listaOrd = new ObservableCollection<Point>();
            double num;
            while (puntitos.Count != 0)
            {
                num = puntitos[0].Y;
                foreach (Point p in puntitos) if (p.Y < num) num = p.Y;
                for (int j = 0; j < puntitos.Count; j++) if (puntitos[j].Y == num) { listaOrd.Add(puntitos[j]); puntitos.RemoveAt(j--); }
            }
            foreach (Point p in listaOrd) puntitos.Add(p);
        }

        private void ordenarPorYDes(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Point> listaOrd = new ObservableCollection<Point>();
            double num;
            while (puntitos.Count != 0)
            {
                num = puntitos[0].Y;
                foreach (Point p in puntitos) if (p.Y > num) num = p.Y;
                for (int j = 0; j < puntitos.Count; j++) if (puntitos[j].Y == num) { listaOrd.Add(puntitos[j]); puntitos.RemoveAt(j--); }
            }
            foreach (Point p in listaOrd) puntitos.Add(p);
        }

        private bool estaOrdenadaAscX(ColeccionPuntos puntitos)
        {
            double num = puntitos[0].X;
            for (int i = 1; i < puntitos.Count; i++) if (puntitos[i].X < num) return false; else num = puntitos[i].X;
            return true;
        }

        private bool estaOrdenadaAscY(ColeccionPuntos puntitos)
        {
            double num = puntitos[0].Y;
            for (int i = 1; i < puntitos.Count; i++) if (puntitos[i].Y < num) return false; else num = puntitos[i].Y;
            return true;
        }

        internal void actualizarPolinomio(object sender, TextChangedEventArgs e, Dictionary<int, double> mapa)
        {
            polynom.Coeficientes = mapa;
            Dictionary<int, double>.KeyCollection lista = mapa.Keys;
            polynom.Maxexp = lista.Max();
            polynom.Minexp = lista.Min();
        }


        internal bool hayQueHacerPolilinea() { return tipoActual==tiposGraf.polilinea; }
        internal bool hayQueHacerVertical() { return tipoActual == tiposGraf.vertical; }
        internal bool hayQueHacerMix() { return tipoActual == tiposGraf.mix; }


        internal void ponerAGrAfica() { this.tipoCtAct = tipoCentro.grafica; }
        internal void ponerA1Cuad() { this.tipoCtAct = tipoCentro.eje1cuadrante; }
        internal void ponerA4Cuads() { this.tipoCtAct = tipoCentro.eje4cuadrantes; }
        internal bool esGrafica() { return this.tipoCtAct == tipoCentro.grafica; }
        internal bool es1Cuad() { return this.tipoCtAct == tipoCentro.eje1cuadrante; }
        internal bool es4Cuads() { return this.tipoCtAct == tipoCentro.eje4cuadrantes; }

        internal Collection<Point> getListaPuntitos() { return puntitos; }

        internal bool noHayPuntitos() { return puntitos.Count == 0 && !puedoHacerPolin; }

        internal ColeccionPuntos valoresPolinomio(double min, double mx, double total)
        {
            puntitos = new ColeccionPuntos();
            if (polynom.Coeficientes.Count == 0) return puntitos;
            for (int i = 0; i < total; i++)
            {
                Point p = new Point();
                p.X = min + (mx-min)*i/total;
                p.Y = resolverPolinomio(p.X);
                puntitos.Add(p);
            }
            return puntitos;
        }

        private double resolverPolinomio(double x)
        {
            double acum = 0;

            for(int i=this.polynom.Minexp; i<=this.polynom.Maxexp; i++) 
                if(polynom.Coeficientes.ContainsKey(i)) 
                    acum += polynom.Coeficientes[i] * Math.Pow(x, i);
            
            return acum;
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
    }
}
