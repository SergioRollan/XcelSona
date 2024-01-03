using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XcelSona
{
    public class Polinomio : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<int,double> coeficientes;
        private int maxexp;
        private int minexp;

        public Polinomio()
        {
            coeficientes = new Dictionary<int, double>();
        }

        public Dictionary<int, double> Coeficientes
        {
            get { return coeficientes; }
            set { if(value.Count>0) coeficientes = value; OnPropertyChanged("Coeficientes"); }
        }

        public int Maxexp
        {
            get { return maxexp; }
            set { maxexp = value; OnPropertyChanged("Maxexp"); }
        }

        public int Minexp
        {
            get { return minexp; }
            set { minexp = value; OnPropertyChanged("MinEexp"); }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
