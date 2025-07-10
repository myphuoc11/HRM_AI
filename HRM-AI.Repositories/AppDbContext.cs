using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using HRM_AI.Repositories.Entities;
using AutoMapper.Features;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using StackExchange.Redis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HRM_AI.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Fix for PostgreSQL timestamp with time zone issues
            // AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            // AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Entity Properties Configuration

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(account => account.FirstName).HasMaxLength(50);
                entity.Property(account => account.LastName).HasMaxLength(50);
                entity.Property(account => account.Username).HasMaxLength(50);
                entity.Property(account => account.Email).HasMaxLength(256);
                entity.Property(account => account.PhoneNumber).HasMaxLength(15);
                entity.HasIndex(account => account.Username).IsUnique();
                entity.HasIndex(account => account.Email).IsUnique();
            });

            
            modelBuilder.Entity<Entities.Role>(entity =>
            {
                entity.Property(role => role.Name).HasMaxLength(50);
                entity.Property(role => role.Description).HasMaxLength(256);
                entity.HasIndex(role => role.Name).IsUnique();
            });

            #endregion

        }

        #region DbSets

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Entities.Role> Roles { get; set; }


        #endregion
    }
}
