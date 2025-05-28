using WPFLocalizeExtension.Engine;

namespace Travel_Company.WPF.Models;

public partial class TourGuide
{
    public string FullName =>
        !string.IsNullOrEmpty(Patronymic)
            ? $"{LastName} {FirstName} {Patronymic}"
            : $"{LastName} {FirstName}";

    public string IsFiredWithText
    {
        get
        {
            var culture = LocalizeDictionary.Instance.Culture.Name;
            if (IsFired == false && culture == "ru-RU")
            {
                return "Нет";
            }
            if (IsFired == true && culture == "ru-RU")
            {
                return "Да";
            }
            if (IsFired == false && culture == "en-US")
            {
                return "No";
            }
            return "Yes";
        }
    }
}

public partial class Client
{
    public string FullName =>
        !string.IsNullOrEmpty(Patronymic)
            ? $"{LastName} {FirstName} {Patronymic}"
            : $"{LastName} {FirstName}";

    public string Passport => $"{PassportSeries} {PassportNumber}";
}