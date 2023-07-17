using GalaSoft.MvvmLight;
using System.Windows.Input;
using System.ComponentModel;

namespace Leak_UI.Model
{
    public class MainModel : ViewModelBase
    {
        public ICommand btMainHome { get; set; }
        public ICommand btExcel { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
