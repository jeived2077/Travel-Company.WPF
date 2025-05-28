using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Travel_Company.WPF.Core;
using Travel_Company.WPF.Models;
using Travel_Company.WPF.Data.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Travel_Company.WPF.MVVM.ViewModel.Penalties;
using Microsoft.EntityFrameworkCore.Metadata;
using Travel_Company.WPF.Services.Navigation;
using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Windows.Controls;

namespace Travel_Company.WPF.MVVM.ViewModel.Payments
{
    public class PaymentsViewModel: Core.ViewModel
    {
        private readonly IRepository<Models.Payments, long> _paymentsRepository;
        public RelayCommand NavigateToInsertingCommand { get; set; } = null!;
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

        public List<Models.Payments> Payments { get; set; }
        public Models.Payments SelectedItem { get; set; }

        public ICommand SaveStatusCommand { get; }
        public RelayCommand ExportToPdfCommand { get; set; } = null!; // Новая команда


        // private readonly IPaymentService _paymentService;

        public PaymentsViewModel(IRepository<Models.Payments, long> repository, INavigationService navigation)
        {
            _paymentsRepository = repository;
            //_paymentService = paymentService;
            Payments =  FetchDataGridData();
            _navigation = navigation;
            SaveStatusCommand = new RelayCommand(
            execute: _ => SaveStatus(),
            canExecute: _ => true);
            // Добавление команды экспорта в PDF
            ExportToPdfCommand = new RelayCommand(
                execute: _ => ExportToPdf(),
                canExecute: _ => true);
            NavigateToInsertingCommand = new RelayCommand(
          execute: _ => Navigation.NavigateTo<PaymentsCreateViewModel>(),
          canExecute: _ => true);
        }

        private List<Models.Payments> FetchDataGridData()
        {
            return _paymentsRepository
                    .GetQuaryable()
                    .Include(x => x.Route)
                    .ToList();
        }

        private void SaveStatus()
        {
            _paymentsRepository.Update(SelectedItem);
            _paymentsRepository.SaveChanges();

            MessageBox.Show("Статус обновлен.", "Выполнение операции" , MessageBoxButton.OK, MessageBoxImage.Information);

            //if (param is Payment payment)
            //{
            //    _paymentService.UpdateStatus(payment.Id, payment.Status);
            //    MessageBox.Show("Статус платежа обновлен");
            //}
        }

        private void ExportToPdf()
        {
            try
            {
                // Выбор пути сохранения файла
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF-документы (*.pdf)|*.pdf",
                    FileName = $"Отчет_платежи_{DateTime.Now:yyyy-MM-dd}.pdf"
                };

                if (saveDialog.ShowDialog() != true) return;

                // Создание PDF-документа
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Times New Roman", 12);
                var titleFont = new XFont("Times New Roman", 14);

                // Заголовок
                gfx.DrawString("Отчет по платежам", titleFont, XBrushes.Black, new XRect(50, 50, page.Width, page.Height), XStringFormats.TopLeft);

                // Поля таблицы
                int y = 80;
                foreach (var payment in Payments)
                {
                    var clientName = payment.Client?.FullName ?? "Не указан";
                    var routeName = payment.Route?.Name ?? "Не указан";

                    gfx.DrawString($"ID: {payment.Id}", font, XBrushes.Black, new XPoint(50, y));
                    gfx.DrawString($"Клиент: {clientName}", font, XBrushes.Black, new XPoint(50, y + 20));
                    gfx.DrawString($"Маршрут: {routeName}", font, XBrushes.Black, new XPoint(50, y + 40));
                    gfx.DrawString($"Сумма: {payment.Amount} долл.", font, XBrushes.Black, new XPoint(50, y + 60));
                    gfx.DrawString($"Дата: {payment.PaymentDate:dd.MM.yyyy}", font, XBrushes.Black, new XPoint(50, y + 80));
                    gfx.DrawString($"Статус: {payment.Status}", font, XBrushes.Black, new XPoint(50, y + 100));
                    gfx.DrawString($"Метод оплаты: {payment.PaymentMethod}", font, XBrushes.Black, new XPoint(50, y + 120));

                    y += 140;
                    if (y > 700) // Перенос на новую страницу
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 50;
                    }
                }

                // Сохранение файла
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
