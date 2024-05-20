using ForaLending.API.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ForaLending.API.Data;

public class ForaFinancialContext : DbContext
{
    public ForaFinancialContext(DbContextOptions<ForaFinancialContext> options) : base(options) { }

    public DbSet<Company> Companies { get; set; }
    public DbSet<IncomeRecord> IncomeRecords { get; set; }
}