namespace Domain.Entities
{
    public class ExtendedRole : Role
    {
        public bool CheckArticles { get; set; }
        public bool CheckComments { get; set; }
        public bool CheckTopics { get; set; }
        public bool CheckTags { get; set; }
        public bool OverrideMenu { get; set; }
    }
}
