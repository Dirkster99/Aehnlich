namespace DiffLibViewModels.Enums
{
    [System.Flags]
    internal enum ChangeDiffOptions
    {
        None = 0,
        IgnoreCase = 1,
        IgnoreWhitespace = 2,
        IgnoreBinaryPrefix = 4
    }
}
