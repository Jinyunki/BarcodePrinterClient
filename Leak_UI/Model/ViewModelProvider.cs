using GalaSoft.MvvmLight;
using Leak_UI.Utiles;
using System.ComponentModel;
using System.IO.Ports;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Reflection;
using System;

namespace Leak_UI.Model
{
    public class ViewModelProvider : ViewModelBase
    {
        
        public SerialPort serialPort1;
        public IDispatcher dispatcher;
        public ChromeDriverService driverService;
        public ChromeOptions options;
        public ChromeDriver driver ;

        public void DriverSet(ChromeDriverService _driverService, ChromeOptions _options) {
            Trace.WriteLine(TraceStart(MethodBase.GetCurrentMethod().Name));
            try {
                driver = new ChromeDriver(_driverService, _options);
            } catch (Exception e) {
                Trace.WriteLine(TraceCatch(MethodBase.GetCurrentMethod().Name) + e);
                throw;
            }
        }
        
        public string TraceStart(string methodName) {
            return "==========   Start   ==========\nMethodName : " + methodName + "\n";
        }
        public string TraceCatch(string methodName) {
            return "========== Exception ==========\nMethodName : " + methodName + "\nException : ";
        }
    }
}
