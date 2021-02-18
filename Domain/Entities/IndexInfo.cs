using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class IndexInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Entity Entity { get; set; }
        public Guid FoundWordId { get; set; }
        public FoundWord FoundWord { get; set; }
        public int Rank { get; set; }
    }
}
