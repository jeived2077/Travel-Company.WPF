using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Travel_Company.WPF.MVVM.View
{
    public partial class MainWindow : Window
    {
        private bool isSidebarOpen = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BorderTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            isSidebarOpen = !isSidebarOpen;
            SidebarColumn.Width = isSidebarOpen ? new GridLength(250) : new GridLength(78);
            BorderMainMenu.DataContext = new { IsOpen = isSidebarOpen };
            LogoIcon.Opacity = isSidebarOpen ? 1 : 0;
            LogoText.Opacity = isSidebarOpen ? 1 : 0;
            SearchBox.Opacity = isSidebarOpen ? 1 : 0;
            ProfileText.Opacity = isSidebarOpen ? 1 : 0;
            ToggleIcon.Data = isSidebarOpen ? (Geometry)FindResource("GeoMenuAltRight") : (Geometry)FindResource("GeoMenu");
        }
    }
}