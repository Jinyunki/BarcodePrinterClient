using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using Leak_UI.Model;
using System.Collections.ObjectModel;

namespace Leak_UI.ViewModel
{
    public class ExcelRecipeViewModel : MainModel
    {

        public ExcelRecipeViewModel() {
            LoadExcelData();
        }
        private ObservableCollection<List<object>> _excelData;
        public ObservableCollection<List<object>> ExcelData {
            get { return _excelData; }
            set {
                _excelData = value;
                OnPropertyChanged(nameof(ExcelData));
                UpdateExcelFile();

            }
        }
        
        public void LoadExcelData() {
            // Excel 파일 경로

            using (ExcelPackage package = new ExcelPackage(new FileInfo(PATH))) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int columnCount = worksheet.Dimension.Columns;

                List<List<object>> dataList = new List<List<object>>();

                for (int row = 2; row <= rowCount; row++) {
                    List<object> rowData = new List<object>();

                    for (int column = 1; column <= columnCount; column++) {
                        object cellValue = worksheet.Cells[row, column].Value;
                        rowData.Add(cellValue ?? "-");
                    }

                    dataList.Add(rowData);
                }

                ExcelData = new ObservableCollection<List<object>>(dataList);
            }
        }
        
        // 엑셀 레시피 데이터 업데이트 메서드
        public void UpdateExcelFile() {
            FileInfo fileInfo = new FileInfo(PATH);

            using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

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
    }
}
