namespace AehnlichViewLib.Interfaces
{
    using AehnlichViewLib.Enums;

    public interface IDiffLineInfo
    {
        DiffContext Context      { get; }

        int? ImaginaryLineNumber { get; }
    }
}