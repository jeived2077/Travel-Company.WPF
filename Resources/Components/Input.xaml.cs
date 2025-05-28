using System;
using System.Windows;
using System.Windows.Controls;

namespace Travel_Company.WPF.Resources.Components;

/// <summary>
/// Interaction logic for Input.xaml
/// </summary>
public partial class Input : UserControl
{
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set
        {
            SetValue(TextProperty, value);
        }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(Input), new PropertyMetadata(""));

    public string? ComponentName { get; set; }

    protected override void OnInitialized(EventArgs e)
    {
        InitializeComponent();
        Placeholder.Text = ComponentName;
        base.OnInitialized(e);
    }

    private void Input_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ComponentInput.Text.Length > 0)
        {
            Placeholder.Text = string.Empty;
            return;
        }
        Placeholder.Text = ComponentName;
    }
}
