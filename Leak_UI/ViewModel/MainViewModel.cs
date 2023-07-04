using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Input;
using OfficeOpenXml;
using Command = Leak_UI.Utiles.Command;
using System.Globalization;
using System.Collections.ObjectModel;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Brushes = System.Windows.Media.Brushes;
using Leak_UI.Model;

namespace Leak_UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Model List Set
        /// </summary>
        private Model.MainModel model = null;
        public Model.MainModel Model {
            get { return model; }
            set { model = value; RaisePropertyChanged("Model"); }
        }
        private SerialPort serialPort1;
        private ChromeDriverService driverService;
        private ChromeOptions options;
        private ChromeDriver driver;
        private IDispatcher dispatcher;

        public MainViewModel(IDispatcher dispatcher) {
            model = new Model.MainModel();
            this.dispatcher = dispatcher;
            OpenSerialPort();
            BtnEvent();
            WinBtnEvent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Model.NumberOfColumns = 5; // 가로
            Model.NumberOfRows = 3; // 세로
        }
        

        #region GridViewStyle
        private ObservableCollection<GridItem> _gridData;
        public ObservableCollection<GridItem> GridData {
            get { return _gridData; }
            set {
                _gridData = value;
                RaisePropertyChanged(nameof(GridData));
            }
        }
        #endregion

        #region ExcelDataRead
        private string ReadExcelData(string returnValue) {
            FileInfo fileInfo = new FileInfo(Model.PATH);

            using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // 첫 번째 워크시트 선택

                int rows = worksheet.Dimension.Rows; // 세로 줄의 총 개수
                int colunm = worksheet.Dimension.Columns;

                for (int row = 1; row <= rows; row++) {
                    string cellValue = worksheet.Cells[row, 1].Value?.ToString(); // A열 값 읽기 [ 품번 , ProductID ]

                    if (cellValue == Model.Product_ID) {
                        Model.LabelType = worksheet.Cells[row, 3].Value?.ToString(); // C열 값 읽기 [ Label Type ]
                        
                        string colunmStr = worksheet.Cells[row, 4].Value?.ToString(); // D열 값 읽기 [ 가로 박스 개수 ]
                        Model.NumberOfColumns = Int32.Parse(colunmStr);

                        string rowStr = worksheet.Cells[row, 5].Value?.ToString(); // E열 값 읽기 [ 새로 박스 개수 ]
                        Model.NumberOfRows = Int32.Parse(rowStr);

                        Model.ModelSerial = worksheet.Cells[row, 6].Value?.ToString(); // ModelSerial
                        
                        Model.BoxColorString = worksheet.Cells[row, 7].Value?.ToString(); // boxColor Convert To String
                        
                        Model.MatchItem1 = worksheet.Cells[row, 8].Value?.ToString(); // matchItem1
                        Model.MatchItem2 = worksheet.Cells[row, 9].Value?.ToString(); // matchItem3
                        Model.MatchItem3 = worksheet.Cells[row, 10].Value?.ToString(); // matchItem3
                        
                        if (returnValue == Model.LabelType || returnValue == Model.ModelSerial || returnValue == Model.BoxColorString || returnValue == Model.MatchItem1 || returnValue == Model.MatchItem2 || returnValue == Model.MatchItem3) {
                            return returnValue;
                        }
                    }
                }
            }

            return null; // 해당 데이터가 없는 경우 null 반환
        }
        #endregion
        #region MatchSystem
        private bool MatchChecking(string matchData, string matchResult) {
            return matchData == matchResult; //  같은경우 true 아닌경우 false
        }
        #endregion

        private void BtnEvent() {
            Model.BtnPortConnectCommand = new Command(BtnPortConnect_Click, CanExCute);
            Model.BtnPrintCommand = new Command(BtnPrint_Click, CanExCute);
        }

        private bool CanExCute(object obj) {
            return true;
        }

        private void BtnPrint_Click(object obj) {
            Model.PrintProgress = "출력 시작";
            driverService = ChromeDriverService.CreateDefaultService();

            // 크롤링 드라이버 CMD창 Hide
            driverService.HideCommandPromptWindow = true;

            options = new ChromeOptions();

            // 크롤링 GPU 가속화 Off
            options.AddArgument("disable-gpu");

            // 크롤링 웹 View Background 처리
            //options.AddArgument("--headless");
            //options.AddArgument("ignore-certificate-errors");

            driver = new ChromeDriver(driverService, options);

            driver.Navigate().GoToUrl(Model.webUri);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // 아이디 입력
            SendTextInput(driver, "txtUserID", Model.id);
            Thread.Sleep(200);
            // 비밀번호 입력
            SendTextInput(driver, "txtPassword", Model.ps);
            Thread.Sleep(200);
            // 로그인 클릭
            ClickByBtn(driver, "btnLogin");
            try {
                // Web ShowDialog Accept Check
                driver.SwitchTo().Alert().Accept();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            Thread.Sleep(1000);

            // 상단탭 부품식별표 클릭
            ClickByBtn(driver, "lblCategory_MOTOR_LABEL");


            Thread.Sleep(1000);
            // 라벨 선택 
            switch (Model.LabelType) {
                case "H_LARGE":
                    // 라벨 선택 (현대 대 - 1330*2800)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // 라벨 선택 (현대 중 - 1580*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // 라벨 선택 (현대 소 - 1345*1430)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // 라벨 선택 (기아 대 - 1100*2900)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // 라벨 선택 (기아 중 - 1600*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // 고객 품번 입력
            SendTextInput(driver, Model.MODEL_PRODUCT_ID, Model.Product_ID);

            Thread.Sleep(2000);
            // 포장 수량 입력
            SendTextInput(driver, Model.PACKAGED_QUANTITY, Model.ScanCount.ToString());
            Thread.Sleep(1000);

            // 발행 수량 입력
            SendTextInput(driver, Model.PRINT_QUANTITY, Model.PrintCount);


            // 마지막 쿼리 데이터 확인 완료 
            Thread.Sleep(1000);
            ClickByXPath(driver, "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]");
        }
        

        private void LetsGoPrint() {
            Model.PrintProgress = "출력 시작";
            driverService = ChromeDriverService.CreateDefaultService();

            // 크롤링 드라이버 CMD창 Hide
            driverService.HideCommandPromptWindow = true;

            options = new ChromeOptions();

            // 크롤링 GPU 가속화 Off
            options.AddArgument("disable-gpu");

            // 크롤링 웹 View Background 처리
            //options.AddArgument("--headless");
            //options.AddArgument("ignore-certificate-errors");

            driver = new ChromeDriver(driverService, options);

            driver.Navigate().GoToUrl(Model.webUri);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // 아이디 입력
            SendTextInput(driver, "txtUserID", Model.id);
            Thread.Sleep(200);
            // 비밀번호 입력
            SendTextInput(driver, "txtPassword", Model.ps);
            Thread.Sleep(200);
            // 로그인 클릭
            ClickByBtn(driver, "btnLogin");
            try {
                // Web ShowDialog Accept Check
                driver.SwitchTo().Alert().Accept();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            Thread.Sleep(1000);

            // 상단탭 부품식별표 클릭
            ClickByBtn(driver, "lblCategory_MOTOR_LABEL");

            Thread.Sleep(1000);
            // 라벨 선택 
            switch (Model.LabelType) {
                case "H_LARGE":
                    // 라벨 선택 (현대 대 - 1330*2800)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // 라벨 선택 (현대 중 - 1580*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // 라벨 선택 (현대 소 - 1345*1430)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // 라벨 선택 (기아 대 - 1100*2900)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // 라벨 선택 (기아 중 - 1600*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // 고객 품번 입력
            SendTextInput(driver, Model.MODEL_PRODUCT_ID, Model.Product_ID);

            Thread.Sleep(2000);
            // 포장 수량 입력
            SendTextInput(driver, Model.PACKAGED_QUANTITY, Model.ScanCount.ToString());
            Thread.Sleep(1000);

            // 발행 수량 입력
            SendTextInput(driver, Model.PRINT_QUANTITY, Model.PrintCount);

            // 마지막 쿼리 데이터 확인 완료 
            Thread.Sleep(1000);
            ClickByXPath(driver, "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]");
        }
        private void OpenSerialPort() {

            try {
                if (serialPort1 != null && serialPort1.IsOpen) {
                    serialPort1.Close();
                    serialPort1.Dispose();
                }

                serialPort1 = new SerialPort {
                    PortName = "COM3",
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None
                };

                serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);

                serialPort1.Open();
                Model.ResultConnect = "포트 연결";
            } catch (UnauthorizedAccessException ex) {
                Model.ResultConnect = "액세스 거부: " + ex.Message;
                Console.WriteLine("액세스 거부: " + ex.Message);
                // 포트 액세스 거부 예외 처리
                // 포트를 닫고 다시 열어보세요.
                serialPort1?.Close();
                serialPort1?.Dispose();
                OpenSerialPort(); // 재귀적으로 메서드 호출
            } catch (Exception ex) {
                Model.ResultConnect = "연결 오류: " + ex.Message;
                Console.WriteLine("연결 오류: " + ex.Message);
            }
        }

        private void BtnPortConnect_Click(object obj) {
            if (serialPort1 != null && serialPort1.IsOpen) {  // 이미 포트가 열려 있는 경우
                serialPort1.Close();  // 포트 닫기
                serialPort1.Dispose();
                Model.ResultConnect = "연결 종료";
            }

            /*
            serialPort1.PortName = "COM3";  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
            serialPort1.BaudRate = 9600;  //보레이트 변경이 필요하면 숫자 변경하기
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived); //이것이 꼭 필요하다

            try {
                serialPort1.Open();  // 시리얼포트 열기
                Model.ResultConnect = "포트 연결";
            } catch (Exception ex) {
                Model.ResultConnect = "연결 오류 : " + ex.Message;
            }
            */
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            string[] indataModel = indata.Split('-');
            string resultData = indataModel[0];

            dispatcher.Invoke(() =>
            {
                GridData = new ObservableCollection<GridItem>();

                if (resultData.StartsWith("T")) {
                    HandleModelSerial(resultData);
                } else if (resultData.StartsWith("9")) {
                    HandleProductID(resultData);
                }
                GenerateGridItems();
                Model.BoxSize = GridData.Count.ToString();
            });
        }

        // ModelCounting
        private void HandleModelSerial(string resultData) {
            if (Model.ModelSerial == resultData) {
                // 매칭할것이 없을때
                if (Model.MatchItem1 == null && Model.MatchItem2 == null && Model.MatchItem3 == null) {
                    Model.ScanCount++;
                }

                if (Model.ScanCount > 0 && Model.ScanCount.ToString() == Model.BoxSize) {
                    LetsGoPrint();
                }
            } else {
                MessageBox.Show("일치하지 않는 시리얼 번호입니다\n" + (Model.ScanCount + 1) + "번 모델을 확인하세요.");
            }
        }

        // ProductIdCatch
        private void HandleProductID(string resultData) {
            Model.Product_ID = resultData;
            ReadExcelData(Model.LabelType);
        }
        // Grid Add
        private void GenerateGridItems() {
            for (int i = 0; i < Model.NumberOfColumns * Model.NumberOfRows; i++) {
                GridItem gridItem = new GridItem {
                    Index = i + 1,
                    ModelSerial = "",
                    Background = Brushes.White
                };

                if (i < Model.ScanCount) {
                    gridItem.Background = Brushes.Green;
                    gridItem.ModelSerial = Model.ModelSerial;
                }

                GridData.Add(gridItem);
            }
        }



        #region Web I/O Method
        // Input Text
        void SendTextInput(IWebDriver driver, string elementId, string text) {
            var element = driver.FindElement(By.Id(elementId));
            element.SendKeys(text);

            // 모델 번호가 정상적으로 들어왔을시 엔터로 조회 진행
            if (text.StartsWith("99")) {
                element.SendKeys(Keys.Enter);
            }

            // 포장 수량 입력시 엔터로 적용
            if (elementId.Equals(Model.PACKAGED_QUANTITY)) {
                element.SendKeys(Keys.Enter);
                try {
                    driver.SwitchTo().Alert().Accept();
                } catch (Exception) {

                }
            }

            // 발행 수량 입력 후 엔터로 출력 진행
            if (elementId.Equals(Model.PRINT_QUANTITY)) {
                element.SendKeys(Keys.Enter);
                try {
                    driver.SwitchTo().Alert().Accept();
                } catch (Exception) {

                }
            }
        }

        // 요소 클릭 (ID 기반)
        void ClickByBtn(IWebDriver driver, string elementId) {
            var element = driver.FindElement(By.Id(elementId));
            element.Click();

        }

        // 요소 클릭 (XPath 기반)
        void ClickByXPath(IWebDriver driver, string xpath) {
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
                    Model.PrintProgress = "출력 완료 !";
                }
            }
        }
        #endregion

        #region Window State

        private WindowState _windowState;
        public WindowState WindowState {
            get { return _windowState; }
            set {
                if (_windowState != value) {
                    _windowState = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ICommand BtnMinmize { get; private set; }
        public ICommand BtnMaxsize { get; private set; }
        public ICommand BtnClose { get; private set; }

        private void WinBtnEvent() {
            BtnMinmize = new RelayCommand(WinMinmize);
            BtnMaxsize = new RelayCommand(WinMaxSize);
            BtnClose = new RelayCommand(WindowClose);
        }

       
        // Window Minimize
        private void WinMinmize() {
            WindowState = WindowState.Minimized;
        }

        // Window Size
        private void WinMaxSize() {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }
        

        private void WindowClose() {
            serialPort1.Close();
            serialPort1.Dispose();

            Application.Current.Shutdown();
        }

        #endregion
    }
}