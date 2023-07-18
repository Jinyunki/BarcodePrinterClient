using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Leak_UI.Model
{
    public class ExcelDataModel : INotifyPropertyChanged
    {
        private ObservableCollection<List<object>> _excelDataList;
        public ObservableCollection<List<object>> ExcelDataList {
            get { return _excelDataList; }
            set {
                _excelDataList = value;
                OnPropertyChanged(nameof(ExcelDataList));
            }
        }

        public ObservableCollection<List<object>> LoadExcelData() {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(Main_Crowling.PATH))) {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int columnCount = worksheet.Dimension.Columns - 1;

                List<List<object>> dataList = new List<List<object>>();

                for (int row = 2; row <= rowCount; row++) {
                    List<object> rowData = new List<object>();

                    for (int column = 1; column <= columnCount; column++) {
                        object cellValue = worksheet.Cells[row, column].Value;
                        rowData.Add(cellValue);
                    }

                    dataList.Add(rowData);
                }
                ExcelDataList = new ObservableCollection<List<object>>(dataList);
            }

            return ExcelDataList;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
