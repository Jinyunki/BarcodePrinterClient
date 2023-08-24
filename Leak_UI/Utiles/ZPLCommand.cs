using Leak_UI.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Leak_UI.Utiles
{
    public class ZPLCommand : ViewModelProvider
    {
        public ZPLCommand() {
            
        }
        /// <summary>
        /// Font 지정 메서드
        /// </summary>
        /// <param name="fontSelect">(A: 9*5, B: 11*7, C/D: 18*10, E: 28*15, F: 26*13, G: 60*40, H: 21*13, GS: 24*24, 0: 15*12(기본))
        /// 범위A~Z(글꼴), 0~9(크기조정 가능폰트)</param>
        /// <param name="a">문자 회전 N=0도, R=90도, I=180도, B=270도</param>
        /// <param name="b">font의 높이 (범위 : 10~ 32000)</param>
        /// <param name="c">font의 넓이 (범위 : 10~ 32000)</param>
        /// <returns></returns>
        public string SelectedFont(string fontSelect,string a, string b, string c) {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                return "^A" + fontSelect + "," + a + "," + b + "," + c;
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }
    }
}
