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
                driverService = ChromeDriverService.CreateDefaultService();

                // 크롤링 드라이버 CMD창 Hide
                driverService.HideCommandPromptWindow = true;

                options = new ChromeOptions();

                // 크롤링 GPU 가속화 Off
                options.AddArgument("disable-gpu");

                // 크롤링 웹 View Background 처리
                //options.AddArgument("--headless");
                //options.AddArgument("ignore-certificate-errors");

                DriverSet(driverService, options);

                driver.Navigate().GoToUrl(webUri);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

                Login(driver);



                SelectLabel(driver);
                InputProductInfo(driver);
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
