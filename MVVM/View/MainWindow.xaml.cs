using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Travel_Company.WPF.MVVM.View
{
    public partial class MainWindow : Window
    {
        private bool isSidebarOpen = false;
        private bool isMaximized = false;

        public MainWindow()
        {
            InitializeComponent();
            UpdateSidebarState();
            ButtonMaximizeRestore.Template = (ControlTemplate)FindResource("MaximizeButton"); // Начальное состояние
        }

        private void UpdateSidebarState()
        {
            SidebarColumnDefinitions.Width = isSidebarOpen ? new GridLength(250) : new GridLength(78);
            LogoIcon.Opacity = 1;
            LogoText.Opacity = isSidebarOpen ? 1 : 0;
            ToggleIcon.Data = isSidebarOpen ? (System.Windows.Media.Geometry)FindResource("GeoMenuAltRight") : (System.Windows.Media.Geometry)FindResource("GeoMenu");

            EmployeesText.Opacity = isSidebarOpen ? 1 : 0;
            ReportsText.Opacity = isSidebarOpen ? 1 : 0;
            TourOperatorsText.Opacity = isSidebarOpen ? 1 : 0;
            ClientsText.Opacity = isSidebarOpen ? 1 : 0;
            RoutesText.Opacity = isSidebarOpen ? 1 : 0;
            PaymentsText.Opacity = isSidebarOpen ? 1 : 0;
            TouristGroupsText.Opacity = isSidebarOpen ? 1 : 0;
            PenaltiesText.Opacity = isSidebarOpen ? 1 : 0;
            CountriesText.Opacity = isSidebarOpen ? 1 : 0;
            StreetsText.Opacity = isSidebarOpen ? 1 : 0;
            HotelsText.Opacity = isSidebarOpen ? 1 : 0;
            PopulatedPlacesText.Opacity = isSidebarOpen ? 1 : 0;

            PagesHeader.Opacity = isSidebarOpen ? 1 : 0;
            CatalogsHeader.Opacity = isSidebarOpen ? 1 : 0;
        }

        private void BorderTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (isMaximized)
                {
                    WindowState = WindowState.Normal;
                    isMaximized = false;
                    ButtonMaximizeRestore.Template = (ControlTemplate)FindResource("MaximizeButton");
                }
                DragMove();
            }
        }

        private void ButtonClose_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMinimize_Clicked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ButtonMaximizeRestore_Clicked(object sender, RoutedEventArgs e)
        {
            if (!isMaximized)
            {
                WindowState = WindowState.Maximized;
                isMaximized = true;
                ButtonMaximizeRestore.Template = (ControlTemplate)FindResource("RestoreButton");
            }
            else
            {
                WindowState = WindowState.Normal;
                isMaximized = false;
                ButtonMaximizeRestore.Template = (ControlTemplate)FindResource("MaximizeButton");
            }
        }

        private void Tg_Btn_Click(object sender, RoutedEventArgs e)
        {
            isSidebarOpen = Tg_Btn.IsChecked == true;
            UpdateSidebarState();
        }

        private void ButtonEmployees_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonEmployees;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Employees";
            }
        }

        private void ButtonEmployees_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonReports_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonReports;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Reports";
            }
        }

        private void ButtonReports_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonTourOperators_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonTourOperators;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Tour Operators";
            }
        }

        private void ButtonTourOperators_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonClients_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonClients;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Clients";
            }
        }

        private void ButtonClients_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonRoutes_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonRoutes;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Routes";
            }
        }

        private void ButtonRoutes_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonPayments_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonPayments;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Payments";
            }
        }

        private void ButtonPayments_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonsTouristGroups_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonsTouristGroups;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Groups";
            }
        }

        private void ButtonsTouristGroups_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonsPenalties_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonsPenalties;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Penalties";
            }
        }

        private void ButtonsPenalties_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonsCountries_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonsCountries;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Countries";
            }
        }

        private void ButtonsCountries_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonsStreets_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonsStreets;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Streets";
            }
        }

        private void ButtonsStreets_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonsHotels_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonsHotels;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Hotels";
            }
        }

        private void ButtonsHotels_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ButtonsPopulatedPlaces_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isSidebarOpen)
            {
                Popup.PlacementTarget = ButtonsPopulatedPlaces;
                Popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                Popup.IsOpen = true;
                PopupText.Text = "Places";
            }
        }

        private void ButtonsPopulatedPlaces_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }
    }
}