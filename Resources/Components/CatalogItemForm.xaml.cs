using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Resources.Localizations;

namespace Travel_Company.WPF.Resources.Components
{
    /// <summary>
    /// Interaction logic for CatalogItemForm.xaml
    /// </summary>
    public partial class CatalogItemForm : UserControl
    {
        public FormState State { get; set; }

        public static readonly DependencyProperty CatalogCommandProperty =
            DependencyProperty.Register("RelayCommand", typeof(RelayCommand), typeof(CatalogItemForm));

        public RelayCommand RelayCommand
        {
            get => (RelayCommand)GetValue(CatalogCommandProperty);
            set => SetValue(CatalogCommandProperty, value);
        }

        protected override void OnInitialized(EventArgs e)
        {
            InitializeComponent();
            InitializeForm();
            base.OnInitialized(e);
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
            TextBlockTitle.Text = LocalizedStrings.Instance["UpdateCatalogHeader"];
            ButtonProceed.Content = LocalizedStrings.Instance["ButtonCreate"];
            SetCommand("UpdateCommand");
        }

        private void InitializeInserting()
        {
            TextBlockTitle.Text = LocalizedStrings.Instance["CreateCatalogHeader"];
            ButtonProceed.Content = LocalizedStrings.Instance["ButtonCreate"];
            SetCommand("CreateCommand");
        }

        private void SetCommand(string commandName)
        {
            Binding binding = new()
            {
                Path = new PropertyPath(commandName)
            };
            ButtonProceed.SetBinding(CatalogCommandProperty, binding);
        }
    }
}
