using System.Globalization;

namespace PopulationModels.UI.Resources;

public static class LocalizationHelper
{
    public static bool CheckLocale(string localeName)
    {
        if (Localization.Culture == null)
            return CultureInfo.CurrentCulture.Equals(new CultureInfo(localeName));
        return Localization.Culture.Equals(new CultureInfo(localeName));
    }

    public static void ChangeLocalization(string localeName, Action? afterChangeAction = null)
    {
        if (!CheckLocale(localeName))
        {
            Localization.Culture = new CultureInfo(localeName);
            afterChangeAction?.Invoke();
        }
    }
}