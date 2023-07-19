using Leak_UI.Utiles;
using System;
using System.IO.Ports;
using OfficeOpenXml;
using Command = Leak_UI.Utiles.Command;
using System.Collections.ObjectModel;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;
using Leak_UI.Model;
using System.Windows.Input;
using System.Diagnostics;
using System.Reflection;

namespace Leak_UI.ViewModel
{
    public class Main_MatchingViewModel : Main_Crowling
    {

        // 웹크롤링
        //private Main_Crowling _webCrowling = new Main_Crowling();
        //public Main_Crowling WebCrowling {
        //    get { return _webCrowling; }
        //    set { _webCrowling = value;
        //        RaisePropertyChanged(nameof(WebCrowling)); }
        //}
        
        public Main_MatchingViewModel(IDispatcher dispatcher) {
            //_webCrowling = new Main_Crowling();
            this.dispatcher = dispatcher;
            OpenSerialPort();
            BtnEvent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
        public ICommand BtnPrintCommand { get; set; }
        public ICommand BtnPortConnectCommand { get; set; }
        private void BtnEvent() {
            BtnPortConnectCommand = new Command(BtnPortConnect_Click, CanExCute);
            BtnPrintCommand = new Command(BtnPrint_Click, CanExCute);
        }
        private bool CanExCute(object obj) {
            return true;
        }
        #region 크롤링
        private void BtnPrint_Click(object obj) {
            GetPrint();
        }
        #endregion
        
        private void OpenSerialPort() {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
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
                ResultConnect = "포트 연결";
            } catch (UnauthorizedAccessException ex) {
                ResultConnect = "액세스 거부: " + ex.Message;
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + ex);
                // 포트 액세스 거부 예외 처리
                // 포트를 닫고 다시 열어보세요.
                serialPort1?.Close();
                serialPort1?.Dispose();
                OpenSerialPort(); // 재귀적으로 메서드 호출
            } catch (Exception ex) {
                ResultConnect = "연결 오류: " + ex.Message;
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + ex);
            }
        }

        private void BtnPortConnect_Click(object obj) {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                if (serialPort1 != null && serialPort1.IsOpen) {  // 이미 포트가 열려 있는 경우
                    serialPort1.Close();  // 포트 닫기
                    serialPort1.Dispose();
                    ResultConnect = "연결 종료";
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }
        #region GridViewStyle
        private ObservableCollection<Main_GridItem> _gridData = new ObservableCollection<Main_GridItem>();
        public ObservableCollection<Main_GridItem> GridData {
            get { return _gridData; }
            set {
                _gridData = value;
                RaisePropertyChanged(nameof(GridData));
            }
        }
        #endregion
        #region
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
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
                        if (IsDuplicate(indata) == true) {
                            OnBinding_BoxGrid(resultData);
                        } else {
                            MessageBox.Show("중복된 바코드 입니다");
                        }
                        // 매칭 데이터 바인딩
                    } else if (resultData.StartsWith("M")) {
                        if (IsDuplicate(indata) == true) {
                            OnBinding_MatchGrid(resultData);
                        } else {
                            MessageBox.Show("중복된 바코드 입니다");
                        }
                    }
                    //AddGridItems_TEST();
                    BoxSize = GridData.Count.ToString();
                } catch (Exception) {
                    MessageBox.Show("작업지시서를 스캔해 주세요");
                    return;
                }
            });
        }




        // 최초 그리드 생성, 엑셀 데이터 리딩
        private void OnCreate_BoxGrid(string data) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                // 데이터를 읽어와서 GridData에 추가 또는 수정하는 로직
                // 예시: GridData.Add(new GridItem(data));
                
                ScanCount = 0;
                GridData.Clear();
                Product_ID = data;
                ReadExcelData(Product_ID);
                PrintSuccese = false;

                for (int i = 0; i < NumberOfColumns * NumberOfRows; i++) {
                    Main_GridItem gridItem = new Main_GridItem {
                        Index = i + 1,
                        ModelSerial = "",
                        Background = Brushes.Black,
                        GridRowSpan = 4 - MatchCount,
                        MatchGridRowSpan = MatchCount
                    };

                    gridItem.MatchItems = new ObservableCollection<Main_GridItem_MatchItem>();
                    for (int j = 0; j < MatchCount; j++) {
                        Main_GridItem_MatchItem matchItem = new Main_GridItem_MatchItem {
                            MatchDataSerial = "",
                            MatchDataBackground = Brushes.Black
                        };
                        gridItem.MatchItems.Add(matchItem);
                    }

                    GridData.Add(gridItem);
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }

        // 모델과 작업지시서 데이터 바인딩
        private void OnBinding_BoxGrid(string data) {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                // 데이터를 처리하고 GridData를 수정하는 로직
                // 예시: GridData[0].TestMatch[0].MatchDataSerial = "TEST성공";
                if (data == ModelSerial) {
                    if (GridData[ScanCount].ModelSerial == "") {
                        GridData[ScanCount].ModelSerial = "제품 : " + data;
                        GridData[ScanCount].Background = Brushes.Green;

                        if (MatchScanCount == MatchCount) {
                            ScanCount++;
                        }
                        // 매칭 카운트가 2개 이상이고 , 스캔카운트가 0이 아니고, 스캔횟수가 박스사이즈에 도달했을때
                        if (MatchCount > 1 && ScanCount > 0 && ScanCount.ToString() == BoxSize) {
                            GetPrint();
                            GridData.Clear();
                            //WebCrowling = new Main_Crowling();
                        }

                    } else {
                        MessageBox.Show("매칭을 진행해 주세요");
                    }
                } else {
                    MessageBox.Show("일치하지 않는 시리얼 번호입니다\n" + (ScanCount + 1) + "번 위치 모델을 확인하세요.");
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }

        // 매칭아이템 바인딩
        private void OnBinding_MatchGrid(string data) {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                // 데이터를 처리하고 GridData를 수정하는 로직
                // 예시: GridData[0].TestMatch[0].MatchDataSerial = "TEST성공";
                if (data == MmatchItems[MatchScanCount] && GridData[ScanCount].ModelSerial != "") { // M 매칭아이템이 들어왔을때
                    GridData[ScanCount].MatchItems[MatchScanCount].MatchDataSerial = "매칭 : " + data;
                    GridData[ScanCount].MatchItems[MatchScanCount].MatchDataBackground = Brushes.Green;
                    MatchScanCount++;

                    if (MatchScanCount == MatchCount) {
                        ScanCount++;
                    }
                    // 매칭카운트 개수와 상관없이, 마지막("M") 매칭 후 스캔 카운트가 도달했을때
                    if (ScanCount.ToString() == BoxSize) {
                        GetPrint();
                        GridData.Clear();
                        //WebCrowling = new Main_Crowling();
                    }
                } else {
                    MessageBox.Show("일치하지 않는 시리얼 번호입니다\n" + (MatchScanCount) + "번 위치 모델을 확인하세요.");
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }


        }

        private void AddGridItems_TEST() {
            // GridData에 대한 추가 작업
        }
        #endregion

    }
}
