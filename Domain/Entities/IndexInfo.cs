﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class IndexInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public Guid FoundWordId { get; set; }
        public FoundWord FoundWord { get; set; }
        [Required]
        public string EntityType { get; set; }
        public int Count { get; set; }
        public int Rank { get; set; }
    }
}