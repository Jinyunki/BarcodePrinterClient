using Leak_UI.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Leak_UI.ViewModel
{
    public class PreviewZPL_ViewModel : ViewModelProvider
    {
        private double DPI = 305;
        public PreviewZPL_ViewModel() {
            
        }

        /// <summary>
        /// Dots를 Cm로 컨버트 하는 메서드 입니다
        /// </summary>
        public double ConvertDotsToCm(int Cm) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                double dotsPerCm = DPI / 2.54;
                return (int)Math.Round(Cm*dotsPerCm);
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }

        public void CommandJPL() {

        }
    }
}
