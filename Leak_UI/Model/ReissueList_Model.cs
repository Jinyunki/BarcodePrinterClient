using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

namespace Leak_UI.Model
{
    public class ReissueList_Model : WebCrowlingCollection
    {
        public ICommand btnInquiry { get; set; }

        // 데이터리스트 긁어오기 (XPath 기반)
        private void LoadDataList(IWebDriver driver, string xpath) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                var _table = driver.FindElement(By.XPath(xpath));
                var _tBody = _table.FindElement(By.TagName("tbody")); // DataListBody
                var _tRows = _tBody.FindElements(By.TagName("tr"));
                List<List<object>> WebRowList = new List<List<object>>();

                foreach (var row in _tRows.Skip(1)) {
                    List<object> WebColumnList = new List<object>();
                    var columns = row.FindElements(By.TagName("td"));
                    if (columns.Count >= 2) // 최소한 2개의 열이 필요함
                    {
                        for (int i = 0; i < columns.Count; i++) {
                            object value = columns[i].Text;
                            WebColumnList.Add(value);
                        }
                        WebRowList.Add(WebColumnList);
                    }
                    WebDataList = new ObservableCollection<List<object>>(WebRowList);
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
            
        }
        // 데이터시트 조회
        public void LoadDataListAdd(ChromeDriver driver) {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                Thread.Sleep(1000);
                LoadDataList(driver, DATALIST_BODY);
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }
        public void GetWebDataRead(string inputProductID) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                SetWebDrive();

                Login(driver);
                // 부품식별표 > 라벨선택기능 추가필요 ( 입력될 품번을 기준으로 레시피에서 참고하여 업데이트해야함 )
                ReadExcelData(inputProductID);
                SelectLabel(driver);
                // 고객품번 입력후 엔터 기능 추가필요
                SendTextInput(driver, MODEL_PRODUCT_ID, inputProductID);
                SetWebDriveWaiting(driver, MODEL_PRODUCT_ID);

                LoadDataListAdd(driver);
                driver.Quit();
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }

        private ObservableCollection<List<object>> _webDataList;
        public ObservableCollection<List<object>> WebDataList {
            get { return _webDataList; }
            set {
                _webDataList = value;
                RaisePropertyChanged(nameof(WebDataList));
            }
        }
        private string _searchItem = "99240AA600";
        public string SearchItem {
            get { return _searchItem; }
            set {
                _searchItem = value;
                RaisePropertyChanged(nameof(SearchItem));
            }
        }
    }
}
