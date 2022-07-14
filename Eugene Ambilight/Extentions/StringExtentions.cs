namespace Eugene_Ambilight.Extentions
{
    public static class StringExtentions
    {
        public static int ToInt32(this string str, int defaultValue) 
            => int.TryParse(str, out int value) ? value : defaultValue;
    }
}
