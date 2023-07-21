using Leak_UI.Model;
using Leak_UI.Utiles;
using System.Windows.Input;

namespace Leak_UI.ViewModel
{
    public class ReissueListViewModel : ViewModelProvider
    {
        public ReissueListViewModel() {
            btnInquiry = new Command(BtnInquiryCute);
        }
        private void BtnInquiryCute(object obj) {
            webDriveManager.GetWebDataRead(SearchItem,WebDataList);
            From_WebItem_To_Model();
        }
        public ICommand btnInquiry { get; set; }

    }
}
