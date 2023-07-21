using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Leak_UI.Utiles
{
    public class MatchingManager
    {
        // 들어온값에 대하여 중복이 있는가 확인하는 용도
        private List<string> resultDataList = new List<string>();
        public bool IsDuplicate(string resultData,bool printSuccese) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            // 호출되던 도중에 ScanCount가 , BoxSize와 동일하면
            // => 출력이 완료되게 되면 리스트 초기화
            try {
                if (printSuccese == true) {
                    resultDataList.Clear();
                }

                if (!resultDataList.Contains(resultData)) {
                    // 중복된 값이 없을때 리스트 생성
                    resultDataList.Add(resultData);
                    return true;

                } else {
                    // 중복된 값이 들어왔을때 false반환
                    return false;
                }
            } catch (Exception ex) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + ex);
                throw;
            }
        }
    }
}
