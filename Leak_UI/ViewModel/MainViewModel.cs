using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using OpenQA.Selenium.Chrome;
using System;
using System.IO.Ports;
using System.Windows.Input;
using OfficeOpenXml;
using Command = Leak_UI.Utiles.Command;
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
        private MainModel model = null;
        public MainModel Model {
            get { return model; }
            set { model = value; RaisePropertyChanged("Model"); }
        }
        private SerialPort serialPort1;
        private ChromeDriverService driverService;
        private ChromeOptions options;
        private ChromeDriver driver;
        private IDispatcher dispatcher;

        public MainViewModel(IDispatcher dispatcher) {
            model = new MainModel();
            this.dispatcher = dispatcher;
            OpenSerialPort();
            BtnEvent();
            WinBtnEvent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        private void BtnEvent() {
            Model.BtnPortConnectCommand = new Command(BtnPortConnect_Click, CanExCute);
            Model.BtnPrintCommand = new Command(BtnPrint_Click, CanExCute);
        }
        private bool CanExCute(object obj) {
            return true;
        }
        #region ũ�Ѹ�
        private void BtnPrint_Click(object obj) {
            Model.PrintProgress = "��� ����";
            driverService = ChromeDriverService.CreateDefaultService();

            // ũ�Ѹ� ����̹� CMDâ Hide
            driverService.HideCommandPromptWindow = true;

            options = new ChromeOptions();

            // ũ�Ѹ� GPU ����ȭ Off
            options.AddArgument("disable-gpu");

            // ũ�Ѹ� �� View Background ó��
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
            Model.PrintProgress = "��� ����";
            driverService = ChromeDriverService.CreateDefaultService();

            // ũ�Ѹ� ����̹� CMDâ Hide
            driverService.HideCommandPromptWindow = true;

            options = new ChromeOptions();

            // ũ�Ѹ� GPU ����ȭ Off
            options.AddArgument("disable-gpu");

            // ũ�Ѹ� �� View Background ó��
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
                Model.ResultConnect = "��Ʈ ����";
            } catch (UnauthorizedAccessException ex) {
                Model.ResultConnect = "�׼��� �ź�: " + ex.Message;
                Console.WriteLine("�׼��� �ź�: " + ex.Message);
                // ��Ʈ �׼��� �ź� ���� ó��
                // ��Ʈ�� �ݰ� �ٽ� �������.
                serialPort1?.Close();
                serialPort1?.Dispose();
                OpenSerialPort(); // ��������� �޼��� ȣ��
            } catch (Exception ex) {
                Model.ResultConnect = "���� ����: " + ex.Message;
                Console.WriteLine("���� ����: " + ex.Message);
            }
        }

        private void BtnPortConnect_Click(object obj) {
            if (serialPort1 != null && serialPort1.IsOpen) {  // �̹� ��Ʈ�� ���� �ִ� ���
                serialPort1.Close();  // ��Ʈ �ݱ�
                serialPort1.Dispose();
                Model.ResultConnect = "���� ����";
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

            dispatcher.Invoke(() =>
            {
                try {
                    // �۾����ü� ��ĵ �� , ����Grid�� �����Ѵ�.
                    if (resultData.StartsWith("9")) {
                        OnCreate_BoxGrid(resultData);
                        // ���ν�,�۾����ü� ��Ī����, �ش���� ��Ī������ ����
                    } else if (resultData.StartsWith("T")) {
                        OnBinding_BoxGrid(resultData);
                        // ��Ī�������� ��Ī ���� üũ
                    } else if (resultData.StartsWith("M")) {
                        OnBinding_MatchGrid(resultData);
                    }

                    //AddGridItems_TEST();
                    Model.BoxSize = GridData.Count.ToString();
                } catch (Exception) {
                    MessageBox.Show("�۾����ü��� ��ĵ�� �ּ���");
                    return;
                }
                
            });
        }
        // ���� �׸��� ����, ���� ������ ����
        private void OnCreate_BoxGrid(string data) {
            // �����͸� �о�ͼ� GridData�� �߰� �Ǵ� �����ϴ� ����
            // ����: GridData.Add(new GridItem(data));
            Model.ScanCount = 0;
            GridData.Clear();
            Model.Product_ID = data;
            Model.ReadExcelData();

            for (int i = 0; i < Model.NumberOfColumns * Model.NumberOfRows; i++) {
                GridItem gridItem = new GridItem {
                    Index = i + 1,
                    ModelSerial = "",
                    Background = Brushes.White,
                    GridRowSpan = 4 - Model.MatchCount
                };

                gridItem.MatchItems = new ObservableCollection<MatchItem>();
                for (int j = 0; j < Model.MatchCount; j++) {
                    MatchItem matchItem = new MatchItem {
                        MatchDataSerial = "",
                        MatchDataBackground = Brushes.White
                    };
                    gridItem.MatchItems.Add(matchItem);
                }

                GridData.Add(gridItem);
            }
        }
        
        // �𵨰� �۾����ü� ������ ���ε�
        private void OnBinding_BoxGrid(string data) {
            // �����͸� ó���ϰ� GridData�� �����ϴ� ����
            // ����: GridData[0].TestMatch[0].MatchDataSerial = "TEST����";
            if (data == Model.ModelSerial) {
                if (GridData[Model.ScanCount].ModelSerial == "") {
                    GridData[Model.ScanCount].ModelSerial = data;
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
                    MessageBox.Show("��Ī�� ������ �ּ���");
                }
            } else {
                MessageBox.Show("��ġ���� �ʴ� �ø��� ��ȣ�Դϴ�\n" + (Model.ScanCount + 1) + "�� ��ġ ���� Ȯ���ϼ���.");
            }
        }

        // ��Ī������ ���ε�
        private void OnBinding_MatchGrid(string data) {
            // �����͸� ó���ϰ� GridData�� �����ϴ� ����
            // ����: GridData[0].TestMatch[0].MatchDataSerial = "TEST����";
            if (data == Model.MmatchItems[Model.MatchScanCount] && GridData[Model.ScanCount].ModelSerial != "") { // M ��Ī�������� ��������
                GridData[Model.ScanCount].MatchItems[Model.MatchScanCount].MatchDataSerial = data;
                GridData[Model.ScanCount].MatchItems[Model.MatchScanCount].MatchDataBackground = Brushes.Green;
                Model.MatchScanCount++;

                if (Model.MatchScanCount == Model.MatchCount) {
                    Model.ScanCount++;
                }
            } else {
                MessageBox.Show("��ġ���� �ʴ� �ø��� ��ȣ�Դϴ�\n" + (Model.MatchScanCount) + "�� ��ġ ���� Ȯ���ϼ���.");
            }

        }

        private void AddGridItems_TEST() {
            // GridData�� ���� �߰� �۾�
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