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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Brush = System.Windows.Media.Brush;

namespace Leak_UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDispatcher dispatcher) {
            this.dispatcher = dispatcher;
            OpenSerialPort();
            BtnEvent();
            WinBtnEvent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            NumberOfColumns = 5; // 가로
            NumberOfRows = 3; // 세로
        }


        #region Item List

        private string webUri = "https://jc-label.mobis.co.kr/";
        private string id = "SS1F";
        private string ps = "Sekonix12@@";

        private const string BOX_LABEL_HYUNDAI_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[1]/div/a";
        private const string BOX_LABEL_HYUNDAI_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[2]/div/a";
        private const string BOX_LABEL_HYUNDAI_SMALL = "//*[@id='MOTOR_LABEL']/ul/li[3]/div/a";
        private const string BOX_LABEL_KIA_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[4]/div/a";
        private const string BOX_LABEL_KIA_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[5]/div/a";

        private const string MODEL_PRODUCT_ID = "ContentPlaceHolder1_txtProductID";
        private const string PACKAGED_QUANTITY = "ContentPlaceHolder1_txtPackQty";
        private const string PRINT_QUANTITY = "ContentPlaceHolder1_txtPrintQty";

        private string PATH = Path.Combine(@"D:\JinYunki\Leak_UI2\Leak_UI\bin\Release", "LabelConfig.xlsx");

        private SerialPort serialPort1;
        private ChromeDriverService driverService;
        private ChromeOptions options;
        private ChromeDriver driver;
        private IDispatcher dispatcher;

        #endregion

        #region GridViewStyle

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

        private ObservableCollection<GridItem> _gridData;
        public ObservableCollection<GridItem> GridData {
            get { return _gridData; }
            set {
                _gridData = value;
                RaisePropertyChanged(nameof(GridData));
            }
        }
        public class GridItem
        {
            private int index;
            public int Index {
                get { return index; }
                set {
                    if (index != value) {
                        index = value;
                        OnPropertyChanged(nameof(Index));
                    }
                }
            }

            private int value;
            public int Value {
                get { return value; }
                set {
                    if (this.value != value) {
                        this.value = value;
                        OnPropertyChanged(nameof(Value));
                        //UpdateBackground();
                    }
                }
            }
            private void UpdateBackground() {
                // Set the background color based on the value
                Background = (Value == 0) ? Brushes.White : Brushes.Red;
            }

            private Brush _background;
            public Brush Background {
                get { return _background; }
                set {
                    if (_background != value) {
                        _background = value;
                        OnPropertyChanged(nameof(Background));
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Property Item List

        //PrintProgress
        private string printProgress = "출력 대기";
        public string PrintProgress {
            get { return printProgress; }
            set {
                printProgress = value;
                RaisePropertyChanged("PrintProgress");
            }
        }

        // 박스 라벨 타입
        private string labelType = "LABEL TYPE";
        public string LabelType {
            get { return labelType; }
            set {
                labelType = value;
                RaisePropertyChanged("BoxType");
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

        // 품번 입력
        private string product_ID = "작업지시서를 스캔 하세요";
        public string Product_ID {
            get { return product_ID; }
            set {
                product_ID = value;
                RaisePropertyChanged("Product_ID");
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
        // ModelScanCount
        private int scanCount;
        public int ScanCount {
            get { return scanCount; }
            set {
                scanCount = value;
                RaisePropertyChanged("ScanCount");
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

        // 발행 수량
        private string printCount = "1";
        public string PrintCount {
            get { return printCount; }
            set {
                printCount = value;
                RaisePropertyChanged("PrintCount");
            }
        }

        public ICommand BtnPrintCommand { get; set; }
        public ICommand BtnPortConnectCommand { get; set; }
        #endregion

        #region ExcelDataRead
        private string ReadExcelData(string filePath, string targetData) {
            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // 첫 번째 워크시트 선택

                int rows = worksheet.Dimension.Rows; // 세로 줄의 총 개수
                int colunm = worksheet.Dimension.Columns;

                for (int row = 1; row <= rows; row++) {
                    string cellValue = worksheet.Cells[row, 1].Value?.ToString(); // A열 값 읽기 [ 품번 , ProductID ]

                    /// A열 값 기준으로 순환중에 들어오는 targetData를 만나게 되면 C,D열 값을 반환
                    /// 두 열값을 각각 받아서 , 다시한번 조건을 걸어서 , 각각 반환타입이 다르게 해야함.
                    /// 그게아니면 라벨타입과 박스카운트를 각각 받아서 "/" 스플릿으로 진행해야함.
                    if (cellValue == targetData) {
                        string LabelType = worksheet.Cells[row, 3].Value?.ToString(); // C열 값 읽기 [ Label Type ]
                        
                        string colunmStr = worksheet.Cells[row, 4].Value?.ToString(); // D열 값 읽기 [ 가로 박스 개수 ]
                        NumberOfColumns = Int32.Parse(colunmStr);

                        string rowStr = worksheet.Cells[row, 5].Value?.ToString(); // E열 값 읽기 [ 새로 박스 개수 ]
                        NumberOfRows = Int32.Parse(rowStr);

                        ModelSerial = worksheet.Cells[row, 6].Value?.ToString(); // ModelSerial

                        return LabelType;
                    }
                }
            }

            return null; // 해당 데이터가 없는 경우 null 반환
        }
        #endregion


        private void BtnEvent() {
            BtnPortConnectCommand = new Command(BtnPortConnect_Click, CanExCute);
            BtnPrintCommand = new Command(BtnPrint_Click, CanExCute);
        }

        private bool CanExCute(object obj) {
            return true;
        }

        private void BtnPrint_Click(object obj) {
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

            driver = new ChromeDriver(driverService, options);

            driver.Navigate().GoToUrl(webUri);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // 아이디 입력
            SendTextInput(driver, "txtUserID", id);
            Thread.Sleep(200);
            // 비밀번호 입력
            SendTextInput(driver, "txtPassword", ps);
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
            switch (LabelType) {
                case "H_LARGE":
                    // 라벨 선택 (현대 대 - 1330*2800)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // 라벨 선택 (현대 중 - 1580*1400)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // 라벨 선택 (현대 소 - 1345*1430)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // 라벨 선택 (기아 대 - 1100*2900)
                    ClickByXPath(driver, BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // 라벨 선택 (기아 중 - 1600*1400)
                    ClickByXPath(driver, BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // 고객 품번 입력
            SendTextInput(driver, MODEL_PRODUCT_ID, Product_ID);

            Thread.Sleep(2000);
            // 포장 수량 입력
            SendTextInput(driver, PACKAGED_QUANTITY, ScanCount.ToString());
            Thread.Sleep(1000);

            // 발행 수량 입력
            SendTextInput(driver, PRINT_QUANTITY, PrintCount);


            // 마지막 쿼리 데이터 확인 완료 
            Thread.Sleep(1000);
            ClickByXPath(driver, "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]");
        }

        private void LetsGoPrint() {
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

            driver = new ChromeDriver(driverService, options);

            driver.Navigate().GoToUrl(webUri);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // 아이디 입력
            SendTextInput(driver, "txtUserID", id);
            Thread.Sleep(200);
            // 비밀번호 입력
            SendTextInput(driver, "txtPassword", ps);
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
            switch (LabelType) {
                case "H_LARGE":
                    // 라벨 선택 (현대 대 - 1330*2800)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // 라벨 선택 (현대 중 - 1580*1400)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // 라벨 선택 (현대 소 - 1345*1430)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // 라벨 선택 (기아 대 - 1100*2900)
                    ClickByXPath(driver, BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // 라벨 선택 (기아 중 - 1600*1400)
                    ClickByXPath(driver, BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // 고객 품번 입력
            SendTextInput(driver, MODEL_PRODUCT_ID, Product_ID);

            Thread.Sleep(2000);
            // 포장 수량 입력
            SendTextInput(driver, PACKAGED_QUANTITY, ScanCount.ToString());
            Thread.Sleep(1000);

            // 발행 수량 입력
            SendTextInput(driver, PRINT_QUANTITY, PrintCount);


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

                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                serialPort1.Open();
                ResultConnect = "포트 연결";
            } catch (UnauthorizedAccessException ex) {
                ResultConnect = "액세스 거부: " + ex.Message;
                Console.WriteLine("액세스 거부: " + ex.Message);
                // 포트 액세스 거부 예외 처리
                // 포트를 닫고 다시 열어보세요.
                serialPort1?.Close();
                serialPort1?.Dispose();
                OpenSerialPort(); // 재귀적으로 메서드 호출
            } catch (Exception ex) {
                ResultConnect = "연결 오류: " + ex.Message;
                Console.WriteLine("연결 오류: " + ex.Message);
            }
        }

        private void BtnPortConnect_Click(object obj) {
            serialPort1 = new SerialPort();
            if (serialPort1 != null && serialPort1.IsOpen) {  // 이미 포트가 열려 있는 경우
                serialPort1.Close();  // 포트 닫기
                serialPort1.Dispose();
            }
            serialPort1.PortName = "COM3";  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
            serialPort1.BaudRate = 9600;  //보레이트 변경이 필요하면 숫자 변경하기
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다

            try {
                serialPort1.Open();  // 시리얼포트 열기
                ResultConnect = "포트 연결";
            } catch (Exception ex) {
                ResultConnect = "연결 오류 : " + ex.Message;
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)  //수신 이벤트가 발생하면 이 부분이 실행된다.
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            string[] indataModel = indata.Split('-');
            string resultData = indataModel[0];

            /// 23 06 29 이곳에서 분기를 쳐야할거같다 . 99~이면 ModelText에 resultData를 넣어주고 ,
            /// startwith가 T~이면 해당 모델의 , 제품이 맞는지 검증후, Count를 증가시켜서 라벨출력까지 이어져야함.
            dispatcher.Invoke(() => {
                if (resultData.StartsWith("T")) 
                {
                    ModelSerial = resultData;
                    ScanCount++;
                    if (ScanCount > 0 && ScanCount.ToString() == BoxSize) {
                        LetsGoPrint();
                    }
                } 
                else 
                {
                    Product_ID = resultData;
                    LabelType = ReadExcelData(PATH, Product_ID);
                }

                GridData = new ObservableCollection<GridItem>();
                for (int i = 0; i < NumberOfColumns * NumberOfRows; i++) {
                    GridItem item = new GridItem() {
                        Index = i + 1,
                        Value = 0,
                    };
                    
                    if (i <= ScanCount - 1) {
                        item.Background = Brushes.Green;
                    }
                    GridData.Add(item);

                }
                BoxSize = GridData.Count.ToString();
            });
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
                    PrintProgress = "출력 완료 !";
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