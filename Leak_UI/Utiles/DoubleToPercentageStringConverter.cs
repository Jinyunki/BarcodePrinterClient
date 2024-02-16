using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Leak_UI.Utiles
{
    /// <summary>
    /// 실수→백분율 문자열 변환자
    /// </summary>
    public class DoubleToPercentageStringConverter : IValueConverter
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 변환하기 - Convert(sourceValue, targetType, parameter, cultureInfo)

        /// <summary>
        /// 변환하기
        /// </summary>
        /// <param name="sourceValue">소스 값</param>
        /// <param name="targetType">타겟 타입</param>
        /// <param name="parameter">매개 변수</param>
        /// <param name="cultureInfo">문화 정보</param>
        /// <returns>변환 값</returns>
        public object Convert(object sourceValue, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            double value = (double)sourceValue;

            string targetValue = string.Format("{0}%", value);

            return targetValue;
        }

        #endregion
        #region 역변환하기 - ConvertBack(sourceValue, targetType, parameter, cultureInfo)

        /// <summary>
        /// 역변환하기
        /// </summary>
        /// <param name="sourceValue">소스 값</param>
        /// <param name="targetType">타겟 타입</param>
        /// <param name="parameter">매개 변수</param>
        /// <param name="cultureInfo">문화 정보</param>
        /// <returns>역변환 값</returns>
        public object ConvertBack(object sourceValue, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
