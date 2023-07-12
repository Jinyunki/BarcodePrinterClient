using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;

namespace Leak_UI.Model
{
    public class GridItem : INotifyPropertyChanged
    {
        public int Index { get; set; }

        private string _modelSerial;
        public string ModelSerial {
            get { return _modelSerial; }
            set {
                if (_modelSerial != value) {
                    _modelSerial = value;
                    OnPropertyChanged("ModelSerial");
                }
            }
        }
        private Brush _background;
        public Brush Background {
            get { return _background; }
            set {
                if (_background != value) {
                    _background = value;
                    OnPropertyChanged("Background");
                }
            }
        }
        private ObservableCollection<MatchItem> _matchItems = new ObservableCollection<MatchItem>();
        public ObservableCollection<MatchItem> MatchItems {
            get { return _matchItems; }
            set {
                _matchItems = value;
                OnPropertyChanged(nameof(MatchItems));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
