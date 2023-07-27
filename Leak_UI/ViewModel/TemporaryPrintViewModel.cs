using Leak_UI.Model;
using System;
using System.Windows.Controls;

namespace Leak_UI.ViewModel
{
    public class TemporaryPrintViewModel : ViewModelProvider
    {
        public TemporaryPrintViewModel() {
            Console.WriteLine(DateTime.Now.Month.ToString());
            TodayTest = DateTime.Now.ToString("yyMMdd");
            MonthReturn();
            // <!--5023G260001[50=수량,23=년도,G=월,26=일,0001=Page]-->
            IssueNumber = Count + DateTime.Now.ToString("yy") + MonthParse + DateTime.Now.Day.ToString() + string.Format("{0:D4}", IntNum);
            IssueBarcode = ModelSerials.Replace("-","") + "  " + IssueNumber;
            
        }



        private string _aground = "진천";
        public string Aground {
            get { return _aground; }
            set {
                _aground = value;
                RaisePropertyChanged(nameof(Aground));
            }
        }

        private string _companyName = "㈜현대모비스";
        public string CompanyName {
            get { return _companyName; }
            set {
                _companyName = value;
                RaisePropertyChanged(nameof(CompanyName));
            }
        }
        //발행번호 + 발행번호 데이터 (바코드 화 -엑셀기준Free 3 of 9)
        private string _issueBarcode;
        public string IssueBarcode {
            get { return _issueBarcode; }
            set {
                _issueBarcode = "*"+value+"*";
                RaisePropertyChanged(nameof(IssueBarcode));
            }
        }

        private string _modelName = "CAMERA ASSY-BACK VIEW";
        public string ModelName {
            get { return _modelName; }
            set {
                _modelName = value;
                RaisePropertyChanged(nameof(ModelName));
            }
        }

        private string _modelSerial = "95760-C9500";
        public string ModelSerials {
            get { return _modelSerial; }
            set {
                _modelSerial = value;
                RaisePropertyChanged(nameof(ModelSerial));
            }
        }

        private int _intNum = 0;
        public int IntNum {
            get { return _intNum; }
            set {
                _intNum = value;
                RaisePropertyChanged(nameof(IntNum));
            }
        }
        private string _strNum;
        public string StrNum {
            get { return _strNum; }
            set {
                _strNum = value;
                RaisePropertyChanged(nameof(StrNum));
            }
        }

        private string _todayTest;
        public string TodayTest {
            get { return _todayTest; }
            set {
                _todayTest = value;
                RaisePropertyChanged(nameof(TodayTest));
            }
        }

        private string _issueNumber;
        public string IssueNumber {
            get { return _issueNumber; }
            set {
                _issueNumber = value;
                RaisePropertyChanged(nameof(_issueNumber));
            }
        }

        private string _count = "50";
        public string Count {
            get { return _count; }
            set {
                _count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        private string _monthParse ;
        public string MonthParse {
            get { return _monthParse; }
            set {
                _monthParse = value;
                RaisePropertyChanged(nameof(MonthParse));
            }
        }

        public void MonthReturn () {
            char[] characters = new char[12];
            for (int i = 0; i < 12; i++) {
                characters[i] = (char)(65 + i); // 65부터 A, 66부터 B, ..., 76부터 L
            }
            for (int i = 1; i <= 12; i++) {
                char mappedValue = characters[i - 1];
                //Console.WriteLine($"{i} = {mappedValue}");
                if (DateTime.Now.Month.ToString() == i.ToString()) {
                    MonthParse = mappedValue.ToString();
                }
            }
        }
    }
}