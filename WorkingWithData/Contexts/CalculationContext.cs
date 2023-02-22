using Microsoft.EntityFrameworkCore;
using WorkingWithData.Models;

namespace WorkingWithData.Contexts
{
    public class CalculationContext : DbContext
    {
        public CalculationContext(DbContextOptions<CalculationContext> opts) : base(opts)
        {

        }

        public DbSet<Calculation> Calculations => Set<Calculation>();
    }
}
