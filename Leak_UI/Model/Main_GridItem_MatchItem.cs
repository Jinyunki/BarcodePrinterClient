using System.ComponentModel;
using System.Windows.Media;

namespace Leak_UI.Model
{
    public class Main_GridItem_MatchItem : ViewModelProvider
    {
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
