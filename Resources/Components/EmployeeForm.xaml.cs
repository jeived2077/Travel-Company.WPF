using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Core.Enums;
using Travel_Company.WPF.Resources.Localizations;

namespace Travel_Company.WPF.Resources.Components;

/// <summary>
/// Interaction logic for EmployeeForm.xaml
/// </summary>
public partial class EmployeeForm : UserControl
{
    public FormState State { get; set; }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("RelayCommand", typeof(RelayCommand), typeof(EmployeeForm));

    public RelayCommand RelayCommand
    {
        get => (RelayCommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
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
        TextBlockTitle.Text = LocalizedStrings.Instance["EditEmployeeHeader"];
        ButtonProceed.Content = LocalizedStrings.Instance["ButtonUpdate"];
        SetCommand("UpdateEmployeeCommand");
    }

    private void InitializeInserting()
    {
        TextBlockTitle.Text = LocalizedStrings.Instance["CreateEmployeeHeader"];
        ButtonProceed.Content = LocalizedStrings.Instance["ButtonCreate"];
        SetCommand("CreateEmployeeCommand");
    }

    private void SetCommand(string commandName)
    {
        Binding binding = new()
        {
            Path = new PropertyPath(commandName)
        };
        ButtonProceed.SetBinding(CommandProperty, binding);
    }
}