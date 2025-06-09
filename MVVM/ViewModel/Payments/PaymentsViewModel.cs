using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Data.Base;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Services.Navigation;
using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace Travel_Company.WPF.MVVM.ViewModel.Payments;

public class PaymentsViewModel : Core.ViewModel
{
    private readonly IRepository<Payment, long> _paymentsRepository;
    private INavigationService _navigation;

    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public List<Payment> Payments { get; set; }
    public Payment? SelectedItem { get; set; }

    public RelayCommand SaveStatusCommand { get; }
    public RelayCommand ExportToPdfCommand { get; set; } = null!;
    public RelayCommand NavigateToInsertingCommand { get; set; } = null!;

    public PaymentsViewModel(IRepository<Payment, long> repository, INavigationService navigation)
    {
        _paymentsRepository = repository;
        Payments = FetchDataGridData();
        _navigation = navigation;

        SaveStatusCommand = new RelayCommand(
            execute: _ => SaveStatus(),
            canExecute: _ => SelectedItem != null);
        ExportToPdfCommand = new RelayCommand(
            execute: _ => ExportToPdf(),
            canExecute: _ => Payments.Any());
        NavigateToInsertingCommand = new RelayCommand(
            execute: _ => Navigation.NavigateTo<PaymentsCreateViewModel>(),
            canExecute: _ => true);
    }

    private List<Payment> FetchDataGridData()
    {
        return _paymentsRepository
            .GetQuaryable()
            .Include(x => x.Route)
            .Include(x => x.Client).ThenInclude(c => c.Person)
            .ToList();
    }

    private void SaveStatus()
    {
        if (SelectedItem != null)
        {
            _paymentsRepository.Update(SelectedItem);
            _paymentsRepository.SaveChanges();
            MessageBox.Show("Статус обновлен.", "Выполнение операции", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите платеж для обновления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ExportToPdf()
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF-документы (*.pdf)|*.pdf",
                FileName = $"Отчет_платежи_{DateTime.Now:yyyy-MM-dd}.pdf"
            };

            if (saveDialog.ShowDialog() != true) return;

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Times New Roman", 12);
            var titleFont = new XFont("Times New Roman", 14);

            gfx.DrawString("Отчет по платежам", titleFont, XBrushes.Black, new XRect(50, 50, page.Width, page.Height), XStringFormats.TopLeft);

            int y = 80;
            foreach (var payment in Payments)
            {
                var clientName = payment.Client?.Person?.FullName ?? "Не указан";
                var routeName = payment.Route?.Name ?? "Не указан";

                gfx.DrawString($"ID: {payment.Id}", font, XBrushes.Black, new XPoint(50, y));
                gfx.DrawString($"Клиент: {clientName}", font, XBrushes.Black, new XPoint(50, y + 20));
                gfx.DrawString($"Маршрут: {routeName}", font, XBrushes.Black, new XPoint(50, y + 40));
                gfx.DrawString($"Сумма: {payment.Amount} долл.", font, XBrushes.Black, new XPoint(50, y + 60));
                gfx.DrawString($"Дата: {payment.PaymentDate:dd.MM.yyyy}", font, XBrushes.Black, new XPoint(50, y + 80));
                gfx.DrawString($"Статус: {payment.Status}", font, XBrushes.Black, new XPoint(50, y + 100));
                gfx.DrawString($"Метод оплаты: {payment.PaymentMethod}", font, XBrushes.Black, new XPoint(50, y + 120));

                y += 140;
                if (y > 700)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 50;
                }
            }

            document.SecuritySettings.UserPassword = "admin";
            document.SecuritySettings.OwnerPassword = "admin";

            document.Save(saveDialog.FileName);
            MessageBox.Show("Данные успешно экспортированы в PDF.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}