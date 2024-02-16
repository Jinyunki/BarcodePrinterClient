using Leak_UI.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Leak_UI.Utiles
{
    public class FileSearchManager
    {
        private FileSearchManager() 
        { 

        }

        /// <summary>
        ///  총사용 금액
        /// </summary>
        public static double ReturnUseCash;

        /// <summary>
        /// 엑셀 데이터를 담아줄 키 벨류
        /// </summary>
        public static Dictionary<string, List<string>> excelRowData = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> excelColumnData = new Dictionary<string, List<string>>();

        public static string NO = "No";
        public static List<string> numberList = new List<string>();
        
        public static string PERSON_CHARGER= "담당자";
        public static List<string> personChargerList = new List<string>();

        public static string DATE = "날짜";
        public static List<string> dateList = new List<string>();

        public static string CAYEGORY = "구분";
        public static int totalCategoryCount = 0;
        public static List<string> categoryListText = new List<string>();
        public static List<int> categoryListCount = new List<int>();
        public static int totalCount = 0;

        public static List<string> monthlyName = new List<string>();
        public static List<double> monthlyUseCash = new List<double>();

        public static string CONTENT = "내용";
        
        public static string CASH = "금액";
        
        public static string RECEIVING_STATUS = "입고현황";
        public static void GetFileSearchManager(string path, string type, string whatData)
        {
            Trace.WriteLine(ViewModelProvider.TraceCatch(MethodBase.GetCurrentMethod().Name));
            try
            {
                type = "*." + type;
                List<string> list = new List<string>();
                string[] excelFiles = Directory.GetFiles(path, type);
                if (excelFiles.Length > 0)
                {
                    foreach (string file in excelFiles)
                    {
                        if (file.Contains(whatData))
                        {
                            ReadExcelDataTest(file);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(ViewModelProvider.TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }

        public static void ReadExcelDataTest(string filePath)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // 첫 번째 시트 선택

                int rowCount = worksheet.Dimension.Rows; // 세로줄의 개수
                int colCount = worksheet.Dimension.Columns; // 가로줄의 개수
                
                // 세로(카테고리별) 데이터
                for (int col = 1; col <= colCount; col++)
                {
                    string columnName = worksheet.Cells[1, col].Text; // 첫 번째 행의 각 셀에서 열 이름을 가져옴
                    List<string> columnData = new List<string>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string cellValue = worksheet.Cells[row, col].Text;
                        if (cellValue != "")
                        {
                            columnData.Add(cellValue);
                        }
                    }

                    excelRowData[columnName] = columnData;
                }

                double totalMonthlyCash = 0.0;
                List<double> monthlyCash = new List<double>();
                List<double> CategoryList = new List<double>();
                foreach (var kvp in GetAmountStatistics("금액"))
                {
                    Console.WriteLine($"{kvp.Key} 월 금액 합계: {kvp.Value}");
                    monthlyName.Add(kvp.Key+" 월");
                    monthlyUseCash.Add(kvp.Value);
                    totalMonthlyCash += kvp.Value;
                    monthlyCash.Add(kvp.Value);
                }

                foreach (var kvp in GetCategorizedStatistics("구분"))
                {
                    categoryListText.Add(kvp.Key);
                    categoryListCount.Add(kvp.Value);
                    totalCount += kvp.Value;
                    Console.WriteLine($"Category: {kvp.Key}, Count: {kvp.Value}");
                }


                Console.WriteLine($"전체 월별 금액 합계: {totalMonthlyCash}");
                ReturnUseCash = totalMonthlyCash;
            }
        }

        public static Dictionary<string, int> GetCategorizedStatistics(string selectCategoryValue)
        {
            List<string> categoryColumn = excelRowData[selectCategoryValue];
            Dictionary<string, int> categoryCounts = new Dictionary<string, int>();

            foreach (string category in categoryColumn)
            {
                if (!categoryCounts.ContainsKey(category))
                {
                    categoryCounts[category] = 0;
                }
                categoryCounts[category]++;
            }

            return categoryCounts;
        }

        public static Dictionary<string, double> GetAmountStatistics(string selectCategoryValue)
        {
            Dictionary<string, double> monthlyTotals = new Dictionary<string, double>();

            List<string> dateColumn = excelRowData["날짜"];
            List<string> amountColumn = excelRowData[selectCategoryValue];

            for (int i = 0; i < dateColumn.Count; i++)
            {
                string date = dateColumn[i];
                int index = date.IndexOf('-');
                if (index >= 0)
                {
                    string yearMonth = date.Substring(0, index + 3); // "yyyy-MM" 형식으로 추출
                    if (!monthlyTotals.ContainsKey(yearMonth))
                    {
                        monthlyTotals[yearMonth] = 0.0;
                    }

                    int amountIndex = amountColumn[i].IndexOfAny("0123456789".ToCharArray());
                    if (amountIndex >= 0)
                    {
                        string amountStr = amountColumn[i].Substring(amountIndex).Replace(",", "");
                        if (double.TryParse(amountStr, out double parsedValue))
                        {
                            monthlyTotals[yearMonth] += parsedValue;
                        }
                    }
                }
            }

            return monthlyTotals;
        }
        public static double ReturnLimitedCash = 200000000;

    }
}
