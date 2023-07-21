using Leak_UI.Utiles;
using Leak_UI.Model;
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
            btMainHome = new Command(btMainHomeCute);
            btExcel = new Command(btExcelCute);
            btData = new Command(btDataCute);
        }

        private void btMainHomeCute(object obj) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            CurrentViewModel = _locator.Main_MatchingViewModel;
        }

        private void btExcelCute(object obj) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            CurrentViewModel = _locator.ExcelRecipe_ViewModel;
        }

        private void btDataCute(object obj) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            CurrentViewModel = _locator.ReissueListViewModel;
        }

    }
}