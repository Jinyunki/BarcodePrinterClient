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
            Model.NumberOfColumns = 5; // ����
            Model.NumberOfRows = 3; // ����
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
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // ù ��° ��ũ��Ʈ ����

                int rows = worksheet.Dimension.Rows; // ���� ���� �� ����
                int colunm = worksheet.Dimension.Columns;

                for (int row = 1; row <= rows; row++) {
                    string cellValue = worksheet.Cells[row, 1].Value?.ToString(); // A�� �� �б� [ ǰ�� , ProductID ]

                    if (cellValue == Model.Product_ID) {
                        Model.LabelType = worksheet.Cells[row, 3].Value?.ToString(); // C�� �� �б� [ Label Type ]
                        
                        string colunmStr = worksheet.Cells[row, 4].Value?.ToString(); // D�� �� �б� [ ���� �ڽ� ���� ]
                        Model.NumberOfColumns = Int32.Parse(colunmStr);

                        string rowStr = worksheet.Cells[row, 5].Value?.ToString(); // E�� �� �б� [ ���� �ڽ� ���� ]
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

            return null; // �ش� �����Ͱ� ���� ��� null ��ȯ
        }
        #endregion
        #region MatchSystem
        private bool MatchChecking(string matchData, string matchResult) {
            return matchData == matchResult; //  ������� true �ƴѰ�� false
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

            /*
            serialPort1.PortName = "COM3";  //�޺��ڽ��� ���õ� COM��Ʈ���� �ø�����Ʈ������ ����
            serialPort1.BaudRate = 9600;  //������Ʈ ������ �ʿ��ϸ� ���� �����ϱ�
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived); //�̰��� �� �ʿ��ϴ�

            try {
                serialPort1.Open();  // �ø�����Ʈ ����
                Model.ResultConnect = "��Ʈ ����";
            } catch (Exception ex) {
                Model.ResultConnect = "���� ���� : " + ex.Message;
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
                // ��Ī�Ұ��� ������
                if (Model.MatchItem1 == null && Model.MatchItem2 == null && Model.MatchItem3 == null) {
                    Model.ScanCount++;
                }

                if (Model.ScanCount > 0 && Model.ScanCount.ToString() == Model.BoxSize) {
                    LetsGoPrint();
                }
            } else {
                MessageBox.Show("��ġ���� �ʴ� �ø��� ��ȣ�Դϴ�\n" + (Model.ScanCount + 1) + "�� ���� Ȯ���ϼ���.");
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