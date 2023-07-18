using Leak_UI.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Leak_UI.View
{
    /// <summary>
    /// ReissueList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReissueList : UserControl
    {
        private ReissueListViewModel _viewModel;
        public ReissueList() {
            InitializeComponent();
            _viewModel = DataContext as ReissueListViewModel;
        }
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            // 수정된 값을 ViewModel에 반영
            if (e.EditAction == DataGridEditAction.Commit) {
                var editedTextbox = e.EditingElement as TextBox;
                if (editedTextbox != null) {
                    var newValue = editedTextbox.Text;
                    var dataItem = e.Row.Item as List<object>;
                    var columnIndex = e.Column.DisplayIndex;
                    dataItem[columnIndex] = newValue;
                }
            }
        }
    }
}
