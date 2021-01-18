using Services.Interfaces.Indexing;

namespace Services.Implementation.Indexing
{
    public class ArticleRateStrategy : IRateStrategy
    {
        public int RateElement(string propertyName)
            => propertyName switch
            {
                "Header" => 5,
                "Author" => 5,
                "Series" => 3,
                "Topic" => 2,
                "Tags" => 2,
                "Text" => 1,
                _ => 0
            };
    }
}
