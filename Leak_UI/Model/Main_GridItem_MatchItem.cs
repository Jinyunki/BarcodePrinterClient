using System.ComponentModel;
using System.Windows.Media;

namespace Leak_UI.Model
{
    public class Main_GridItem_MatchItem : INotifyPropertyChanged
    {
        private string _matchDataSerial;
        public string MatchDataSerial {
            get { return _matchDataSerial; }
            set {
                if (_matchDataSerial != value) {
                    _matchDataSerial = value;
                    OnPropertyChanged("MatchDataSerial");
                }
            }
        }
        private Brush _matchDataBackground;

        public Brush MatchDataBackground {
            get { return _matchDataBackground; }
            set {
                if (_matchDataBackground != value) {
                    _matchDataBackground = value;
                    OnPropertyChanged("MatchDataBackground");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
