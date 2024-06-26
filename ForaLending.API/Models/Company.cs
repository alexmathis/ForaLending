﻿using ForaLending.API.Data;
using System.ComponentModel.DataAnnotations;

namespace ForaLending.API.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public  ICollection<IncomeRecord>? IncomeRecords { get; set; }
    }
}
