namespace Comic.API.Code.Extensions
{
    public static class Exensions
    {
        public static int? GetId(this object obj, int? defaultId = null)
        {
            if (obj == null) return defaultId;

            if (int.TryParse(obj.ToString(), out int value))
                return value as int?;

            return defaultId;
        }
    }
}
