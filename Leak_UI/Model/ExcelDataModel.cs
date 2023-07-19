using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System;

namespace Leak_UI.Model
{
    public class ExcelDataModel : ViewModelProvider
    {
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

        public ObservableCollection<List<object>> LoadExcelData() {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

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
                    ExcelData = new ObservableCollection<List<object>>(dataList);
                }

                return ExcelData;
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }

        public void UpdateExcelFile() {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                FileInfo fileInfo = new FileInfo(Main_Crowling.PATH);

                using (ExcelPackage package = new ExcelPackage(fileInfo)) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rows = worksheet.Dimension.Rows;
                    int columns = worksheet.Dimension.Columns - 1;

                    // Excel 데이터를 수정된 데이터로 업데이트
                    for (int row = 2; row <= rows; row++) {
                        for (int column = 1; column <= columns; column++) {
                            object newValue = ExcelData[row - 2][column - 1];
                            worksheet.Cells[row, column].Value = newValue;
                        }
                    }

                    package.Save();
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }
    }
}
