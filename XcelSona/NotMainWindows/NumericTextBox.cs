using System;
using System.Windows;
using System.Windows.Input;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Diagnostics;

namespace XcelSona.NotMainWindows
{
    class NumericTextBox : TextBox
    {
        public int IntValue
        {
            get
            {
                return Int32.Parse(this.Text);
            }
        }

        public double DoubleValue
        {
            get
            {
                return Double.Parse(this.Text);
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {

            base.OnPreviewTextInput(e);

            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;

            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;

            string negativeSign = numberFormatInfo.NegativeSign;

            string caracter = e.Text;

            if(!char.IsDigit(caracter[0]) && !caracter.Equals(decimalSeparator) && !caracter.Equals(negativeSign) && !caracter.Equals('\b')) e.Handled = true;
        }

    }
}
