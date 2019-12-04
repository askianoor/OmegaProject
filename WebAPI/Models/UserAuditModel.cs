using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class UserAuditModel
    {
        public string ActionName { get; set; }

        public string Status { get; set; }

        public DateTimeOffset ActionDate { get; set; }

        public string UserName { get; set; }
    }
}
