using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Leak_UI.Model
{
    public class Main_GridItem : ViewModelProvider
    {
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

        private string _modelSerial;
        public string ModelSerial {
            get { return _modelSerial; }
            set {
                if (_modelSerial != value) {
                    _modelSerial = value;

                    RaisePropertyChanged("ModelSerial");
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
        private ObservableCollection<Main_GridItem> _matchItems = new ObservableCollection<Main_GridItem>();
        public ObservableCollection<Main_GridItem> MatchItems {
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
    }
}
