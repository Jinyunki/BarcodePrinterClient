using Leak_UI.Model;
using Leak_UI.Utiles;

namespace Leak_UI.ViewModel
{
    public class ReissueListViewModel : ReissueList_Model
    {
        
        public ReissueListViewModel() {
            btnInquiry = new Command(BtnInquiryCute,CanExCute);
        }

        private void BtnInquiryCute(object obj) {
            GetWebDataRead(SearchItem);
        }

        private bool CanExCute(object obj) {
            return true;
        }
    }
}
