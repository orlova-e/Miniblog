using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Entities
{
    public class RefreshToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Token { get; set; }
        public DateTimeOffset AccessTokenExpiration { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
