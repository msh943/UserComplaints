using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }
        [Required]
        [MaxLength(600)]
        public string? ComplaintText { get; set; }
        public string? AttachmentPath { get; set; }
        public ICollection<Demand>? Demands { get; set; }

        public bool isApproved { get; set; }
    }
}
