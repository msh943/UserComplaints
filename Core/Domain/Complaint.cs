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
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public ICollection<Demand>? Demands { get; set; }

        public int isApproved { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
    }
}
