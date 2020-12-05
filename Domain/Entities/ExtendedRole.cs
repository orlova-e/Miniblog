namespace Domain.Entities
{
    public class ExtendedRole : Role
    {
        public bool ModerateArticles { get; set; }
        public bool ModerateComments { get; set; }
        public bool ModerateTopics { get; set; }
        public bool ModerateTags { get; set; }
        public bool OverrideMenu { get; set; }
    }
}
