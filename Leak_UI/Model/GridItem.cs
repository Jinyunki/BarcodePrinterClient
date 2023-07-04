using System.ComponentModel;
using System.Windows.Media;

namespace Leak_UI.Model
{
    public class GridItem
    {
        private int index;
        public int Index {
            get { return index; }
            set {
                if (index != value) {
                    index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        private string modelSerial;
        public string ModelSerial {
            get { return modelSerial; }
            set {
                if (this.modelSerial != value) {
                    this.modelSerial = value;
                    OnPropertyChanged(nameof(ModelSerial));
                }
            }
        }

        private Brush _background;
        public Brush Background {
            get { return _background; }
            set {
                if (_background != value) {
                    _background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }
        private string matchItem1;
        public string MatchItem1 {
            get { return matchItem1; }
            set {
                if (this.matchItem1 != value) {
                    this.matchItem1 = value;
                    OnPropertyChanged(nameof(MatchItem1));
                }
            }
        }
        private string matchItem2;
        public string MatchItem2 {
            get { return matchItem2; }
            set {
                if (this.matchItem2 != value) {
                    this.matchItem2 = value;
                    OnPropertyChanged(nameof(MatchItem2));
                }
            }
        }
        private string matchItem3;
        public string MatchItem3 {
            get { return matchItem3; }
            set {
                if (this.matchItem3 != value) {
                    this.matchItem3 = value;
                    OnPropertyChanged(nameof(MatchItem3));
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
