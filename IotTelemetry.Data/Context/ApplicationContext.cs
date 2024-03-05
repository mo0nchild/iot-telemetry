using IotTelemetry.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotTelemetry.Data.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Indicator> Indicators { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) =>
            Database.EnsureCreated();
    }
}
