using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using OpenQA.Selenium.Chrome;
using System;
using System.IO.Ports;
using OfficeOpenXml;
using Command = Leak_UI.Utiles.Command;
using System.Collections.ObjectModel;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;
using Leak_UI.Model;

namespace Leak_UI.ViewModel
{
    public class MainProgramViewModel : ViewModelBase
    {
        private SerialPort serialPort1;
        private ChromeDriverService driverService;
        private ChromeOptions options;
        private ChromeDriver driver;
        private IDispatcher dispatcher;

        private MainModel model = null;
        public MainModel Model {
            get { return model; }
            set { model = value; RaisePropertyChanged("Model"); }
        }

        public MainProgramViewModel(IDispatcher dispatcher) {
            model = new MainModel();
            this.dispatcher = dispatcher;
            OpenSerialPort();
            BtnEvent();
            //WinBtnEvent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }
        private void BtnEvent() {
            Model.BtnPortConnectCommand = new Command(BtnPortConnect_Click, CanExCute);
            Model.BtnPrintCommand = new Command(BtnPrint_Click, CanExCute);
        }
        private bool CanExCute(object obj) {
            return true;
        }
        #region 크롤링
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

            Model.Login(driver);

            Model.SelectLabel(driver);

            Model.InputProductInfo(driver);

            Model.VerifyPrintData(driver);

            GridData.Clear();
            Model = new MainModel();
        }
        private void AutoPrint() {
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

            Model.Login(driver);

            Model.SelectLabel(driver);

            Model.InputProductInfo(driver);

            Model.VerifyPrintData(driver);
        }
        #endregion

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
        }

        #region GridViewStyle
        private ObservableCollection<GridItem> _gridData = new ObservableCollection<GridItem>();
        public ObservableCollection<GridItem> GridData {
            get { return _gridData; }
            set {
                _gridData = value;
                RaisePropertyChanged(nameof(GridData));
            }
        }

        #endregion
        #region
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            string[] indataModel = indata.Split('-');
            string resultData = indataModel[0];

            dispatcher.Invoke(() => {
                try {
                    // 작업지시서 스캔 시 , 메인Grid를 생성한다.
                    if (resultData.StartsWith("9")) {
                        OnCreate_BoxGrid(resultData);
                        // 모델인식,작업지시서 매칭여부, 해당모델의 매칭데이터 여부
                    } else if (resultData.StartsWith("T")) {
                        OnBinding_BoxGrid(resultData);
                        // 매칭아이템의 매칭 여부 체크
                    } else if (resultData.StartsWith("M")) {
                        OnBinding_MatchGrid(resultData);
                    }

                    //AddGridItems_TEST();
                    Model.BoxSize = GridData.Count.ToString();
                } catch (Exception) {
                    MessageBox.Show("작업지시서를 스캔해 주세요");
                    return;
                }

            });
        }
        // 최초 그리드 생성, 엑셀 데이터 리딩
        private void OnCreate_BoxGrid(string data) {
            // 데이터를 읽어와서 GridData에 추가 또는 수정하는 로직
            // 예시: GridData.Add(new GridItem(data));
            Model.ScanCount = 0;
            GridData.Clear();
            Model.Product_ID = data;
            Model.ReadExcelData();

            for (int i = 0; i < Model.NumberOfColumns * Model.NumberOfRows; i++) {
                GridItem gridItem = new GridItem {
                    Index = i + 1,
                    ModelSerial = "",
                    Background = Brushes.Black,
                    GridRowSpan = 4 - Model.MatchCount
                };

                gridItem.MatchItems = new ObservableCollection<MatchItem>();
                for (int j = 0; j < Model.MatchCount; j++) {
                    MatchItem matchItem = new MatchItem {
                        MatchDataSerial = "",
                        MatchDataBackground = Brushes.Black
                    };
                    gridItem.MatchItems.Add(matchItem);
                }

                GridData.Add(gridItem);
            }
        }

        // 모델과 작업지시서 데이터 바인딩
        private void OnBinding_BoxGrid(string data) {
            // 데이터를 처리하고 GridData를 수정하는 로직
            // 예시: GridData[0].TestMatch[0].MatchDataSerial = "TEST성공";
            if (data == Model.ModelSerial) {
                if (GridData[Model.ScanCount].ModelSerial == "") {
                    GridData[Model.ScanCount].ModelSerial = "제품 : " + data;
                    GridData[Model.ScanCount].Background = Brushes.Green;

                    if (Model.MatchScanCount == Model.MatchCount) {
                        Model.ScanCount++;
                    }

                    if (Model.MatchCount < 1) {

                    }

                    if (Model.ScanCount > 0 && Model.ScanCount.ToString() == Model.BoxSize) {
                        AutoPrint();
                        GridData.Clear();
                        Model = new MainModel();
                    }
                } else {
                    MessageBox.Show("매칭을 진행해 주세요");
                }
            } else {
                MessageBox.Show("일치하지 않는 시리얼 번호입니다\n" + (Model.ScanCount + 1) + "번 위치 모델을 확인하세요.");
            }
        }

        // 매칭아이템 바인딩
        private void OnBinding_MatchGrid(string data) {
            // 데이터를 처리하고 GridData를 수정하는 로직
            // 예시: GridData[0].TestMatch[0].MatchDataSerial = "TEST성공";
            if (data == Model.MmatchItems[Model.MatchScanCount] && GridData[Model.ScanCount].ModelSerial != "") { // M 매칭아이템이 들어왔을때
                GridData[Model.ScanCount].MatchItems[Model.MatchScanCount].MatchDataSerial = "매칭 : " + data;
                GridData[Model.ScanCount].MatchItems[Model.MatchScanCount].MatchDataBackground = Brushes.Green;
                Model.MatchScanCount++;

                if (Model.MatchScanCount == Model.MatchCount) {
                    Model.ScanCount++;
                }
            } else {
                MessageBox.Show("일치하지 않는 시리얼 번호입니다\n" + (Model.MatchScanCount) + "번 위치 모델을 확인하세요.");
            }

        }

        private void AddGridItems_TEST() {
            // GridData에 대한 추가 작업
        }
        #endregion

    }
}
