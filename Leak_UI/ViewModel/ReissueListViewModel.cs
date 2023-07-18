using GalaSoft.MvvmLight;
using Leak_UI.Model;
using Leak_UI.Utiles;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Leak_UI.ViewModel
{
    public class ReissueListViewModel : ViewModelBase
    {
        Main_Crowling Main_Crowling = new Main_Crowling();
        public ICommand btnInquiry { get; set; }
        public ReissueListViewModel() {
            btnInquiry = new Command(btnInquiryCute,CanExCute);
        }

        private void btnInquiryCute(object obj) {
            Main_Crowling = new Main_Crowling();
            Main_Crowling.GetWebDataRead(Main_Crowling.SearchProduct_ID);
            DataList = Main_Crowling.WebDataList;
        }

        private bool CanExCute(object obj) {
            return true;
        }

        private ObservableCollection<List<object>> _dataList;
        public ObservableCollection<List<object>> DataList {
            get { return _dataList; }
            set {
                _dataList = value;
                RaisePropertyChanged(nameof(DataList));
            }
        }
    }
}
