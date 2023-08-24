using Leak_UI.Model;
using Leak_UI.Utiles;
using Leak_UI.View;
using System;
using System.Drawing.Printing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Printing;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Xps;
using System.Windows;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;

namespace Leak_UI.ViewModel
{
    public class TemporaryPrintViewModel : ViewModelProvider
    {
        PrintDialog print ;
        //PrintDocument printDocument;
        public TemporaryPrintViewModel() {
            //Console.WriteLine(DateTime.Now.Month.ToString());
            TodayTest = DateTime.Now.ToString("yyMMdd");
            MonthReturn();
            // <!--5023G260001[50=수량,23=년도,G=월,26=일,0001=Page]-->
            IssueNumber = Count + DateTime.Now.ToString("yy") + MonthParse + DateTime.Now.Day.ToString() + string.Format("{0:D4}", IntNum);
            IssueBarcode = ModelSerials.Replace("-","") + "  " + IssueNumber;
            btnPrinterPage = new Command(PrintBtn);
            
        }
        #region 출력전 미리보기 테스트용
        //private void PrintPreview(object parameter) {
        //    PrintDialog printDialog = new PrintDialog();
        //    if (printDialog.ShowDialog() == true) {
        //        // TempLabelView를 프린트합니다.
        //        PrintBaseVIew tempLabelView = new PrintBaseVIew();
        //        FixedDocument fixedDocument = PrintDocument(tempLabelView, printDialog.PrintQueue);

        //        // XPS 문서 생성
        //        XpsDocument xpsDocument = new XpsDocument("PrintBaseVIew.xps", FileAccess.ReadWrite);
        //        XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
        //        xpsWriter.Write(fixedDocument);

        //        // 미리보기 화면 생성
        //        DocumentViewer documentViewer = new DocumentViewer();
        //        documentViewer.Document = xpsDocument.GetFixedDocumentSequence();

        //        // 미리보기 창 열기
        //        Window previewWindow = new Window {
        //            Content = documentViewer,
        //            Width = 800,
        //            Height = 600,
        //            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        //            Title = "Print Preview"
        //        };
        //        previewWindow.ShowDialog();

        //        // XPS 문서 닫기
        //        xpsDocument.Close();
        //    }
        //}

        //private FixedDocument PrintDocument(UserControl visual, PrintQueue printQueue) {
        //    double width = ConvertToPixels(21); // A4 용지 폭 (단위: cm)
        //    double height = ConvertToPixels(29.7); // A4 용지 높이 (단위: cm)

        //    // FixedPage 생성
        //    FixedPage fixedPage = new FixedPage();
        //    fixedPage.Width = width;
        //    fixedPage.Height = height;

        //    // UserControl을 FixedPage에 추가
        //    PageContent pageContent = new PageContent();
        //    FixedPage.SetLeft(visual, 0);
        //    FixedPage.SetTop(visual, 0);
        //    visual.Width = width;
        //    visual.Height = height;
        //    fixedPage.Children.Add(visual);

        //    ((IAddChild)pageContent).AddChild(fixedPage);

        //    // FixedDocument 생성
        //    FixedDocument fixedDocument = new FixedDocument();
        //    fixedDocument.Pages.Add(pageContent);

        //    return fixedDocument;
        //}
        #endregion
        #region 일단 출력되는로직

        // cm를 px로 변환하는 메서드
        private double ConvertToPixels(double cm) {
            double dpi = 96; // WPF의 기본 DPI 값
            return cm / 2.54 * dpi;
        }
        
