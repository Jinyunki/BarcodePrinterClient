using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Leak_UI.Model;
using System;

namespace Leak_UI.ViewModel
{
    public class MainViewModel : MainModel
    {
        #region CurrentViewChanger
        public ViewModelLocator _locator = new ViewModelLocator();
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel {
            get {
                return _currentViewModel;
            }
            set {
                if (_currentViewModel != value) {
                    Set<ViewModelBase>(ref _currentViewModel, value);
                    _currentViewModel.RaisePropertyChanged("CurrentViewModel");
                }
            }
        }

        #endregion
        public MainViewModel() {
            CurrentViewModel = _locator.MainProgramViewModel;
            WinBtnEvent();
            BtnEvent();
        }

        private void BtnEvent() {
            btMainHome = new Command(btMainHomeCute, CanExCute);
            btExcel = new Command(btExcelCute, CanExCute);
        }

        private void btExcelCute(object obj) {
            CurrentViewModel = _locator.ExcelRecipeViewModel;
        }

        private void btMainHomeCute(object obj) {
            CurrentViewModel = _locator.MainProgramViewModel;
        }
        private bool CanExCute(object obj) {
            return true;
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

        private void WinBtnEvent() {
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
    }
}