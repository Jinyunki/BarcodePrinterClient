using Leak_UI.Model;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Leak_UI.Utiles
{
    public class WebDriveManager
    {
        private ChromeDriverService driverService;
        private ChromeOptions options;
        private ChromeDriver driver;
        private WebDriverWait _webDriverWait;

        public string webUri = "https://jc-label.mobis.co.kr/";

        private readonly string BOX_LABEL_CHOICE = "//*[@id='lblCategory_MOTOR_LABEL']";

        private readonly string BOX_LABEL_HYUNDAI_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[1]/div/a";
        private readonly string BOX_LABEL_HYUNDAI_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[2]/div/a";
        private readonly string BOX_LABEL_HYUNDAI_SMALL = "//*[@id='MOTOR_LABEL']/ul/li[3]/div/a";
        private readonly string BOX_LABEL_KIA_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[4]/div/a";
        private readonly string BOX_LABEL_KIA_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[5]/div/a";

        private readonly string MODEL_PRODUCT_ID = "ContentPlaceHolder1_txtProductID";
        private readonly string PACKAGED_QUANTITY = "ContentPlaceHolder1_txtPackQty";
        private readonly string PRINT_QUANTITY = "ContentPlaceHolder1_txtPrintQty";

        private readonly string PRINT_SUCCESSCHECK = "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]";
        private readonly string DATALIST_BODY = "//*[@id='ContentPlaceHolder1_dxGrid2_DXMainTable']";

        
        public string LabelType { get; set; }
        public int NumberOfColumns { get; set; }
        public int NumberOfRows { get; set; }
        public string ModelSerial { get; set; }
        public string BoxColorString { get; set; }
        public int MatchCount { get; set; }
        public List<string> MatchItemList { get; set; }
        private string PrintProgress { get; set; }
        public ObservableCollection<List<object>> WebDataList { get; set; }
        
        /// <summary>
        /// WebDrive,Option Set (해당 경로의 Uri를 입력)
        /// </summary>
        /// <param name="webUri"></param>
        public void SetWebDrive(string webUri) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {

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
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }
        }

        /// <summary>
        /// WebDrive의 기초 세팅
        /// </summary>
        /// <param name="_driverService"></param>
        /// <param name="_options"></param>
        public void DriverSet(ChromeDriverService _driverService, ChromeOptions _options) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                driver = new ChromeDriver(_driverService, _options);
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }
        }
        
        /// <summary>
        /// 로딩과,데이터처리가 정상적으로 됬을때 다음 으로 넘어가게 하는 메서드
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="path"></param>
        public void SetWebDriveWaiting(ChromeDriver driver, string path) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                _webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                _webDriverWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath(path)));
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }
        }


        
        #region WebCrowling

        public void Login(ChromeDriver driver) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            string loginID = "SS1F";
            string loginPASSWORD = "Sekonix12@@";
            SendTextInput(driver, "txtUserID", loginID);

            SendTextInput(driver, "txtPassword", loginPASSWORD);

            ClickByXPath(driver, "//*[@id='btnLogin']");
            try {
                // Web ShowDialog Accept Check
                driver.SwitchTo().Alert().Accept();
            } catch (NoAlertPresentException) {
                
            }
        }

        public void SelectLabel(ChromeDriver driver,string productID) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                // 상단탭 부품식별표 클릭
                
                ClickByXPath(driver, BOX_LABEL_CHOICE);
                switch (LabelType) {
                    case "H_LARGE":
                        ClickByXPath(driver, BOX_LABEL_HYUNDAI_LARGE);
                        break;
                    case "H_MIDDLE":
                        ClickByXPath(driver, BOX_LABEL_HYUNDAI_MIDDLE);
                        break;
                    case "H_SMALL":
                        ClickByXPath(driver, BOX_LABEL_HYUNDAI_SMALL);
                        break;
                    case "K_LARGE":
                        ClickByXPath(driver, BOX_LABEL_KIA_LARGE);
                        break;
                    case "K_MIDDLE":
                        ClickByXPath(driver, BOX_LABEL_KIA_MIDDLE);
                        break;
                }
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }
        }

        /// <summary>
        /// 정상적으로 출력이 되었는지 확인하는 로직
        /// </summary>
        /// <param name="driver"></param>
        public void VerifyPrintData(ChromeDriver driver) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                ClickByXPath(driver, PRINT_SUCCESSCHECK);
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }

        }
        #endregion

        #region Web I/O Method
        /// <summary>
        /// 파라메터로 인자 받은 text를 해당 elementId에 driver를 통해 Input해준다
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="elementId"></param>
        /// <param name="text"></param>
        public void SendTextInput(IWebDriver driver, string elementId, string text) {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                var element = driver.FindElement(By.Id(elementId));
                element.SendKeys(text);

                // 모델 번호가 정상적으로 들어왔을시 엔터로 조회 진행
                if (text.StartsWith("99")) {
                    element.SendKeys(Keys.Enter);
                }

                // 포장 수량 입력시 엔터로 적용
                if (elementId.Equals(PACKAGED_QUANTITY)) {
                    element.SendKeys(Keys.Enter);
                    try {
                        driver.SwitchTo().Alert().Accept();
                    } catch (Exception) {

                    }
                }

                // 발행 수량 입력 후 엔터로 출력 진행
                if (elementId.Equals(PRINT_QUANTITY)) {
                    element.SendKeys(Keys.Enter);
                    try {
                        driver.SwitchTo().Alert().Accept();
                    } catch (Exception) {

                    }
                }
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }

        }


        /// <summary>
        /// Web Xpath값을 참조하여 , 해당 위치를 클릭해준다
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="xpath"></param>
        public void ClickByXPath(IWebDriver driver, string xpath) {

            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                var element = driver.FindElement(By.XPath(xpath));
                element.Click();

                if (xpath.Equals("//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]")) {
                    string eleDate = element.Text; // 라벨 발행 마지막 시간 string

                    // string => dateTime casting
                    CultureInfo culture = new CultureInfo("en-US");
                    DateTime webTimeData = Convert.ToDateTime(eleDate, culture);

                    // now dateTime
                    DateTime timeNow = DateTime.Now;
                    timeNow.ToString("yyyy//MM//dd HH:mm:ss");

                    // 현재 시간 - 발행 완료 시간 = 시간 차이
                    TimeSpan result = timeNow - webTimeData;
                    // 시간(초) 차이
                    int resultTest = result.Seconds;

                    // 
                    if (resultTest < 5) {
                        PrintProgress = "출력 완료 !";
                    }
                }

            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }

        }
        /// <summary>
        /// Print를 진행하려면 , 해당 파라메터가 필요하다
        /// </summary>
        /// <param name="Product_ID"></param>
        /// <param name="ScanCount"></param>
        /// <param name="PrintCount"></param>
        /// <param name="PrintSuccese"></param>
        public void GetPrint(string Product_ID, int ScanCount, string PrintCount, bool PrintSuccese) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                PrintProgress = "출력 시작";
                SetWebDrive(webUri);

                Login(driver);

                SelectLabel(driver, Product_ID);

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
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }
        }
        #endregion


        /// <summary>
        /// Web 이력 데이터 불러오기
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="xpath"></param>
        private void LoadDataList(IWebDriver driver, string xpath) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                var _table = driver.FindElement(By.XPath(xpath));
                var _tBody = _table.FindElement(By.TagName("tbody")); // DataListBody
                var _tRows = _tBody.FindElements(By.TagName("tr"));
                List<List<object>> WebRowList = new List<List<object>>();

                foreach (var row in _tRows.Skip(1)) {
                    List<object> WebColumnList = new List<object>();
                    var columns = row.FindElements(By.TagName("td"));
                    if (columns.Count >= 2) // 최소한 2개의 열이 필요함
                    {
                        for (int i = 0; i < columns.Count; i++) {
                            object value = columns[i].Text;
                            WebColumnList.Add(value);
                        }
                        WebRowList.Add(WebColumnList);
                    }
                    WebDataList = new ObservableCollection<List<object>>(WebRowList);
                }
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }
        }

        /// <summary>
        /// 불러들인 데이터를 GetWebDataRead로 Recived
        /// </summary>
        /// <param name="driver"></param>
        public void LoadDataListAdd(ChromeDriver driver, ObservableCollection<List<object>> webDataList) {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                LoadDataList(driver, DATALIST_BODY);
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }

        }
        /// <summary>
        /// 읽어온 Web 이력 데이터를 GUI상 업데이트
        /// </summary>
        /// <param name="inputProductID"></param>
        public void GetWebDataRead(string inputProductID, ObservableCollection<List<object>> webDataList) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                SetWebDrive(webUri);

                Login(driver);

                Read_Write_ExcelData(inputProductID, ViewModelProvider.PATH);

                SelectLabel(driver, inputProductID);

                SendTextInput(driver, MODEL_PRODUCT_ID, inputProductID);

                SetWebDriveWaiting(driver, MODEL_PRODUCT_ID);

                LoadDataListAdd(driver,webDataList);
                //driver.Quit();
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                driver.Quit();
            }

        }

        /// <summary>
        /// 경로에 맞는 엑셀데이터를 읽어들여 , 각 Property값에 지정한다.
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="path"></param>
        public void Read_Write_ExcelData(string inputData,string path) {
            FileInfo fileInfo = new FileInfo(path);
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // 첫 번째 워크시트 선택

                    int rows = worksheet.Dimension.Rows; // 세로 줄의 총 개수
                    int colunm = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rows; row++) {
                        string cellValue = worksheet.Cells[row, 1].Value?.ToString(); // A열 값 읽기 [ 품번 , ProductID ]

                        if (cellValue == inputData) {
                            LabelType = worksheet.Cells[row, 3].Value?.ToString(); // C열 값 읽기 [ Label Type ]

                            string colunmStr = worksheet.Cells[row, 4].Value?.ToString(); // D열 값 읽기 [ 가로 박스 개수 ]
                            NumberOfColumns = Int32.Parse(colunmStr);

                            string rowStr = worksheet.Cells[row, 5].Value?.ToString(); // E열 값 읽기 [ 새로 박스 개수 ]
                            NumberOfRows = Int32.Parse(rowStr);

                            ModelSerial = worksheet.Cells[row, 6].Value?.ToString(); // ModelSerial

                            BoxColorString = worksheet.Cells[row, 7].Value?.ToString(); // boxColor Convert To String

                            string matchNumberOfRows = worksheet.Cells[row, 11].Value?.ToString();
                            MatchCount = Int32.Parse(matchNumberOfRows);

                            for (int i = 0; i < 3; i++) {
                                if (worksheet.Cells[row, i + 8].Value?.ToString() != "") {
                                    MatchItemList.Add(worksheet.Cells[row, i + 8].Value?.ToString());
                                }
                            }
                            break; // 해당 데이터를 찾았으므로 반복문 종료
                        }
                    }
                }
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
                throw;
            }
        }
        
    }
}