        private void PrintBtn(object obj) {

            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                print = new PrintDialog();
                if (print.ShowDialog() == true) {

                    PrintBaseVIew temporary = new PrintBaseVIew();
                    //PrintVisual(temporary, print.PrintQueue);
                    PrintToPDF(temporary);
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }
        private void PrintToPDF(UserControl visual) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {

                // 뷰의 크기 및 회전 설정
                double width = ConvertToPixels(14); // A4 용지 높이 (단위: cm)
                double height = ConvertToPixels(16); // A4 용지 폭 (단위: cm)

                visual.Width = width;  // 가로와 세로를 반전하여 출력
                visual.Height = height;
                //visual.LayoutTransform = new RotateTransform(-90); // 시계 방향으로 90도 회전

                // FixedPage 생성 및 설정
                FixedPage fixedPage = new FixedPage();
                fixedPage.Width = width;
                fixedPage.Height = height;

                // UserControl을 FixedPage에 추가 및 가운데 정렬
                PageContent pageContent = new PageContent();
                FixedPage.SetLeft(visual, (width - visual.Width) / 2);
                FixedPage.SetTop(visual, (height - visual.Height) / 2);
                fixedPage.Children.Add(visual);

                ((IAddChild)pageContent).AddChild(fixedPage);

                // FixedDocument 생성
                FixedDocument fixedDocument = new FixedDocument();
                fixedDocument.Pages.Add(pageContent);

                // 프린터 설정
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true) {
                    // 프린터 드라이버 선택 (예: Microsoft Print to PDF)
                    PrintQueue printQueue = new PrintQueue(new PrintServer(), "OneNote for Windows 10");

                    // XPS 문서 작성
                    XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printQueue);
                    writer.Write(fixedDocument);
                }
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }

        }

        /*
        private void PrintVisual(UserControl visual, PrintQueue printQueue) {
            // 용지 크기 설정 (가로 방향)
            double width = ConvertToPixels(21); // A4 용지 높이 (단위: cm)
            double height = ConvertToPixels(29.7); // A4 용지 폭 (단위: cm)

            // 뷰의 크기를 용지 크기에 맞게 조정
            visual.Width = height;  // 가로와 세로를 반전하여 출력
            visual.Height = width;
            visual.LayoutTransform = new RotateTransform(-90); // 시계 방향으로 90도 회전

            // FixedPage 생성
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = width;
            fixedPage.Height = height;

            // UserControl을 FixedPage에 추가 및 가운데 정렬
            PageContent pageContent = new PageContent();
            FixedPage.SetLeft(visual, (width - visual.Width) / 2);
            FixedPage.SetTop(visual, (height - visual.Height) / 2);
            fixedPage.Children.Add(visual);

            ((IAddChild)pageContent).AddChild(fixedPage);

            // FixedDocument 생성
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.Pages.Add(pageContent);

            // 프린터 설정
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true) {
                PrintTicket printTicket = printDialog.PrintTicket;
                printTicket.PageMediaSize = new PageMediaSize(width, height);

                // XPS 문서 작성
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printQueue);
                writer.Write(fixedDocument);
            }
        }
        */
        #endregion
        

        public ICommand btnPrinterPage { get; set; }

        private string _aground = "진천";
        public string Aground {
            get { return _aground; }
            set {
                _aground = value;
                RaisePropertyChanged(nameof(Aground));
            }
        }

        private string _companyName = "R7A8 ㈜현대모비스";
        public string CompanyName {
            get { return _companyName; }
            set {
                _companyName = value;
                RaisePropertyChanged(nameof(CompanyName));
            }
        }
        //발행번호 + 발행번호 데이터 (바코드 화 -엑셀기준Free 3 of 9)
        private string _issueBarcode;
        public string IssueBarcode {
            get { return _issueBarcode; }
            set {
                _issueBarcode = "*"+value+"*";
                RaisePropertyChanged(nameof(IssueBarcode));
            }
        }

        private string _modelName = "CAMERA ASSY-BACK VIEW";
        public string ModelName {
            get { return _modelName; }
            set {
                _modelName = value;
                RaisePropertyChanged(nameof(ModelName));
            }
        }

        private string _modelSerial = "95760-C9500";
        public string ModelSerials {
            get { return _modelSerial; }
            set {
                _modelSerial = value;
                RaisePropertyChanged(nameof(ModelSerial));
            }
        }

        private int _intNum = 0;
        public int IntNum {
            get { return _intNum; }
            set {
                _intNum = value;
                RaisePropertyChanged(nameof(IntNum));
            }
        }
        private string _strNum;
        public string StrNum {
            get { return _strNum; }
            set {
                _strNum = value;
                RaisePropertyChanged(nameof(StrNum));
            }
        }

        private string _todayTest;
        public string TodayTest {
            get { return _todayTest; }
            set {
                _todayTest = value;
                RaisePropertyChanged(nameof(TodayTest));
            }
        }

        private string _issueNumber;
        public string IssueNumber {
            get { return _issueNumber; }
            set {
                _issueNumber = value;
                RaisePropertyChanged(nameof(_issueNumber));
            }
        }

        private string _count = "50";
        public string Count {
            get { return _count; }
            set {
                _count = value;
                RaisePropertyChanged(nameof(Count));
            }
        }

        private string _monthParse ;
        public string MonthParse {
            get { return _monthParse; }
            set {
                _monthParse = value;
                RaisePropertyChanged(nameof(MonthParse));
            }
        }

        public void MonthReturn () {
            char[] characters = new char[12];
            for (int i = 0; i < 12; i++) {
                characters[i] = (char)(65 + i); // 65부터 A, 66부터 B, ..., 76부터 L
            }
            for (int i = 1; i <= 12; i++) {
                char mappedValue = characters[i - 1];
                //Console.WriteLine($"{i} = {mappedValue}");
                if (DateTime.Now.Month.ToString() == i.ToString()) {
                    MonthParse = mappedValue.ToString();
                }
            }
        }
    }
}