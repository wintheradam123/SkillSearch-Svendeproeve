﻿using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Models;

// ReSharper disable StringLiteralTypo

namespace Shared
{
    public class Context : DbContext
    {
        private readonly IEncryptionProvider _provider;

        public Context()
        {
            //TODO: Move this to secrets
            _provider = new GenerateEncryptionProvider("3c80c9d4a2a75b05441e677f5d276933");
        }

        public Context(DbContextOptions<Context> options) : base(options)
        {
            _provider = new GenerateEncryptionProvider("3c80c9d4a2a75b05441e677f5d276933");
        }

        private readonly string _connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=SkillSearchDB;Trusted_Connection=True;MultipleActiveResultSets=true";

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(_connectionString);
            //dbContextOptionsBuilder.UseSqlServer(
            //    @"Server=CPH00301;Database=SkillSearch;Integrated Security=True;TrustServerCertificate=True");
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
        }
    }
}