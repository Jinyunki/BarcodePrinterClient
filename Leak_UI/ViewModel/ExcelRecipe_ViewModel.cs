using Leak_UI.Model;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Leak_UI.ViewModel
{
    public class ExcelRecipe_ViewModel : ViewModelBase
    {
        ExcelDataModel excelDataModel = new ExcelDataModel();
        public ExcelRecipe_ViewModel() {
            ExcelData = excelDataModel.LoadExcelData();
        }

        #region LoadExcelData
        private ObservableCollection<List<object>> _excelData;
        public ObservableCollection<List<object>> ExcelData {
            get { return _excelData; }
            set {
                _excelData = value;
                RaisePropertyChanged(nameof(ExcelData));
                // 업데이트 트리거
                UpdateExcelFile();
            }
        }

        /// // GUI상 변경된 데이터를 엑셀로 반영
        public void UpdateExcelFile() {
            FileInfo fileInfo = new FileInfo(Main_Crowling.PATH);

            using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns-1;

                // Excel 데이터를 수정된 데이터로 업데이트
                for (int row = 2; row <= rows; row++) {
                    for (int column = 1; column <= columns; column++) {
                        object newValue = ExcelData[row - 2][column - 1];
                        worksheet.Cells[row, column].Value = newValue;
                    }
                }

                package.Save();
            }
        }
        #endregion
    }
}
