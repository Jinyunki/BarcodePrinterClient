using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Leak_UI.Model
{
    public class GridItem : INotifyPropertyChanged
    {
        public List<MatchItemData> MatchItem { get; set; }
        public int Index { get; set; }
        public string ModelSerial { get; set; }
        public Brush Background { get; set; }
        public int GridRowSpan { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class MatchItemData : INotifyPropertyChanged
        {
            private string matchModelSerial;
            private Brush matchBackground = Brushes.White;

            public string MatchModelSerial {
                get { return matchModelSerial; }
                set {
                    if (matchModelSerial != value) {
                        matchModelSerial = value;
                        OnPropertyChanged(nameof(MatchModelSerial));
                    }
                }
            }

            public Brush MatchBackground {
                get { return matchBackground; }
                set {
                    if (matchBackground != value) {
                        matchBackground = value;
                        OnPropertyChanged(nameof(MatchBackground));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
