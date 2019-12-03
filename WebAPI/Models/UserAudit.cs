using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class UserAudit
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string ActionName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }

        public DateTimeOffset ActionDate { get; set; }

        [ForeignKey("User")]
        public string userId { get; set; }
    }
}
