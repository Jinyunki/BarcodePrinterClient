using Leak_UI.Model;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System;
using Leak_UI.Utiles;

namespace Leak_UI.ViewModel
{
    public class ExcelRecipe_ViewModel : ViewModelProvider
    {
        public ExcelRecipe_ViewModel() {
            ExcelReadManager.LoadExcelData(PATH);
            ExcelData = ExcelReadManager.ExcelDataProperty;
        }

        ExcelReadManager _excelReadManager = new ExcelReadManager();
        private ObservableCollection<List<object>> _excelData = new ObservableCollection<List<object>>();
        public ObservableCollection<List<object>> ExcelData {
            get { return _excelData; }
            set {
                _excelData = value;
                RaisePropertyChanged(nameof(ExcelData));

                // 업데이트 트리거
                UpdateExcelFile(PATH);
            }
        }
        
        public void UpdateExcelFile(string path) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                FileInfo fileInfo = new FileInfo(path);

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
