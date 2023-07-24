using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;

namespace Leak_UI.Utiles
{
    public class SerialPortManager 
    {
        private SerialPort serialPort;
        public string ResultConnectValue { get; private set; }
        public delegate void SerialDataReceivedDelegate(object sender, SerialDataReceivedEventArgs e);

        /// <summary>
        /// PortNumber = 연결할 스캐너의 포트번호
        /// dataReceivedHandler = 기능 바인딩된 핸들러
        /// </summary>
        /// <param name="portNumber"></param>
        /// <param name="dataReceivedHandler"></param>
        public void OpenSerialPort(int portNumber, SerialDataReceivedDelegate dataReceivedHandler) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\n");
            try {
                if (serialPort != null && serialPort.IsOpen) {
                    serialPort.Close();
                    serialPort.Dispose();
                }
                serialPort = new SerialPort {
                    PortName = "COM"+portNumber.ToString(),
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None
                };

                serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceivedHandler);

                serialPort.Open();
                ResultConnectValue = "포트 연결";
            } catch (UnauthorizedAccessException ex) {
                ResultConnectValue = "액세스 거부: " + ex.Message;
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + ex);
                // 포트 액세스 거부 예외 처리
                // 포트를 닫고 다시 열어보세요.
                serialPort?.Close();
                serialPort?.Dispose();
                OpenSerialPort(portNumber, dataReceivedHandler); // 재귀적으로 메서드 호출
            } catch (Exception ex) {
                ResultConnectValue = "연결 오류: " + ex.Message;
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + ex);
            }
        }

        /// <summary>
        /// 수동 포트 연결/해제
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="portNumber"></param>
        public void CloseSerialPort(object obj,int portNumber) {
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\n");
            try {
                if (serialPort != null ) {
                    if (serialPort.IsOpen) {// 이미 포트가 열려 있는 경우
                        serialPort.Close();  // 포트 닫기
                        serialPort.Dispose();
                        ResultConnectValue = "연결 종료";
                    } else {
                        serialPort = new SerialPort {
                            PortName = "COM" + portNumber.ToString(),
                            BaudRate = 9600,
                            DataBits = 8,
                            StopBits = StopBits.One,
                            Parity = Parity.None
                        };

                        serialPort.Open();
                        ResultConnectValue = "포트 연결";
                    }
                    
                }
            } catch (Exception ex) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + ex);
                throw;
            }

        }
        /// <summary>
        /// 직렬 연결 된 시리얼포트의 정보를 가져오는 메서드
        /// </summary>
        /// <returns></returns>
        public string[] GetPortNames() {
            return SerialPort.GetPortNames();
        }

    }
}
