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
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Leak_UI.Model;
using System.Collections.Generic;
using static Leak_UI.Model.GridItem;

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
            Model.NumberOfColumns = 5; // ����
            Model.NumberOfRows = 3; // ����
        }


        

        #region ExcelDataRead
        private string ReadExcelData(string returnValue) {
            FileInfo fileInfo = new FileInfo(Model.PATH);

            using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // ù ��° ��ũ��Ʈ ����

                int rows = worksheet.Dimension.Rows; // ���� ���� �� ����
                int colunm = worksheet.Dimension.Columns;

                for (int row = 1; row <= rows; row++) {
                    string cellValue = worksheet.Cells[row, 1].Value?.ToString(); // A�� �� �б� [ ǰ�� , ProductID ]

                    if (cellValue == Model.Product_ID) {

                        returnValue = worksheet.Cells[row, 3].Value?.ToString();
                        Model.LabelType = worksheet.Cells[row, 3].Value?.ToString(); // C�� �� �б� [ Label Type ]

                        string colunmStr = worksheet.Cells[row, 4].Value?.ToString(); // D�� �� �б� [ ���� �ڽ� ���� ]
                        Model.NumberOfColumns = Int32.Parse(colunmStr);

                        string rowStr = worksheet.Cells[row, 5].Value?.ToString(); // E�� �� �б� [ ���� �ڽ� ���� ]
                        Model.NumberOfRows = Int32.Parse(rowStr);

                        Model.ModelSerial = worksheet.Cells[row, 6].Value?.ToString(); // ModelSerial

                        Model.BoxColorString = worksheet.Cells[row, 7].Value?.ToString(); // boxColor Convert To String

                        string matchNumberOfRows = worksheet.Cells[row, 11].Value?.ToString();
                        Model.MatchCount = Int32.Parse(matchNumberOfRows);

                        for (int i = 0; i < Model.MatchCount; i++) {
                            Model.MmatchItems.Add(worksheet.Cells[row, i + 8].Value?.ToString());
                        }
                    }
                }
            }

            return returnValue; // �ش� �����Ͱ� ���� ��� null ��ȯ
        }
        #endregion
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

            // ���̵� �Է�
            SendTextInput(driver, "txtUserID", Model.id);
            Thread.Sleep(200);
            // ��й�ȣ �Է�
            SendTextInput(driver, "txtPassword", Model.ps);
            Thread.Sleep(200);
            // �α��� Ŭ��
            ClickByBtn(driver, "btnLogin");
            try {
                // Web ShowDialog Accept Check
                driver.SwitchTo().Alert().Accept();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            Thread.Sleep(1000);

            // ����� ��ǰ�ĺ�ǥ Ŭ��
            ClickByBtn(driver, "lblCategory_MOTOR_LABEL");


            Thread.Sleep(1000);
            // �� ���� 
            switch (Model.LabelType) {
                case "H_LARGE":
                    // �� ���� (���� �� - 1330*2800)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // �� ���� (���� �� - 1580*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // �� ���� (���� �� - 1345*1430)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // �� ���� (��� �� - 1100*2900)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // �� ���� (��� �� - 1600*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // �� ǰ�� �Է�
            SendTextInput(driver, Model.MODEL_PRODUCT_ID, Model.Product_ID);

            Thread.Sleep(2000);
            // ���� ���� �Է�
            SendTextInput(driver, Model.PACKAGED_QUANTITY, Model.ScanCount.ToString());
            Thread.Sleep(1000);

            // ���� ���� �Է�
            SendTextInput(driver, Model.PRINT_QUANTITY, Model.PrintCount);


            // ������ ���� ������ Ȯ�� �Ϸ� 
            Thread.Sleep(1000);
            ClickByXPath(driver, "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]");
        }

        private void LetsGoPrint() {
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

            // ���̵� �Է�
            SendTextInput(driver, "txtUserID", Model.id);
            Thread.Sleep(200);
            // ��й�ȣ �Է�
            SendTextInput(driver, "txtPassword", Model.ps);
            Thread.Sleep(200);
            // �α��� Ŭ��
            ClickByBtn(driver, "btnLogin");
            try {
                // Web ShowDialog Accept Check
                driver.SwitchTo().Alert().Accept();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            Thread.Sleep(1000);

            // ����� ��ǰ�ĺ�ǥ Ŭ��
            ClickByBtn(driver, "lblCategory_MOTOR_LABEL");

            Thread.Sleep(1000);
            // �� ���� 
            switch (Model.LabelType) {
                case "H_LARGE":
                    // �� ���� (���� �� - 1330*2800)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // �� ���� (���� �� - 1580*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // �� ���� (���� �� - 1345*1430)
                    ClickByXPath(driver, Model.BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // �� ���� (��� �� - 1100*2900)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // �� ���� (��� �� - 1600*1400)
                    ClickByXPath(driver, Model.BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // �� ǰ�� �Է�
            SendTextInput(driver, Model.MODEL_PRODUCT_ID, Model.Product_ID);

            Thread.Sleep(2000);
            // ���� ���� �Է�
            SendTextInput(driver, Model.PACKAGED_QUANTITY, Model.ScanCount.ToString());
            Thread.Sleep(1000);

            // ���� ���� �Է�
            SendTextInput(driver, Model.PRINT_QUANTITY, Model.PrintCount);

            // ������ ���� ������ Ȯ�� �Ϸ� 
            Thread.Sleep(1000);
            ClickByXPath(driver, "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]");
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

                serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived_Test);

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
        //private ObservableCollection<GridItem> GridData = new ObservableCollection<GridItem>();

        private ObservableCollection<MatchItem> _matchData = new ObservableCollection<MatchItem>();
        public ObservableCollection<MatchItem> MatchData {
            get { return _matchData; }
            set {
                _matchData = value;
                RaisePropertyChanged(nameof(MatchData));
            }
        }

        #endregion
        #region �׽�Ʈ ���� 07 11
        private void SerialPort1_DataReceived_Test(object sender, SerialDataReceivedEventArgs e) {
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
                    } else if (resultData.StartsWith("T")) {
                        OnBinding_BoxGrid(resultData);
                    } else if (resultData.StartsWith("M")) {
                        OnBinding_BoxGrid(resultData);
                    }

                    //AddGridItems_TEST();
                    Model.BoxSize = GridData.Count.ToString();
                } catch (Exception) {
                    MessageBox.Show("�۾����ü��� ��ĵ�� �ּ���");
                    return;
                }
                
            });
        }

        private void OnCreate_BoxGrid(string data) {
            // �����͸� �о�ͼ� GridData�� �߰� �Ǵ� �����ϴ� ����
            // ����: GridData.Add(new GridItem(data));
            GridData.Clear();
            Model.Product_ID = data;
            ReadExcelData(Model.LabelType);

            for (int i = 0; i < Model.NumberOfColumns * Model.NumberOfRows; i++) {
                GridItem gridItem = new GridItem {
                    Index = i + 1,
                    ModelSerial = "",
                    Background = Brushes.White
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

        private void OnBinding_BoxGrid(string data) {
            // �����͸� ó���ϰ� GridData�� �����ϴ� ����
            // ����: GridData[0].TestMatch[0].MatchDataSerial = "TEST����";
            if (data == Model.ModelSerial ) { // T ModelData�� ��������
                GridData[Model.ScanCount].ModelSerial = data;
                GridData[Model.ScanCount].Background = Brushes.Green;
                
                if (Model.MatchScanCount == Model.MatchCount) {
                    Model.ScanCount++;
                }
                if (Model.ScanCount > 0 && Model.ScanCount.ToString() == Model.BoxSize) {
                    LetsGoPrint();
                }
            } 
            else if (data == Model.MmatchItems[Model.MatchScanCount] && GridData[Model.ScanCount].ModelSerial != "") { // M ��Ī�������� ��������
                GridData[Model.ScanCount].MatchItems[Model.MatchScanCount].MatchDataSerial = data;
                GridData[Model.ScanCount].MatchItems[Model.MatchScanCount].MatchDataBackground = Brushes.Green;
                Model.MatchScanCount++;

                if (Model.MatchScanCount == Model.MatchCount) {
                    Model.ScanCount++;
                }
            } 
            
            else {
                MessageBox.Show("��ġ���� �ʴ� �ø��� ��ȣ�Դϴ�\n" + (Model.ScanCount + 1) + "�� ��ġ ���� Ȯ���ϼ���.");
            }

        }

        private void AddGridItems_TEST() {
            // GridData�� ���� �߰� �۾�
        }
        #endregion
        
        #region Web I/O Method
        // Input Text
        void SendTextInput(IWebDriver driver, string elementId, string text) {
            var element = driver.FindElement(By.Id(elementId));
            element.SendKeys(text);

            // �� ��ȣ�� ���������� �������� ���ͷ� ��ȸ ����
            if (text.StartsWith("99")) {
                element.SendKeys(Keys.Enter);
            }

            // ���� ���� �Է½� ���ͷ� ����
            if (elementId.Equals(Model.PACKAGED_QUANTITY)) {
                element.SendKeys(Keys.Enter);
                try {
                    driver.SwitchTo().Alert().Accept();
                } catch (Exception) {

                }
            }

            // ���� ���� �Է� �� ���ͷ� ��� ����
            if (elementId.Equals(Model.PRINT_QUANTITY)) {
                element.SendKeys(Keys.Enter);
                try {
                    driver.SwitchTo().Alert().Accept();
                } catch (Exception) {

                }
            }
        }

        // ��� Ŭ�� (ID ���)
        void ClickByBtn(IWebDriver driver, string elementId) {
            var element = driver.FindElement(By.Id(elementId));
            element.Click();

        }

        // ��� Ŭ�� (XPath ���)
        void ClickByXPath(IWebDriver driver, string xpath) {
            var element = driver.FindElement(By.XPath(xpath));
            element.Click();

            if (xpath.Equals("//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]")) {
                string eleDate = element.Text; // �� ���� ������ �ð� string

                // string => dateTime casting
                CultureInfo culture = new CultureInfo("en-US");
                DateTime webTimeData = Convert.ToDateTime(eleDate, culture);

                // now dateTime
                DateTime timeNow = DateTime.Now;
                timeNow.ToString("yyyy//MM//dd HH:mm:ss");

                // ���� �ð� - ���� �Ϸ� �ð� = �ð� ����
                TimeSpan result = timeNow - webTimeData;
                // �ð�(��) ����
                int resultTest = result.Seconds;

                // 
                if (resultTest < 5) {
                    Model.PrintProgress = "��� �Ϸ� !";
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