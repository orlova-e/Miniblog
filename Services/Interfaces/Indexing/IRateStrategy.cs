namespace Services.Interfaces.Indexing
{
    public interface IRateStrategy
    {
        int RateElement(string propertyName);
    }
}
