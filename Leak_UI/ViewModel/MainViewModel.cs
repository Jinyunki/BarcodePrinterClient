using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Leak_UI.Model;
using System;
using System.Reflection;
using System.Diagnostics;

namespace Leak_UI.ViewModel
{
    public class MainViewModel : MainModel
    {
        public MainViewModel() {
            CurrentViewModel = _locator.Main_MatchingViewModel;
            WinBtnEvent();
            BtnEvent();
        }

        private void BtnEvent() {
            btMainHome = new Command(btMainHomeCute, CanExCute);
            btExcel = new Command(btExcelCute, CanExCute);
            btData = new Command(btDataCute, CanExCute);
        }
        private void btDataCute(object obj) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            CurrentViewModel = _locator.ReissueListViewModel;
        }
        private void btExcelCute(object obj) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            CurrentViewModel = _locator.ExcelRecipe_ViewModel;
        }

        private void btMainHomeCute(object obj) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            CurrentViewModel = _locator.Main_MatchingViewModel;
        }
        private bool CanExCute(object obj) {
            return true;
        }



        
    }
}