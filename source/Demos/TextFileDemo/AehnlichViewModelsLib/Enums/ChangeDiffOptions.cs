namespace AehnlichViewModelsLib.Enums
{
    [System.Flags]
    public enum ChangeDiffOptions
    {
        None = 0,
        IgnoreCase = 1,
        IgnoreWhitespace = 2,
        IgnoreBinaryPrefix = 4
    }
}
