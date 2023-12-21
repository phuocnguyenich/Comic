using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Comic.API.Code.Extensions
{
    public static class FormatExensions
    {
        public static string RemoveAccents(this string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.ToLower().Normalize(NormalizationForm.FormD);

            // Remove diacritics
            temp = regex.Replace(temp, string.Empty);

            // Replace specific characters
            temp = temp.Replace('\u0111', 'd').Replace('\u0110', 'D');

            // Replace non-alphanumeric characters with hyphens
            temp = Regex.Replace(temp, "[^a-z0-9]+", "-");

            // Remove leading and trailing hyphens
            temp = temp.Trim('-');

            return temp;
        }

        public static string FormatAsVietnameseNumber(this int number)
        {
            CultureInfo vietnameseCulture = new CultureInfo("vi-VN");
            return number.ToString("N0", vietnameseCulture);
        }

        public static string FormatAsVietnameseNumber(this long number)
        {
            CultureInfo vietnameseCulture = new CultureInfo("vi-VN");
            return number.ToString("N0", vietnameseCulture);
        }
    }
}
