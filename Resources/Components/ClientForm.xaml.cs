using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.MVVM.ViewModel.Clients;
using Travel_Company.WPF.Resources.Localizations;

namespace Travel_Company.WPF.Resources.Components
{
    public partial class ClientForm : UserControl
    {
        public FormState State { get; set; }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("RelayCommand", typeof(RelayCommand), typeof(ClientForm));

        public RelayCommand RelayCommand
        {
            get => (RelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnInitialized(EventArgs e)
        {
            InitializeComponent();
            InitializeForm();
            EnsurePassportInitialized(); // Добавлено для предотвращения потери данных
            base.OnInitialized(e);
        }

        private void EnsurePassportInitialized()
        {
            if (DataContext is ClientsCreateViewModel vm && vm.Client.Person.Passport == null)
            {
                vm.Client.Person.Passport = new Passport { PassportIssueDate = DateTime.Now.AddYears(-18) };
                //vm.OnPropertyChanged(nameof(vm.Client)); // Уведомляем об изменении
            }
        }

        private void InitializeForm()
        {
            switch (State)
            {
                case FormState.Updating:
                    InitializeUpdating();
                    break;
                case FormState.Inserting:
                    InitializeInserting();
                    break;
            }
        }

        private void InitializeUpdating()
        {
            TextBlockTitle.Text = LocalizedStrings.Instance["EditClientHeader"];
            ButtonProceed.Content = LocalizedStrings.Instance["ButtonUpdate"];
        }

        private void InitializeInserting()
        {
            TextBlockTitle.Text = LocalizedStrings.Instance["CreateClientHeader"];
            ButtonProceed.Content = LocalizedStrings.Instance["ButtonCreate"];
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedValue != null)
            {
                Debug.WriteLine($"Selected StreetId: {comboBox.SelectedValue}");
            }
            else
            {
                Debug.WriteLine("No Street selected or SelectedValue is null.");
            }
        }
    }
}