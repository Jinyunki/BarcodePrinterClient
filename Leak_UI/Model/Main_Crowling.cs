using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Reflection;

namespace Leak_UI.Model
{
    public class Main_Crowling : WebCrowlingCollection
    {
        public void GetPrint() {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                PrintProgress = "출력 시작";
                SetWebDrive();

                Login(driver);

                SelectLabel(driver);

                // 모델 정보 입력
                SendTextInput(driver, MODEL_PRODUCT_ID, Product_ID);
                SetWebDriveWaiting(driver, MODEL_PRODUCT_ID);

                // 포장 수량 입력
                SendTextInput(driver, PACKAGED_QUANTITY, ScanCount.ToString());
                SetWebDriveWaiting(driver, PACKAGED_QUANTITY);

                // 라벨 출력 수량 입력
                SendTextInput(driver, PRINT_QUANTITY, PrintCount);
                SetWebDriveWaiting(driver, PRINT_QUANTITY);

                VerifyPrintData(driver);
                PrintSuccese = true;
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }

        
        // 들어온값에 대하여 중복이 있는가 확인하는 용도
        private List<string> resultDataList = new List<string>();
        public bool IsDuplicate(string resultData) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            // 호출되던 도중에 ScanCount가 , BoxSize와 동일하면
            // => 출력이 완료되게 되면 리스트 초기화
            try {
                if (PrintSuccese == true) {
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
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }

    }

}
