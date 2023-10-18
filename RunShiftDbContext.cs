// ***********************************************************************
// Assembly         : 
// Author           : sudarshan
// Created          : 06-18-2023
//
// Last Modified By : sudarshan
// Last Modified On : 06-18-2023
// ***********************************************************************
// <copyright file="RunShiftDbContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace RunShift.DataAccess.Entity
{
    /// <summary>
    /// Class RunShiftDbContext.
    /// </summary>
    public class RunShiftDbContext : DbContext
    {       
        private readonly string? connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunShiftDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public RunShiftDbContext(DbContextOptions<RunShiftDbContext> options) : base(options)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            connectionString = root.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        }

        /// <summary>
        /// Called when [configuring].
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.UseSqlite(connectionString);
        }

       
        public DbSet<Role> Roles { get; set; }
        
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        public DbSet<Company> Companies { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Seed(modelBuilder);
        }

        
        private static void Seed(ModelBuilder modelBuilder)
        {
            Guid guIdArole = Guid.NewGuid();
            Guid guIdUrole = Guid.NewGuid();
            Guid guIdMuser = Guid.NewGuid();
            Guid guIdHuser = Guid.NewGuid();

            modelBuilder.Entity<Role>()
                .Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<Role>().HasData(
            new Role { Id = guIdArole, RoleName = "Admin" },
            new Role { Id = guIdUrole, RoleName = "User" });

            modelBuilder.Entity<ApplicationUser>()
                .Property(s => s.CreatedOn)
                .HasDefaultValue(DateTime.UtcNow);

            modelBuilder.Entity<ApplicationUser>()
                .Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ApplicationUser>()
                .Property(s => s.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<ApplicationUser>()
            .HasIndex(p => new { p.Email, p.Mobile }).IsUnique();
            //.HasIndex(p => new { p.FirstColumn, p.SecondColumn }).IsUnique();

            modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser { Id = guIdMuser, FirstName = "Mitul",LastName = "Sudra", Email = "mitul@fseven.co.uk", RoleId = guIdArole, Mobile = "+913214569870", Password = "D4E8E61F3BB30E106D5ED53A0246CE83" },
            new ApplicationUser { Id = guIdHuser, FirstName = "Hectar", LastName = "Salamanca",  Email = "hectar.salamanca@seveneightnine.com", RoleId = guIdUrole, Mobile = "+913214569870", Password = "D4E8E61F3BB30E106D5ED53A0246CE83" });

            modelBuilder.Entity<Company>()
                .Property(s => s.DateCreated)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Company>()
                .Property(s => s.CreatedOn)
                .HasDefaultValue(DateTime.UtcNow);

            modelBuilder.Entity<Company>()
                .Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<Company>()
                .Property(s => s.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Company>().HasData(
            new Company { Id = Guid.NewGuid(), Name = "Stripe", Url = "www.stripe.com", AddressLine1 = "Address Line 1", AddressTownCity = "London", AddressPostalCode = "N1", ApplicationUserId= guIdMuser },
            new Company { Id = Guid.NewGuid(), Name = "Tink", Url = "www.tink.com", AddressLine1 = "Address Line 1", AddressTownCity = "London", AddressPostalCode = "N1", ApplicationUserId = guIdMuser },
            new Company { Id = Guid.NewGuid(), Name = "TrueLayer", Url = "www.truelayer.com", AddressLine1 = "Address Line 1", AddressTownCity = "London", AddressPostalCode = "N1", ApplicationUserId = guIdHuser },
            new Company { Id = Guid.NewGuid(), Name = "Plaid", Url = "www.plaid.com", AddressLine1 = "Address Line 1", AddressTownCity = "London", AddressPostalCode = "N1", ApplicationUserId = guIdHuser });

        }
    }
}