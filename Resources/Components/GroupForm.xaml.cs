using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.MVVM.ViewModel.Groups;
using Travel_Company.WPF.Resources.Localizations;

namespace Travel_Company.WPF.Resources.Components
{
    public partial class GroupForm : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FormState State { get; set; }

        public GroupForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            switch (State)
            {
                case FormState.Updating:
                    TextBlockTitle.Text = LocalizedStrings.Instance["EditTouristGroupHeader"];
                    break;
                case FormState.Inserting:
                    TextBlockTitle.Text = LocalizedStrings.Instance["CreateTouristGroupHeader"];
                    break;
            }
        }
    }
}