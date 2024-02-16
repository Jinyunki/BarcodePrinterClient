using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Leak_UI.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Leak_UI.Model
{
    public class MainModel : ViewModelProvider
    {
        public ICommand btMainHome { get; set; }
        public ICommand btExcel { get; set; }
        public ICommand btData { get; set; }
        public ICommand btTemporary { get; set; }
        public ICommand btStatistics { get; set; }

        #region CurrentViewChanger

        public ViewModelLocator _locator = new ViewModelLocator();
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel {
            get {
                //Messenger.Default.Send(new SignalMessage(IsViewName, IsPress));
                return _currentViewModel;
            }
            set {
                if (_currentViewModel != value) {
                    Set<ViewModelBase>(ref _currentViewModel, value);
                    _currentViewModel.RaisePropertyChanged("CurrentViewModel");
                }
            }
        }
        #region Window State

        private WindowState _windowState;
        public WindowState WindowState {
            get { return _windowState; }
            set {
                if (_windowState != value) {
                    _windowState = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ICommand BtnMinmize { get; private set; }
        public ICommand BtnMaxsize { get; private set; }
        public ICommand BtnClose { get; private set; }

        public void WinBtnEvent() {
            BtnMinmize = new RelayCommand(WinMinmize);
            BtnMaxsize = new RelayCommand(WinMaxSize);
            BtnClose = new RelayCommand(WindowClose);
        }
        // Window Minimize
        private void WinMinmize() {
            WindowState = WindowState.Minimized;
        }

        // Window Size
        private void WinMaxSize() {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }
        private void WindowClose() {
            Application.Current.Shutdown();
        }

        #endregion

        #endregion
    }
}
