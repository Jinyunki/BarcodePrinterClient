using System;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Leak_UI.Utiles
{
    public class IndexToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            // value는 해당 아이템의 Index 값입니다.
            // parameter로는 ScanCount 값이 전달되어야 합니다.
            int index = (int)value;
            int scanCount = System.Convert.ToInt32(parameter);

            // Index와 ScanCount가 같으면 빨간색 배경, 다르면 흰색 배경을 반환합니다.
            return index == scanCount ? Brushes.Red : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
