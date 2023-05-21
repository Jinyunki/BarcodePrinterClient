using System.Windows;
using System.Windows.Input;

namespace Leak_UI {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        

        public MainWindow() {
            InitializeComponent();
            
        }

        #region ViewDesign
        // Window Minimize
        private void btnMinimize_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        // Window Size 
        private void btnSize_Click(object sender, RoutedEventArgs e) {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        #endregion

    }
}
