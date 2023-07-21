using Leak_UI.Model;
using Leak_UI.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Leak_UI.View
{
    /// <summary>
    /// ExcelRecipeView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExcelRecipeView : UserControl
    {
        private ExcelRecipe_ViewModel _viewModel;

        public ExcelRecipeView() {
            InitializeComponent();
            _viewModel = DataContext as ExcelRecipe_ViewModel;
        }

        // GUI상 변경된 데이터를 엑셀로 반영
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            // 수정된 값을 ViewModel에 반영
            if (e.EditAction == DataGridEditAction.Commit) {
                var editedTextbox = e.EditingElement as TextBox;
                if (editedTextbox != null) {
                    var newValue = editedTextbox.Text;
                    var dataItem = e.Row.Item as List<object>;
                    var columnIndex = e.Column.DisplayIndex;
                    dataItem[columnIndex] = newValue;

                    // Excel 데이터 수정
                    _viewModel.UpdateExcelFile(ViewModelProvider.PATH);
                }
            }
        }

    }
}
