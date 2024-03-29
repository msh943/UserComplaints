﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
    public class Demand
    {
        [Key]
        public int DemandId { get; set; }
        [Required]
        public string? Description { get; set; }
        [ForeignKey("ComplaintId")]
        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
    }
}
