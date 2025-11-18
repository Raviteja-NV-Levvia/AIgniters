using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using AIPoweredDefectManagementAssistant.Models;

namespace AIPoweredDefectManagementAssistant.Datalayer
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<Defect> Defects => Set<Defect>();
        public DbSet<TestStepDto> TestSteps => Set<TestStepDto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var floatArrayConverter = new ValueConverter<float[], string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<float[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<float>());

            modelBuilder.Entity<Defect>(b =>
            {
                b.HasKey(d => d.TestCaseId);
                b.Property(d => d.Title).IsRequired().HasMaxLength(500);
                b.Property(d => d.Module).HasMaxLength(200);
                b.Property(d => d.Status).HasMaxLength(100);

                b.Property(d => d.GeneratedDescription).HasColumnType("nvarchar(max)");
                b.Property(d => d.GeneratedStepsToReproduce).HasColumnType("nvarchar(max)");

                b.Property(d => d.Embedding)
                 .HasConversion(floatArrayConverter)
                 .HasColumnType("nvarchar(max)");

                // Note: FK property on TestStepDto is TestCaseId (string)
                b.HasMany(d => d.Steps)
                 .WithOne(s => s.Defect)
                 .HasForeignKey(s => s.TestCaseId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TestStepDto>(b =>
            {
                // TestCaseId is now the primary key (string)
                b.HasKey(s => s.Id);
                b.Property(s => s.TestStep).HasMaxLength(2000);
                b.Property(s => s.TestData).HasMaxLength(1000);
                b.Property(s => s.ExpectedResult).HasMaxLength(2000);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
