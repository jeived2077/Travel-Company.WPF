using System.Windows;
using System.Windows.Input;

namespace Travel_Company.WPF.MVVM.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    #region General Buttons
    private void BorderTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void ButtonClose_Click(object sender, RoutedEventArgs e) => App.Current.Shutdown();

    private void ButtonMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    #endregion

    private void ButtonReports_Checked(object sender, RoutedEventArgs e)
    {

    }
}