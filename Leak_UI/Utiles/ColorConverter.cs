using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Leak_UI.Utiles
{
    class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool overTen = (bool)value;
            if (overTen) {
                return new SolidColorBrush(Colors.Red);
            } else {
                return new SolidColorBrush(Colors.Blue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
