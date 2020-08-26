namespace Miniblog.Models.Entities
{
    public class JobOpportunities : Opportunities
    {
        public bool ModerateArticles { get; set; }
        public bool ModerateComments { get; set; }
        public bool ModerateTopics { get; set; }
        public bool ModerateTags { get; set; }
    }
}