using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leak_UI.Model
{
    public class ExcelItems : INotifyPropertyChanged
    {
        private string _productID = "-";
        public string ProductID {
            get { return _productID; }
            set {
                _productID = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }

        private string _modelName = "-";
        public string ModelName {
            get { return _modelName; }
            set {
                _modelName = value;
                OnPropertyChanged(nameof(ModelName));
            }
        }

        private string _labelType = "-";
        public string LabelType {
            get { return _labelType; }
            set {
                _labelType = value;
                OnPropertyChanged(nameof(LabelType));
            }
        }

        private string _colSize = "-";
        public string ColSize {
            get { return _colSize; }
            set {
                _colSize = value;
                OnPropertyChanged(nameof(ColSize));
            }
        }

        private string _rowSize = "-";
        public string RowSize {
            get { return _rowSize; }
            set {
                _rowSize = value;
                OnPropertyChanged(nameof(RowSize));
            }
        }

        private string _modelCode = "-";
        public string ModelCode {
            get { return _modelCode; }
            set {
                _modelCode = value;
                OnPropertyChanged(nameof(ModelCode));
            }
        }

        private string _boxColor = "-";
        public string BoxColor {
            get { return _boxColor; }
            set {
                _boxColor = value;
                OnPropertyChanged(nameof(BoxColor));
            }
        }

        private string _match1 = "-";
        public string Match1 {
            get { return _match1; }
            set {
                _match1 = value;
                OnPropertyChanged(nameof(Match1));
            }
        }

        private string _match2 = "-";
        public string Match2 {
            get { return _match2; }
            set {
                _match2 = value;
                OnPropertyChanged(nameof(Match2));
            }
        }

        private string _match3 = "-";
        public string Match3 {
            get { return _match3; }
            set {
                _match3 = value;
                OnPropertyChanged(nameof(Match3));
            }
        }

        private string _matchCount = "-";
        public string MatchCount {
            get { return _matchCount; }
            set {
                _matchCount = value;
                OnPropertyChanged(nameof(MatchCount));
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
