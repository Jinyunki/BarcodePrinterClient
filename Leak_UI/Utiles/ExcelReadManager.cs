using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
using System.Collections.ObjectModel;
using System;

namespace Leak_UI.Utiles
{
    public class ExcelReadManager
    {
        public static ObservableCollection<List<object>> ExcelDataProperty { get; set; }
        /// <summary>
        /// path = 엑셀레시피의 경로를 파라메터에 입력하시면 됩니다.
        /// excelDataProperty = 타입과 동일한 변수를 생성하여 입력하시오.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="excelDataProperty"></param>
        /// <returns></returns>
        public static ObservableCollection<List<object>> LoadExcelData(string path) {
            try {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(path))) {
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
                    ExcelDataProperty = new ObservableCollection<List<object>>(dataList);
                }

                return ExcelDataProperty;
            } catch (Exception) {
                throw;
            }
        }
    }
}
