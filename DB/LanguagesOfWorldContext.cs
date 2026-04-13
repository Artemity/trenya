using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace экзамка.DB;

public partial class LanguagesOfWorldContext : DbContext
{
    public LanguagesOfWorldContext()
    {
    }

    public LanguagesOfWorldContext(DbContextOptions<LanguagesOfWorldContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EthnicComposition> EthnicCompositions { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=WIN-TNT0KK312B5\\ARTEMITY; Database = LanguagesOfWorld; Integrated Security=True; Encrypt = false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryCode).HasName("PK__Countrie__5D9B0D2D9A05EA8B");

            entity.Property(e => e.CountryCode).ValueGeneratedNever();
            entity.Property(e => e.Capital).HasMaxLength(100);
            entity.Property(e => e.Continent).HasMaxLength(50);
            entity.Property(e => e.CountryName).HasMaxLength(100);
        });

        modelBuilder.Entity<EthnicComposition>(entity =>
        {
            entity.HasKey(e => e.CompositionId).HasName("PK__EthnicCo__B8E2333F0F6997E7");

            entity.ToTable("EthnicComposition");

            entity.Property(e => e.CompositionId).HasColumnName("CompositionID");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.EthnicCompositions)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ethnic_Country");

            entity.HasOne(d => d.LanguageCodeNavigation).WithMany(p => p.EthnicCompositions)
                .HasForeignKey(d => d.LanguageCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ethnic_Language");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageCode).HasName("PK__Language__8B8C8A35245FA080");

            entity.Property(e => e.LanguageCode).ValueGeneratedNever();
            entity.Property(e => e.LanguageGroup).HasMaxLength(100);
            entity.Property(e => e.LanguageName).HasMaxLength(100);
            entity.Property(e => e.WritingSystem).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C080C6CA5");

            entity.HasIndex(e => e.Login, "UQ__Users__5E55825B970AB195").IsUnique();

            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
