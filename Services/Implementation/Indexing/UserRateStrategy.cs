using Services.Interfaces.Indexing;

namespace Services.Implementation.Indexing
{
    public class UserRateStrategy : IRateStrategy
    {
        public int RateElement(string propertyName)
            => propertyName switch
            {
                "Username" => 5,
                "Description" => 2,
                _ => 0
            };
    }
}
