using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace Leak_UI {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WindowChrome.SetWindowChrome(this, new WindowChrome {
                ResizeBorderThickness = new Thickness(8),
                CaptionHeight = 0
            });

            Style roundedWindowStyle = FindResource("RoundedWindowStyle") as Style;
            if (roundedWindowStyle != null) {
                Style = roundedWindowStyle;
            }
        }
    }
}
