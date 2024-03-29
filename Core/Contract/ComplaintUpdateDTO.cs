﻿using Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contract
{
    public class ComplaintUpdateDTO
    {
        public int ComplaintId { get; set; }
        [Required]
        [MaxLength(600)]
        public string? ComplaintText { get; set; }
        public string? Url { get; set; }
        public ICollection<Demand>? Demands { get; set; }

        public bool isApproved { get; set; }
    }
}
