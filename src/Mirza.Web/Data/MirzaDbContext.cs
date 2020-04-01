using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mirza.Web.Models;

namespace Mirza.Web.Data
{
    public class MirzaDbContext : DbContext
    {
        public MirzaDbContext(DbContextOptions<MirzaDbContext> options)
            : base(options)
        {
        }

        public DbSet<MirzaUser> UserSet { get; set; }
        public DbSet<Team> TeamSet { get; set; }
        public DbSet<WorkLog> WorkLogSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            _ = modelBuilder
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

                    b.HasMany(a => a.AccessKeys)
                    .WithOne(a => a.Owner)
                    .HasForeignKey(a => a.OwnerId);

                    b.HasMany(a => a.Teams)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId);

                    b.HasMany(a => a.WorkLog)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId);
                })
                .Entity<Team>(b =>
                {
                    b.ToTable("Teams");

                    b.HasKey(a => a.Id);

                    b.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                    b.HasMany(a => a.Members)
                    .WithOne(a => a.Team)
                    .HasForeignKey(a => a.TeamId);

                    b.HasMany(a => a.WorkLog)
                    .WithOne(a => a.Team)
                    .HasForeignKey(a => a.TeamId);
                })
                .Entity<UserTeam>(b =>
                {
                    b.ToTable("Team_Users");

                    b.HasKey(a => a.Id);

                    b.HasIndex(a => new { a.TeamId, a.UserId })
                    .IsUnique()
                    .HasName("IDX_Team_User");

                    b.HasOne(a => a.User)
                    .WithMany(a => a.Teams)
                    .HasForeignKey(a => a.UserId);

                    b.HasOne(a => a.Team)
                    .WithMany(a => a.Members)
                    .HasForeignKey(a => a.TeamId);

                })
                .Entity<WorkLog>(b =>
                {
                    b.ToTable("WorkLogs");

                    b.HasKey(a => a.Id);

                    b.Property(a => a.EntryDate)
                    .HasColumnType("date")
                    .IsRequired();

                    b.Property(a => a.StartTime)
                    .HasColumnType("time")
                    .IsRequired();

                    b.Property(a => a.EndTime)
                    .HasColumnType("time")
                    .IsRequired();
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

                    b.Property(a => a.Expriation)
                    .IsRequired()
                    .HasColumnType("datetime2");
                });
        }
    }
}
