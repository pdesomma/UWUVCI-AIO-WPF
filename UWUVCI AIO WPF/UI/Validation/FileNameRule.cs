using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace UWUVCI_AIO_WPF.UI.Validation
{
    public class GameNameRule : ValidationRule
    {
        public GameNameRule() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex reg = new Regex("^[a-zA-Z0-9 |_-]*$");
            if (reg.IsMatch(value.ToString()) && value.ToString().Length < 250)
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Invalid game name");
        }
    }
}
