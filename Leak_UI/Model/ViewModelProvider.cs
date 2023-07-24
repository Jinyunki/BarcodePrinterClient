using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media;

namespace Leak_UI.Model
{
    public class ViewModelProvider : ViewModelBase
    {
        public IDispatcher dispatcher;
        public ChromeDriverService driverService;
        public ChromeOptions options;
        public ChromeDriver driver ;
        public WebDriveManager webDriveManager = new WebDriveManager();
        public SerialPortManager _serialPortManager = new SerialPortManager();
        public MatchingManager matchingManager = new MatchingManager();
        /// <summary>
        /// 23 07 21 해당 경로를 직접 정하고 변경할수있게 수정하면 어떨까? 업데이트예정
        /// </summary>
        public static string PATH = Path.Combine(@"D:\JinYunki\BoxLabe(23.07.20)\Leak_UI\Leak_UI\RecipeData", "LabelConfig.xlsx");

        public void From_WebItem_To_Model() {
            LabelType = webDriveManager.LabelType;
            NumberOfColumns = webDriveManager.NumberOfColumns;
            NumberOfRows = webDriveManager.NumberOfRows;
            ModelSerial = webDriveManager.ModelSerial;
            BoxColorString = webDriveManager.BoxColorString;
            MatchCount = webDriveManager.MatchCount;
            MatchItemList = webDriveManager.MatchItemList;
            WebDataList = webDriveManager.WebDataList;
        }

        private ObservableCollection<List<object>> _webDataList;
        public ObservableCollection<List<object>> WebDataList {
            get { return _webDataList; }
            set {
                _webDataList = value;
                RaisePropertyChanged(nameof(WebDataList));
            }
        }
        private string _searchItem = "99240AA600";
        public string SearchItem {
            get { return _searchItem; }
            set {
                _searchItem = value;
                RaisePropertyChanged(nameof(SearchItem));
            }
        }

        private List<string> _matchItemList = new List<string>();
        public List<string> MatchItemList {
            get { return _matchItemList; }
            set {
                _matchItemList = value;
                RaisePropertyChanged(nameof(MatchItemList));
            }
        }

        private int _matchCount = 0;
        public int MatchCount {
            get { return _matchCount; }
            set {
                _matchCount = value;
                RaisePropertyChanged(nameof(MatchCount));
            }
        }

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

        // 박스 라벨 타입
        private string labelType = "LABEL TYPE";
        public string LabelType {
            get { return labelType; }
            set {
                labelType = value;
                RaisePropertyChanged("LabelType");
            }
        }
        // 품번 입력
        private string product_ID = "작업지시서를 스캔 하세요";
        public string Product_ID {
            get { return product_ID; }
            set {
                product_ID = value;
                RaisePropertyChanged("Product_ID");
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
        // 발행 수량
        private string printCount = "1";
        public string PrintCount {
            get { return printCount; }
            set {
                printCount = value;
                RaisePropertyChanged("PrintCount");
            }
        }
        //PrintProgress
        private string printProgress = "출력 대기";
        public string PrintProgress {
            get { return printProgress; }
            set {
                printProgress = value;
                RaisePropertyChanged("PrintProgress");
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
        // Box Color 
        private Brush boxColor;
        public string BoxColorString {
            get { return boxColor.ToString(); }
            set {
                BrushConverter converter = new BrushConverter();
                Brush newBrush = (Brush)converter.ConvertFromString(value);
                boxColor = newBrush;
                RaisePropertyChanged("BoxColor");
            }
        }
        public Brush BoxColor {
            get { return boxColor; }
            set {
                boxColor = value;
                RaisePropertyChanged("BoxColor");
            }
        }
        // MatchCount
        private int matchScanCount;
        public int MatchScanCount {
            get { return matchScanCount; }
            set {
                matchScanCount = value;
                if (matchScanCount == MatchCount) {
                    matchScanCount = 0;
                    ScanCount++;
                }
                RaisePropertyChanged("MatchScanCount");
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

        // 포트 연결 상태
        private string resultConnect = "포트 연결을 눌러 주세요";
        public string ResultConnect {
            get { return resultConnect; }
            set {
                resultConnect = value;
                RaisePropertyChanged("ResultConnect");
            }
        }

        private bool _printSuccese = false;
        public bool PrintSuccese {
            get { return _printSuccese; }
            set {
                _printSuccese = value;
                RaisePropertyChanged("PrintSuccese");
            }
        }

        #region MatchItemList

        private int _index;
        public int Index {
            get { return _index; }
            set {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }
        private int _gridRowSpan;
        public int GridRowSpan {
            get { return _gridRowSpan; }
            set {
                if (_gridRowSpan != value) {
                    _gridRowSpan = value;
                    RaisePropertyChanged("GridRowSpan");
                }
            }
        }
        private int _matchGridRowSpan;
        public int MatchGridRowSpan {
            get { return _matchGridRowSpan; }
            set {
                if (_matchGridRowSpan != value) {
                    _matchGridRowSpan = value;
                    RaisePropertyChanged("MatchGridRowSpan");
                }
            }
        }

        private Brush _background;
        public Brush Background {
            get { return _background; }
            set {
                if (_background != value) {
                    _background = value;
                    RaisePropertyChanged("Background");
                }
            }
        }
        private ObservableCollection<ViewModelProvider> _matchItems = new ObservableCollection<ViewModelProvider>();
        public ObservableCollection<ViewModelProvider> MatchItems {
            get { return _matchItems; }
            set {
                _matchItems = value;
                RaisePropertyChanged(nameof(MatchItems));
            }
        }

        private string _matchDataSerial;
        public string MatchDataSerial {
            get { return _matchDataSerial; }
            set {
                if (_matchDataSerial != value) {
                    _matchDataSerial = value;
                    RaisePropertyChanged("MatchDataSerial");
                }
            }
        }
        private Brush _matchDataBackground;

        public Brush MatchDataBackground {
            get { return _matchDataBackground; }
            set {
                if (_matchDataBackground != value) {
                    _matchDataBackground = value;
                    RaisePropertyChanged("MatchDataBackground");
                }
            }
        }

        #endregion

        public string TraceStart(string methodName) {
            return "==========   Start   ==========\nMethodName : " + methodName + "\n";
        }
        public string TraceCatch(string methodName) {
            return "========== Exception ==========\nMethodName : " + methodName + "\nException : ";
        }
    }
}
