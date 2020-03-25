using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AiOffer.Dbo.Offers
{
    public partial class OfferContext : DbContext
    {
        public OfferContext()
        {
        }

        public OfferContext(DbContextOptions<OfferContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Offer> Offer { get; set; }
        public virtual DbSet<OfferCandidate> OfferCandidate { get; set; }
        public virtual DbSet<OfferSkill> OfferSkill { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>(entity =>
            {
                entity.ToTable("OFFER");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IdCompany)
                    .HasColumnName("id_company")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("enum('TRAINEE','JUNIOR','SENIOR','OTHER')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("enum('PART-TIME','FULL-TIME','OTHER')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Wage)
                    .HasColumnName("wage")
                    .HasColumnType("decimal(10,0)");
            });

            modelBuilder.Entity<OfferCandidate>(entity =>
            {
                entity.HasKey(e => new { e.Offer, e.Candidate })
                    .HasName("PRIMARY");

                entity.ToTable("OFFER_CANDIDATE");

                entity.HasIndex(e => e.Offer)
                    .HasName("OFFER_CANDIDATE_key_offer");

                entity.Property(e => e.Offer)
                    .HasColumnName("offer")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Candidate)
                    .HasColumnName("candidate")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.OfferNavigation)
                    .WithMany(p => p.OfferCandidate)
                    .HasForeignKey(d => d.Offer)
                    .HasConstraintName("OFFER_CANDIDATE_fk_candidate");
            });

            modelBuilder.Entity<OfferSkill>(entity =>
            {
                entity.HasKey(e => new { e.Offer, e.Skill })
                    .HasName("PRIMARY");

                entity.ToTable("OFFER_SKILL");

                entity.HasIndex(e => e.Offer)
                    .HasName("OFFER_SKILL_key_offer");

                entity.Property(e => e.Offer)
                    .HasColumnName("offer")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Skill)
                    .HasColumnName("skill")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.OfferNavigation)
                    .WithMany(p => p.OfferSkill)
                    .HasForeignKey(d => d.Offer)
                    .HasConstraintName("OFFER_SKILL_fk_candidate");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
