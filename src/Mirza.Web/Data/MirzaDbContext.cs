using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mirza.Web.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Mirza.Web.Data
{
    public class MirzaDbContext : IdentityDbContext<MirzaUser, MirzaRole, int>
    {
        public MirzaDbContext(DbContextOptions<MirzaDbContext> options)
            : base(options)
        {
        }

        public DbSet<MirzaUser> UserSet { get; set; }
        public DbSet<MirzaTeam> TeamSet { get; set; }
        public DbSet<WorkLog> WorkLogSet { get; set; }
        public DbSet<AccessKey> AccessKeySet { get; set; }
        public DbSet<MirzaTenant> TenantSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            _ = modelBuilder
                .Entity<MirzaTenant>(b =>
                {
                    b.ToTable("Tenants");
                    b.HasKey(a => a.Id);

                    b.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                    b.HasMany(a => a.Members)
                    .WithOne(u => u.Tenant)
                    .HasForeignKey(u => u.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                    b.HasMany(a => a.Teams)
                    .WithOne(t => t.Tenant)
                    .HasForeignKey(t => t.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);

                })
                .Entity<MirzaTeam>(b =>
                {
                    b.ToTable("Teams");

                    b.HasKey(a => a.Id);

                    b.Property(a => a.Name)
                     .IsRequired()
                     .HasMaxLength(50);

                    b.HasOne(a => a.Tenant)
                    .WithMany(t => t.Teams)
                    .HasForeignKey(t => t.TenantId);

                    b.HasMany(a => a.Members)
                    .WithOne(a => a.Team)
                    .HasForeignKey(a => a.TeamId);
                })
                .Entity<MirzaUser>(b =>
                {
                    b.ToTable("Users");

                    b.HasKey(a => a.Id);

                    b.Property(a => a.FirstName)
                     .IsRequired()
                     .HasMaxLength(40);

                    b.Property(a => a.LastName)
                     .IsRequired()
                     .HasMaxLength(50);

                    b.Property(a => a.IsActive)
                     .IsRequired();

                    b.Property(a => a.Email)
                     .IsRequired();

                    b.HasOne(u => u.Tenant)
                    .WithMany(t => t.Members);

                    b.HasMany(u => u.Teams)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId);
                })
                .Entity<WorkLog>(b =>
                {
                    b.ToTable("WorkLogs");

                    b.HasKey(a => a.Id);

                    b.Property(a => a.LogDate)
                     .IsRequired()
                     .HasColumnType("datetime2");

                    b.Property(a => a.EntryDate)
                     .HasColumnType("date")
                     .IsRequired();

                    b.Property(a => a.StartTime)
                     .HasColumnType("time")
                     .IsRequired();

                    b.Property(a => a.EndTime)
                     .HasColumnType("time")
                     .IsRequired();

                    b.HasMany(a => a.Tags)
                     .WithOne(t => t.WorkLog)
                     .OnDelete(DeleteBehavior.Cascade);
                })
                .Entity<Tag>(b =>
                {
                    b.ToTable("Tags");
                    b.HasKey(a => a.Id);

                    b.HasOne(a => a.WorkLog)
                     .WithMany(w => w.Tags);

                    b.Property(a => a.Value)
                     .IsRequired()
                     .HasMaxLength(256);
                })
                .Entity<AccessKey>(b =>
                {
                    b.ToTable("AccessKeys");

                    b.HasKey(a => a.Id);

                    b.HasIndex(a => new { a.OwnerId, a.State })
                     .HasName("IDX_Owner_State");

                    b.Property(a => a.Key)
                     .IsRequired()
                     .HasColumnType("char(32)")
                     .IsFixedLength();

                    b.Property(a => a.State)
                     .IsRequired()
                     .HasConversion(new EnumToStringConverter<AccessKeyState>());

                    b.Property(a => a.Expiration)
                     .IsRequired()
                     .HasColumnType("datetime2");
                })
                .Entity<TeamUser>(b =>
                {
                    b.ToTable("TeamUser")
                    .HasKey(a => new { a.TeamId, a.UserId });
                });
        }
    }
}