using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using OfficeOpenXml;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Leak_UI.Model
{
    public class WebCrowlingCollection : ViewModelProvider
    {
        public WebDriverWait _webDriverWait;
        public void SetWebDrive() {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
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
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }
        /// <summary>
        /// 로딩과,데이터처리가 정상적으로 됬을때 다음 으로 넘어가게 하는 메서드
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="path"></param>
        public void SetWebDriveWaiting(ChromeDriver driver, string path) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                _webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                _webDriverWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath(path)));

            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }
        #region WebCrowling Item
        public string webUri = "https://jc-label.mobis.co.kr/";
        public string id = "SS1F";
        public string ps = "Sekonix12@@";
        public string BOX_LABEL_HYUNDAI_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[1]/div/a";
        public string BOX_LABEL_HYUNDAI_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[2]/div/a";
        public string BOX_LABEL_HYUNDAI_SMALL = "//*[@id='MOTOR_LABEL']/ul/li[3]/div/a";
        public string BOX_LABEL_KIA_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[4]/div/a";
        public string BOX_LABEL_KIA_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[5]/div/a";

        public string MODEL_PRODUCT_ID = "ContentPlaceHolder1_txtProductID";
        public string PACKAGED_QUANTITY = "ContentPlaceHolder1_txtPackQty";
        public string PRINT_QUANTITY = "ContentPlaceHolder1_txtPrintQty";

        public string PRINT_SUCCESSCHECK = "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]";

        public static string PATH = Path.Combine(@"D:\JinYunki\Leak_UI2\Leak_UI\bin\Release", "LabelConfig.xlsx");

        public string DATALIST_BODY = "//*[@id='ContentPlaceHolder1_dxGrid2_DXMainTable']";

        private List<string> _matchItemList = new List<string>();
        public List<string> MatchItemList {
            get { return _matchItemList; }
            set {
                _matchItemList = value;
                RaisePropertyChanged(nameof(MatchItemList));
            }
        }

        private int _matchCount = 0;
        public int MatchCount {
            get { return _matchCount; }
            set {
                _matchCount = value;
                RaisePropertyChanged(nameof(MatchCount));
            }
        }

        private int _numberOfRows = 1;
        public int NumberOfRows {
            get { return _numberOfRows; }
            set {
                _numberOfRows = value;
                RaisePropertyChanged(nameof(NumberOfRows));
            }
        }

        private int _numberOfColumns = 1;
        public int NumberOfColumns {
            get { return _numberOfColumns; }
            set {
                _numberOfColumns = value;
                RaisePropertyChanged(nameof(NumberOfColumns));
            }
        }

        // 박스 라벨 타입
        private string labelType = "LABEL TYPE";
        public string LabelType {
            get { return labelType; }
            set {
                labelType = value;
                RaisePropertyChanged("LabelType");
            }
        }
        // 품번 입력
        private string product_ID = "작업지시서를 스캔 하세요";
        public string Product_ID {
            get { return product_ID; }
            set {
                product_ID = value;
                RaisePropertyChanged("Product_ID");
            }
        }
        // ModelScanCount
        private int scanCount;
        public int ScanCount {
            get { return scanCount; }
            set {
                scanCount = value;
                RaisePropertyChanged("ScanCount");
            }
        }
        // 발행 수량
        private string printCount = "1";
        public string PrintCount {
            get { return printCount; }
            set {
                printCount = value;
                RaisePropertyChanged("PrintCount");
            }
        }
        //PrintProgress
        private string printProgress = "출력 대기";
        public string PrintProgress {
            get { return printProgress; }
            set {
                printProgress = value;
                RaisePropertyChanged("PrintProgress");
            }
        }

        // Model Serial
        private string modelSerial = "";
        public string ModelSerial {
            get { return modelSerial; }
            set {
                modelSerial = value;
                RaisePropertyChanged("ModelSerial");
            }
        }
        // Box Color 
        private Brush boxColor;
        public string BoxColorString {
            get { return boxColor.ToString(); }
            set {
                BrushConverter converter = new BrushConverter();
                Brush newBrush = (Brush)converter.ConvertFromString(value);
                boxColor = newBrush;
                RaisePropertyChanged("BoxColor");
            }
        }
        public Brush BoxColor {
            get { return boxColor; }
            set {
                boxColor = value;
                RaisePropertyChanged("BoxColor");
            }
        }
        // MatchCount
        private int matchScanCount;
        public int MatchScanCount {
            get { return matchScanCount; }
            set {
                matchScanCount = value;
                if (matchScanCount == MatchCount) {
                    matchScanCount = 0;
                    ScanCount++;
                }
                RaisePropertyChanged("MatchScanCount");
            }
        }

        // BoxSize
        private string boxSize = "1";
        public string BoxSize {
            get { return boxSize; }
            set {
                boxSize = value;
                RaisePropertyChanged("BoxSize");
            }
        }

        // 포트 연결 상태
        private string resultConnect = "포트 연결을 눌러 주세요";
        public string ResultConnect {
            get { return resultConnect; }
            set {
                resultConnect = value;
                RaisePropertyChanged("ResultConnect");
            }
        }

        private bool _printSuccese = false;
        public bool PrintSuccese {
            get { return _printSuccese; }
            set {
                _printSuccese = value;
                RaisePropertyChanged("PrintSuccese");
            }
        }
        #endregion

        #region ExcelDataRead
        public void ReadExcelData(string inputData) {
            FileInfo fileInfo = new FileInfo(PATH);
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
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
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

            // 해당 데이터가 없는 경우에 대한 처리는 이곳에 추가하면 됩니다.
        }
        #endregion

        #region WebCrowling

        public void Login(ChromeDriver driver) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            SendTextInput(driver, "txtUserID", id);

            SendTextInput(driver, "txtPassword", ps);

            ClickByBtn(driver, "btnLogin");
            try {
                // Web ShowDialog Accept Check
                driver.SwitchTo().Alert().Accept();
            } catch (NoAlertPresentException) {
                
            }
            
            
        }
        public void SelectLabel(ChromeDriver driver) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                // 상단탭 부품식별표 클릭
                ClickByBtn(driver, "lblCategory_MOTOR_LABEL");

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
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }
        public void InputProductInfo(ChromeDriver driver) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                SendTextInput(driver, MODEL_PRODUCT_ID, Product_ID);
                /// 23 07 19 데이터리딩 속도때문에 쓰레드처리, 시그널로 변경
                //Thread.Sleep(500);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                By loadingIndicator = By.XPath(MODEL_PRODUCT_ID);
                wait.Until(ExpectedConditions.ElementIsVisible(loadingIndicator));

                SendTextInput(driver, PACKAGED_QUANTITY, ScanCount.ToString());

                SendTextInput(driver, PRINT_QUANTITY, PrintCount);
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            } 
        }
        public void VerifyPrintData(ChromeDriver driver) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                ClickByXPath(driver, PRINT_SUCCESSCHECK);
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }
        #endregion

        #region Web I/O Method
        // Input Text
        public void SendTextInput(IWebDriver driver, string elementId, string text) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
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
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }

        // 요소 클릭 (ID 기반)
        public void ClickByBtn(IWebDriver driver, string elementId) {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                var element = driver.FindElement(By.Id(elementId));
                element.Click();

            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }

        // 요소 클릭 (XPath 기반)
        public void ClickByXPath(IWebDriver driver, string xpath) {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

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
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }
        #endregion

    }
}
