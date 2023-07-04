﻿using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;

namespace Leak_UI.Model
{
    public class MainModel : ViewModelBase
    {
        #region Item List

        public string webUri = "https://jc-label.mobis.co.kr/";
        public string id = "SS1F";
        public string ps = "Sekonix12@@";

        public string BOX_LABEL_HYUNDAI_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[1]/div/a";
        public string BOX_LABEL_HYUNDAI_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[2]/div/a";
        public string BOX_LABEL_HYUNDAI_SMALL = "//*[@id='MOTOR_LABEL']/ul/li[3]/div/a";
        public string BOX_LABEL_KIA_LARGE = "//*[@id='MOTOR_LABEL']/ul/li[4]/div/a";
        public string BOX_LABEL_KIA_MIDDLE = "//*[@id='MOTOR_LABEL']/ul/li[5]/div/a";

        public string MODEL_PRODUCT_ID = "ContentPlaceHolder1_txtProductID";
        public string PACKAGED_QUANTITY = "ContentPlaceHolder1_txtPackQty";
        public string PRINT_QUANTITY = "ContentPlaceHolder1_txtPrintQty";
        public string PATH = Path.Combine(@"D:\JinYunki\Leak_UI2\Leak_UI\bin\Release", "LabelConfig.xlsx");
        
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

        #endregion
        #region Property Item List

        private string matchItme1;
        public string MatchItem1 {
            get { return matchItme1; }
            set {
                matchItme1 = value;
                RaisePropertyChanged("MatchItem1");
            }
        }
        private string matchItme2;
        public string MatchItem2 {
            get { return matchItme2; }
            set {
                matchItme2 = value;
                RaisePropertyChanged("MatchItem2");
            }
        }
        private string matchItme3;
        public string MatchItem3 {
            get { return matchItme3; }
            set {
                matchItme3 = value;
                RaisePropertyChanged("MatchItem3");
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

        // 박스 라벨 타입
        private string labelType = "LABEL TYPE";
        public string LabelType {
            get { return labelType; }
            set {
                labelType = value;
                RaisePropertyChanged("BoxType");
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

        // 품번 입력
        private string product_ID = "작업지시서를 스캔 하세요";
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

        // 발행 수량
        private string printCount = "1";
        public string PrintCount {
            get { return printCount; }
            set {
                printCount = value;
                RaisePropertyChanged("PrintCount");
            }
        }
        // Box Color 
        private Brush boxColor = Brushes.Black;
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
        public ICommand BtnPrintCommand { get; set; }
        public ICommand BtnPortConnectCommand { get; set; }
        #endregion
    }

}