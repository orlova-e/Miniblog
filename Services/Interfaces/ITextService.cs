namespace Services.Interfaces
{
    public interface ITextService
    {
        string GetEncoded(string text);
        object GetEncoded(object obj);
        string FixLines(string text);
        string GetPrepared(string text);
    }
}
