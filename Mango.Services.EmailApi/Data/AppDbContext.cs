﻿using Mango.Services.EmailApi.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }


    }
}
