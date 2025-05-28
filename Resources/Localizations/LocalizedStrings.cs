using System.Globalization;
using WPFLocalizeExtension.Engine;

namespace Travel_Company.WPF.Resources.Localizations;

public class LocalizedStrings
{
    private LocalizedStrings()
    {

    }

    public static LocalizedStrings Instance { get; } = new LocalizedStrings();

    public void SetCulture(string cultureCode)
    {
        var newCulture = new CultureInfo(cultureCode);
        LocalizeDictionary.Instance.Culture = newCulture;
    }

    public string this[string key]
    {
        get
        {
            var result = LocalizeDictionary.Instance.GetLocalizedObject(
                "Travel-Company.WPF", "Strings", key, LocalizeDictionary.Instance.Culture);
            return result as string;
        }
    }
}
