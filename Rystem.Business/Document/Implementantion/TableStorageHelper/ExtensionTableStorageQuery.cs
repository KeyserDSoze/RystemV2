namespace Rystem
{
    public static class ExtensionTableStorageQuery
    {
        /// <summary>
        /// Controllo carattere per carattere se a è maggiore di b (come da dizionario)
        /// </summary>
        public static bool GreaterThan(this string a, string b)
        {
            int minimumLength = a.Length > b.Length ? b.Length : a.Length;
            for (int i = 0; i < minimumLength; i++)
            {
                if (a[i] < b[i])
                    return false;
                else if (a[i] > b[i])
                    return true;
            }
            return a.Length > b.Length;
        }
        /// <summary>
        /// Controllo carattere per carattere se a è minore di b (come da dizionario)
        /// </summary>
        public static bool LessThan(this string a, string b)
        {
            int minimumLength = a.Length > b.Length ? b.Length : a.Length;
            for (int i = 0; i < minimumLength; i++)
            {
                if (a[i] > b[i])
                    return false;
                else if (a[i] < b[i])
                    return true;
            }
            return a.Length < b.Length;
        }
        /// <summary>
        /// Controllo carattere per carattere se a è maggiore o uguale di b (come da dizionario)
        /// </summary>
        public static bool GreaterThanOrEqual(this string a, string b)
        {
            int minimumLength = a.Length > b.Length ? b.Length : a.Length;
            for (int i = 0; i < minimumLength; i++)
            {
                if (a[i] < b[i])
                    return false;
                else if (a[i] > b[i])
                    return true;
            }
            return a.Length >= b.Length;
        }
        /// <summary>
        /// Controllo carattere per carattere se a è minore o uguale di b (come da dizionario)
        /// </summary>
        public static bool LessThanOrEqual(this string a, string b)
        {
            int minimumLength = a.Length > b.Length ? b.Length : a.Length;
            for (int i = 0; i < minimumLength; i++)
            {
                if (a[i] > b[i])
                    return false;
                else if (a[i] < b[i])
                    return true;
            }
            return a.Length <= b.Length;
        }
    }
}
