﻿using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

// ReSharper disable StringLiteralTypo

namespace Shared
{
    public class Context : DbContext
    {
        private readonly IEncryptionProvider _provider;

        public Context()
        {
            _provider = new GenerateEncryptionProvider("3c80c9d4a2a75b05441e677f5d276933");
        }

        public Context(DbContextOptions<Context> options) : base(options)
        {
            _provider = new GenerateEncryptionProvider("3c80c9d4a2a75b05441e677f5d276933");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(
                @"Server=CPH00301;Database=SvendeProve;Integrated Security=True;TrustServerCertificate=True");
            //dbContextOptionsBuilder.UseSqlServer(
            //    @"Server=10.22.24.204;Database=TeamFinder_Europe;User Id=tmfndr;Password=Flodhest13;TrustServerCertificate=True");
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Skill> Skills { get; set; }

        //public DbSet<SkillRequest> SkillRequests { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<OfficeLocation> OfficeLocations { get; set; }

        public DbSet<JobTitle> JobTitles { get; set; }
        //public DbSet<WhitelistUser> WhitelistUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(_provider);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Skills)
                .WithMany(e => e.Users);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Solutions)
                .WithMany(e => e.Users);

            //modelBuilder.Entity<User>()
            //    .Property(e => e.ImageSize)
            //    .HasDefaultValue(0);

            //modelBuilder.Entity<Solution>().HasData(new List<Solution>
            //{
            //    new() { Id = 1, Title = "Georg Jensen", Link = "https://www.georgjensen.com/" },
            //    new() { Id = 2, Title = "ECCO" },
            //    new() { Id = 3, Title = "Alustre" },
            //    new() { Id = 4, Title = "Region Midtjylland" },
            //    new() { Id = 5, Title = "Teamfinder" }
            //});

            //modelBuilder.Entity<Skill>().HasData(new List<Skill>
            //{
            //    new() { Id = 1, Title = ".NET", Category = "Developer" },
            //    new() { Id = 2, Title = "JavaScript", Category = "Developer" },
            //    new() { Id = 3, Title = "PHP", Category = "Developer" },
            //    new() { Id = 4, Title = "Java", Category = "Developer" },
            //    new() { Id = 5, Title = "TypeScript", Category = "Developer" },
            //    new() { Id = 6, Title = "React", Category = "Developer" }
            //});
        }
    }
}