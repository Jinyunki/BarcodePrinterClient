using Leak_UI.Model;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace Leak_UI.ViewModel
{
    public class ExcelRecipeViewModel : ViewModelBase
    {
        public ExcelRecipeViewModel() {
            LoadExcelData();
            
        }

        #region LoadExcelData

        private ObservableCollection<List<object>> _excelData;
        public ObservableCollection<List<object>> ExcelData {
            get { return _excelData; }
            set {
                _excelData = value;
                OnPropertyChanged(nameof(ExcelData));
                // 업데이트 트리거
                UpdateExcelFile();
            }
        }

        /// 엑셀 데이터를 불러오는곳
        public ObservableCollection<List<object>> LoadExcelData() {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(WebCrowling.PATH))) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int columnCount = worksheet.Dimension.Columns-1;

                List<List<object>> dataList = new List<List<object>>();

                for (int row = 2; row <= rowCount; row++) {
                    List<object> rowData = new List<object>();

                    for (int column = 1; column <= columnCount; column++) {
                        object cellValue = worksheet.Cells[row, column].Value;
                        rowData.Add(cellValue);
                    }

                    dataList.Add(rowData);
                }

                ExcelData = new ObservableCollection<List<object>>(dataList);
            }

            return ExcelData;
        }

        /// 엑셀 데이터 업데이트
        public void UpdateExcelFile() {
            FileInfo fileInfo = new FileInfo(WebCrowling.PATH);

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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
