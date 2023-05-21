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

        // 품번 입력
        private string modelText = "1";
        public string ModelText {
            get { return modelText; }
            set {
                modelText = value;
                RaisePropertyChanged("ModelText");
            }
        }

        // 포장 수량
        private string packCount = "1";
        public string PackCount {
            get { return packCount; }
            set {
                packCount = value;
                RaisePropertyChanged("PackCount");
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

            // 아이디 입력
            SendTextInput(driver, "txtUserID", id);

            // 비밀번호 입력
            SendTextInput(driver, "txtPassword", ps);

            // 로그인 클릭
            ClickByBtn(driver, "btnLogin");

            // 상단탭 부품식별표 클릭
            ClickByBtn(driver, "lblCategory_MOTOR_LABEL");

            // 라벨 선택 (K_Big)
            ClickByXPath(driver, "//*[@id='MOTOR_LABEL']/ul/li[4]/div");

            // 고객 품번 입력
            SendTextInput(driver, "ContentPlaceHolder1_txtProductID", ModelText);

            
            Thread.Sleep(2000);

            // 포장 수량 입력
            SendTextInput(driver, "ContentPlaceHolder1_txtPackQty", PackCount);

            // 포장 수량 입력
            SendTextInput(driver, "ContentPlaceHolder1_txtPrintQty", PrintCount);

        }

        private void BtnPortConnect_Click(object obj) {
            if (!serialPort1.IsOpen)  //시리얼포트가 열려 있지 않으면
            {
                serialPort1.PortName = "COM4";  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                serialPort1.BaudRate = 9600;  //보레이트 변경이 필요하면 숫자 변경하기
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다

                serialPort1.Open();  //시리얼포트 열기
            } else {
                MessageBox.Show("serialPort1.PortName = >" + serialPort1.PortName);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)  //수신 이벤트가 발생하면 이 부분이 실행된다.
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

        // 요소 클릭 (ID 기반)
        void ClickByBtn(IWebDriver driver, string elementId) {
            var element = driver.FindElement(By.Id(elementId));
            element.Click();
        }

        // 요소 클릭 (XPath 기반)
        void ClickByXPath(IWebDriver driver, string xpath) {
            var element = driver.FindElement(By.XPath(xpath));
            element.Click();
        }


    }
}