﻿using IotData.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotData.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Indicator> Indicators { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) =>
            Database.EnsureCreated();
    }
}
