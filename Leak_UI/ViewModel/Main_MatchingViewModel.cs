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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using System.IO;

namespace Leak_UI.ViewModel
{
    public class Main_MatchingViewModel : ViewModelProvider
    {
        public ICommand BtnPrintCommand { get; set; }
        public ICommand BtnPortConnectCommand { get; set; }

        private string[] portNames;

        public string[] PortNames {
            get { return portNames; }
            set {
                portNames = value;
                RaisePropertyChanged("PortNames");
            }
        }


        public Main_MatchingViewModel(IDispatcher dispatcher) {
            this.dispatcher = dispatcher;
            SerialConnect();
            BtnEvent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void SerialConnect() {
            PortNames = _serialPortManager.GetPortNames();
            _serialPortManager.OpenSerialPort(3, SerialPort_DataReceived);
            ResultConnect = _serialPortManager.ResultConnectValue;
        }


        private void BtnEvent() {
            BtnPortConnectCommand = new Command(BtnPortConnect_Click);
            BtnPrintCommand = new Command(BtnPrint_Click);
        }
        
        #region 크롤링
        private void BtnPrint_Click(object obj) {
            webDriveManager.GetPrint(Product_ID,ScanCount,PrintCount,PrintSuccese);
            GridData.Clear();
        }
        #endregion

        public void BtnPortConnect_Click(object obj) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                _serialPortManager.CloseSerialPort(obj,3);
                ResultConnect = _serialPortManager.ResultConnectValue;
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }
        #region GridViewStyle
        private ObservableCollection<ViewModelProvider> _gridData = new ObservableCollection<ViewModelProvider>();
        public ObservableCollection<ViewModelProvider> GridData {
            get { return _gridData; }
            set {
                _gridData = value;
                RaisePropertyChanged(nameof(GridData));
            }
        }
        #endregion

        #region
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e) {
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
                        if (matchingManager.IsDuplicate(indata,PrintSuccese) == true) {
                            OnBinding_BoxGrid(resultData);
                        } else {
                            MessageBox.Show("중복된 모델 바코드 입니다");
                        }
                        // 매칭 데이터 바인딩
                    } else if (resultData.StartsWith("M")) {
                        if (matchingManager.IsDuplicate(indata, PrintSuccese) == true) {
                            OnBinding_MatchGrid(resultData);
                        } else {
                            MessageBox.Show("중복된 매치 바코드 입니다");
                        }
                    }

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
                Read_Write_ExcelData(Product_ID,PATH);
                //From_WebItem_To_Model();
                PrintSuccese = false;

                for (int i = 0; i < NumberOfColumns * NumberOfRows; i++) {
                    ViewModelProvider gridItem = new ViewModelProvider {
                        Index = i + 1,
                        ModelSerial = "",
                        Background = Brushes.Black,
                        GridRowSpan = 4 - MatchCount,
                        MatchGridRowSpan = MatchCount
                    };

                    gridItem.MatchItems = new ObservableCollection<ViewModelProvider>();
                    for (int j = 0; j < MatchCount; j++) {
                        ViewModelProvider matchItem = new ViewModelProvider {
                            MatchDataSerial = "",
                            MatchDataBackground = Brushes.Black
                        };
                        gridItem.MatchItems.Add(matchItem);
                    }

                    GridData.Add(gridItem);
                }
                BoxSize = GridData.Count.ToString();
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }

        public void Read_Write_ExcelData(string inputData, string path) {
            FileInfo fileInfo = new FileInfo(path);
            Trace.WriteLine("==========   Start   ==========\nMethodName : " + "\n" + (MethodBase.GetCurrentMethod().Name));
            try {
                using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // 첫 번째 워크시트 선택

                    int rows = worksheet.Dimension.Rows; // 세로 줄의 총 개수
                    int colunm = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rows; row++) {
                        string cellValue = worksheet.Cells[row, 1].Value?.ToString(); // A열 값 읽기 [ 품번 , ProductID ]

                        if (cellValue == inputData) {
                            LabelType = worksheet.Cells[row, 3].Value?.ToString(); // C열 값 읽기 [ Label Type ]

                            string colunmStr = worksheet.Cells[row, 4].Value?.ToString(); // D열 값 읽기 [ 가로 박스 개수 ]
                            NumberOfColumns = Int32.Parse(colunmStr);

                            string rowStr = worksheet.Cells[row, 5].Value?.ToString(); // E열 값 읽기 [ 새로 박스 개수 ]
                            NumberOfRows = Int32.Parse(rowStr);

                            ModelSerial = worksheet.Cells[row, 6].Value?.ToString(); // ModelSerial

                            BoxColorString = worksheet.Cells[row, 7].Value?.ToString(); // boxColor Convert To String

                            string matchNumberOfRows = worksheet.Cells[row, 11].Value?.ToString();
                            MatchCount = Int32.Parse(matchNumberOfRows);
                            MatchItemList.Clear();
                            for (int i = 0; i < 3; i++) {
                                if (worksheet.Cells[row, i + 8].Value?.ToString() != "") {
                                    MatchItemList.Add(worksheet.Cells[row, i + 8].Value?.ToString());
                                }
                            }
                            //break; // 해당 데이터를 찾았으므로 반복문 종료
                        }
                    }
                }
            } catch (Exception e) {
                Trace.WriteLine("========== Exception ==========\nMethodName : " + (MethodBase.GetCurrentMethod().Name) + "\nException : " + e);
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
                            webDriveManager.GetPrint(Product_ID, ScanCount, PrintCount, PrintSuccese);
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
                if (data == MatchItemList[MatchScanCount] && GridData[ScanCount].ModelSerial != "") { // M 매칭아이템이 들어왔을때
                    GridData[ScanCount].MatchItems[MatchScanCount].MatchDataSerial = "매칭 : " + data;
                    GridData[ScanCount].MatchItems[MatchScanCount].MatchDataBackground = Brushes.Green;
                    MatchScanCount++;

                    if (MatchScanCount == MatchCount) {
                        ScanCount++;
                    }
                    // 매칭카운트 개수와 상관없이, 마지막("M") 매칭 후 스캔 카운트가 도달했을때
                    if (ScanCount.ToString() == BoxSize) {
                        webDriveManager.GetPrint(Product_ID, ScanCount, PrintCount, PrintSuccese);
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
