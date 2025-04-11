using System.Globalization;

namespace GenesisApi.Utilities
{
    public class Extensions
    {
        public static double? ParseDouble(string value)
        {
            if (string.IsNullOrEmpty(value) || value == ".")
            {
                return null;
            }
            else if (double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
