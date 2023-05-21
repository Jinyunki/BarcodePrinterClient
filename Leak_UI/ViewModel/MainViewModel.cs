using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Leak_UI.ViewModel {
    public class MainViewModel : ViewModelBase
    {
        string webUri = "https://jc-label.mobis.co.kr/";
        string id = "SS1F";
        string ps = "Sekonix12@@";
        //string scanModel = "9999999";

        ChromeDriverService driverService;
        ChromeOptions options;
        ChromeDriver driver;
        SerialPort serialPort1 = new SerialPort("COM4");
        IDispatcher dispatcher ;

        // ǰ�� �Է�
        private string modelText = "1";
        public string ModelText {
            get { return modelText; }
            set {
                modelText = value;
                RaisePropertyChanged("ModelText");
            }
        }

        // ���� ����
        private string packCount = "1";
        public string PackCount {
            get { return packCount; }
            set {
                packCount = value;
                RaisePropertyChanged("PackCount");
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

        public MainViewModel(IDispatcher dispatcher) {
            this.dispatcher = dispatcher;
            initBtn();
        }

        private void initBtn() {
            BtnPortConnectCommand = new Utiles.Command(BtnPortConnect_Click, CanExCute);
            BtnPrintCommand = new Utiles.Command(BtnPrint_Click, CanExCute);
        }

        private bool CanExCute(object obj) {
            return true;
        }

        private void BtnPrint_Click(object obj) {
            driverService = ChromeDriverService.CreateDefaultService();

            options = new ChromeOptions();
            options.AddArgument("disable-gpu");

            driver = new ChromeDriver(driverService, options);

            driver.Navigate().GoToUrl(webUri);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            // ���̵� �Է�
            SendTextInput(driver, "txtUserID", id);

            // ��й�ȣ �Է�
            SendTextInput(driver, "txtPassword", ps);

            // �α��� Ŭ��
            ClickByBtn(driver, "btnLogin");

            // ����� ��ǰ�ĺ�ǥ Ŭ��
            ClickByBtn(driver, "lblCategory_MOTOR_LABEL");

            // �� ���� (K_Big)
            ClickByXPath(driver, "//*[@id='MOTOR_LABEL']/ul/li[4]/div");

            // �� ǰ�� �Է�
            SendTextInput(driver, "ContentPlaceHolder1_txtProductID", ModelText);

            
            Thread.Sleep(2000);

            // ���� ���� �Է�
            SendTextInput(driver, "ContentPlaceHolder1_txtPackQty", PackCount);

            // ���� ���� �Է�
            SendTextInput(driver, "ContentPlaceHolder1_txtPrintQty", PrintCount);

        }

        private void BtnPortConnect_Click(object obj) {
            if (!serialPort1.IsOpen)  //�ø�����Ʈ�� ���� ���� ������
            {
                serialPort1.PortName = "COM4";  //�޺��ڽ��� ���õ� COM��Ʈ���� �ø�����Ʈ������ ����
                serialPort1.BaudRate = 9600;  //������Ʈ ������ �ʿ��ϸ� ���� �����ϱ�
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //�̰��� �� �ʿ��ϴ�

                serialPort1.Open();  //�ø�����Ʈ ����
            } else {
                MessageBox.Show("serialPort1.PortName = >" + serialPort1.PortName);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)  //���� �̺�Ʈ�� �߻��ϸ� �� �κ��� ����ȴ�.
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            dispatcher.Invoke(() => {
                ModelText = indata;
            });
        }



        // Input Text
        void SendTextInput(IWebDriver driver, string elementId, string text) {
            var element = driver.FindElement(By.Id(elementId));
            element.SendKeys(text);
            if (text.StartsWith("99")) {
                element.SendKeys(Keys.Enter);
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
        }


    }
}