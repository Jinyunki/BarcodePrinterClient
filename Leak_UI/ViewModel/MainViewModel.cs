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
            NumberOfColumns = 5; // ����
            NumberOfRows = 3; // ����
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
        private string printProgress = "��� ���";
        public string PrintProgress {
            get { return printProgress; }
            set {
                printProgress = value;
                RaisePropertyChanged("PrintProgress");
            }
        }

        // �ڽ� �� Ÿ��
        private string labelType = "LABEL TYPE";
        public string LabelType {
            get { return labelType; }
            set {
                labelType = value;
                RaisePropertyChanged("BoxType");
            }
        }

        // ��Ʈ ���� ����
        private string resultConnect = "��Ʈ ������ ���� �ּ���";
        public string ResultConnect {
            get { return resultConnect; }
            set {
                resultConnect = value;
                RaisePropertyChanged("ResultConnect");
            }
        }

        // ǰ�� �Է�
        private string product_ID = "�۾����ü��� ��ĵ �ϼ���";
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

        // ���� ����
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
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // ù ��° ��ũ��Ʈ ����

                int rows = worksheet.Dimension.Rows; // ���� ���� �� ����
                int colunm = worksheet.Dimension.Columns;

                for (int row = 1; row <= rows; row++) {
                    string cellValue = worksheet.Cells[row, 1].Value?.ToString(); // A�� �� �б� [ ǰ�� , ProductID ]

                    /// A�� �� �������� ��ȯ�߿� ������ targetData�� ������ �Ǹ� C,D�� ���� ��ȯ
                    /// �� ������ ���� �޾Ƽ� , �ٽ��ѹ� ������ �ɾ , ���� ��ȯŸ���� �ٸ��� �ؾ���.
                    /// �װԾƴϸ� ��Ÿ�԰� �ڽ�ī��Ʈ�� ���� �޾Ƽ� "/" ���ø����� �����ؾ���.
                    if (cellValue == targetData) {
                        string LabelType = worksheet.Cells[row, 3].Value?.ToString(); // C�� �� �б� [ Label Type ]
                        
                        string colunmStr = worksheet.Cells[row, 4].Value?.ToString(); // D�� �� �б� [ ���� �ڽ� ���� ]
                        NumberOfColumns = Int32.Parse(colunmStr);

                        string rowStr = worksheet.Cells[row, 5].Value?.ToString(); // E�� �� �б� [ ���� �ڽ� ���� ]
                        NumberOfRows = Int32.Parse(rowStr);

                        ModelSerial = worksheet.Cells[row, 6].Value?.ToString(); // ModelSerial

                        return LabelType;
                    }
                }
            }

            return null; // �ش� �����Ͱ� ���� ��� null ��ȯ
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
            PrintProgress = "��� ����";
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

            driver.Navigate().GoToUrl(webUri);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // ���̵� �Է�
            SendTextInput(driver, "txtUserID", id);
            Thread.Sleep(200);
            // ��й�ȣ �Է�
            SendTextInput(driver, "txtPassword", ps);
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
            switch (LabelType) {
                case "H_LARGE":
                    // �� ���� (���� �� - 1330*2800)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // �� ���� (���� �� - 1580*1400)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // �� ���� (���� �� - 1345*1430)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // �� ���� (��� �� - 1100*2900)
                    ClickByXPath(driver, BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // �� ���� (��� �� - 1600*1400)
                    ClickByXPath(driver, BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // �� ǰ�� �Է�
            SendTextInput(driver, MODEL_PRODUCT_ID, Product_ID);

            Thread.Sleep(2000);
            // ���� ���� �Է�
            SendTextInput(driver, PACKAGED_QUANTITY, ScanCount.ToString());
            Thread.Sleep(1000);

            // ���� ���� �Է�
            SendTextInput(driver, PRINT_QUANTITY, PrintCount);


            // ������ ���� ������ Ȯ�� �Ϸ� 
            Thread.Sleep(1000);
            ClickByXPath(driver, "//*[@id='ContentPlaceHolder1_dxGrid2_DXDataRow0']/td[1]");
        }

        private void LetsGoPrint() {
            PrintProgress = "��� ����";
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

            driver.Navigate().GoToUrl(webUri);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // ���̵� �Է�
            SendTextInput(driver, "txtUserID", id);
            Thread.Sleep(200);
            // ��й�ȣ �Է�
            SendTextInput(driver, "txtPassword", ps);
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
            switch (LabelType) {
                case "H_LARGE":
                    // �� ���� (���� �� - 1330*2800)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_LARGE);
                    break;
                case "H_MIDDLE":
                    // �� ���� (���� �� - 1580*1400)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_MIDDLE);
                    break;
                case "H_SMALL":
                    // �� ���� (���� �� - 1345*1430)
                    ClickByXPath(driver, BOX_LABEL_HYUNDAI_SMALL);
                    break;
                case "K_LARGE":
                    // �� ���� (��� �� - 1100*2900)
                    ClickByXPath(driver, BOX_LABEL_KIA_LARGE);
                    break;
                case "K_MIDDLE":
                    // �� ���� (��� �� - 1600*1400)
                    ClickByXPath(driver, BOX_LABEL_KIA_MIDDLE);
                    break;
            }


            // �� ǰ�� �Է�
            SendTextInput(driver, MODEL_PRODUCT_ID, Product_ID);

            Thread.Sleep(2000);
            // ���� ���� �Է�
            SendTextInput(driver, PACKAGED_QUANTITY, ScanCount.ToString());
            Thread.Sleep(1000);

            // ���� ���� �Է�
            SendTextInput(driver, PRINT_QUANTITY, PrintCount);


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

                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                serialPort1.Open();
                ResultConnect = "��Ʈ ����";
            } catch (UnauthorizedAccessException ex) {
                ResultConnect = "�׼��� �ź�: " + ex.Message;
                Console.WriteLine("�׼��� �ź�: " + ex.Message);
                // ��Ʈ �׼��� �ź� ���� ó��
                // ��Ʈ�� �ݰ� �ٽ� �������.
                serialPort1?.Close();
                serialPort1?.Dispose();
                OpenSerialPort(); // ��������� �޼��� ȣ��
            } catch (Exception ex) {
                ResultConnect = "���� ����: " + ex.Message;
                Console.WriteLine("���� ����: " + ex.Message);
            }
        }

        private void BtnPortConnect_Click(object obj) {
            serialPort1 = new SerialPort();
            if (serialPort1 != null && serialPort1.IsOpen) {  // �̹� ��Ʈ�� ���� �ִ� ���
                serialPort1.Close();  // ��Ʈ �ݱ�
                serialPort1.Dispose();
            }
            serialPort1.PortName = "COM3";  //�޺��ڽ��� ���õ� COM��Ʈ���� �ø�����Ʈ������ ����
            serialPort1.BaudRate = 9600;  //������Ʈ ������ �ʿ��ϸ� ���� �����ϱ�
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //�̰��� �� �ʿ��ϴ�

            try {
                serialPort1.Open();  // �ø�����Ʈ ����
                ResultConnect = "��Ʈ ����";
            } catch (Exception ex) {
                ResultConnect = "���� ���� : " + ex.Message;
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)  //���� �̺�Ʈ�� �߻��ϸ� �� �κ��� ����ȴ�.
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            string[] indataModel = indata.Split('-');
            string resultData = indataModel[0];

            /// 23 06 29 �̰����� �б⸦ �ľ��ҰŰ��� . 99~�̸� ModelText�� resultData�� �־��ְ� ,
            /// startwith�� T~�̸� �ش� ���� , ��ǰ�� �´��� ������, Count�� �������Ѽ� ����±��� �̾�������.
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

            // �� ��ȣ�� ���������� �������� ���ͷ� ��ȸ ����
            if (text.StartsWith("99")) {
                element.SendKeys(Keys.Enter);
            }

            // ���� ���� �Է½� ���ͷ� ����
            if (elementId.Equals(PACKAGED_QUANTITY)) {
                element.SendKeys(Keys.Enter);
                try {
                    driver.SwitchTo().Alert().Accept();
                } catch (Exception) {

                }
            }

            // ���� ���� �Է� �� ���ͷ� ��� ����
            if (elementId.Equals(PRINT_QUANTITY)) {
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
                    PrintProgress = "��� �Ϸ� !";
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