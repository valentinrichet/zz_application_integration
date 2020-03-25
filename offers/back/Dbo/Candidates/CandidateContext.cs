using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AiOffer.Dbo.Candidates
{
    public partial class CandidateContext : DbContext
    {
        public CandidateContext()
        {
        }

        public CandidateContext(DbContextOptions<CandidateContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<CandidateSkill> CandidateSkill { get; set; }
        public virtual DbSet<Education> Education { get; set; }
        public virtual DbSet<Experience> Experience { get; set; }
        public virtual DbSet<Skill> Skill { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.ToTable("CANDIDATE");

                entity.HasIndex(e => e.Mail)
                    .HasName("USER_key_mail")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.HashedPassword)
                    .IsRequired()
                    .HasColumnName("hashed_password")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Mail)
                    .IsRequired()
                    .HasColumnName("mail")
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CandidateSkill>(entity =>
            {
                entity.HasKey(e => new { e.Candidate, e.Skill })
                    .HasName("PRIMARY");

                entity.ToTable("CANDIDATE_SKILL");

                entity.HasIndex(e => e.Candidate)
                    .HasName("CANDIDATE_SKILL_key_candidate");

                entity.HasIndex(e => e.Skill)
                    .HasName("CANDIDATE_SKILL_key_skill");

                entity.Property(e => e.Candidate)
                    .HasColumnName("candidate")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Skill)
                    .HasColumnName("skill")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.CandidateNavigation)
                    .WithMany(p => p.CandidateSkill)
                    .HasForeignKey(d => d.Candidate)
                    .HasConstraintName("CANDIDATE_SKILL_fk_candidate");

                entity.HasOne(d => d.SkillNavigation)
                    .WithMany(p => p.CandidateSkill)
                    .HasForeignKey(d => d.Skill)
                    .HasConstraintName("CANDIDATE_SKILL_fk_skill");
            });

            modelBuilder.Entity<Education>(entity =>
            {
                entity.ToTable("EDUCATION");

                entity.HasIndex(e => e.IdCandidate)
                    .HasName("EDUCATION_key_candidate");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IdCandidate)
                    .HasColumnName("id_candidate")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Level)
                    .IsRequired()
                    .HasColumnName("level")
                    .HasColumnType("enum('L1','L2','L3','M1','M2','D1','D2','+')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.IdCandidateNavigation)
                    .WithMany(p => p.Education)
                    .HasForeignKey(d => d.IdCandidate)
                    .HasConstraintName("EDUCATION_key_candidate");
            });

            modelBuilder.Entity<Experience>(entity =>
            {
                entity.ToTable("EXPERIENCE");

                entity.HasIndex(e => e.IdCandidate)
                    .HasName("EXPERIENCE_key_candidate");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.End)
                    .HasColumnName("end")
                    .HasColumnType("date");

                entity.Property(e => e.IdCandidate)
                    .HasColumnName("id_candidate")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Start)
                    .HasColumnName("start")
                    .HasColumnType("date");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.IdCandidateNavigation)
                    .WithMany(p => p.Experience)
                    .HasForeignKey(d => d.IdCandidate)
                    .HasConstraintName("EXPERIENCE_key_candidate");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("SKILL");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("tinytext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("enum('SOFT','TECHNICAL','OTHER')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
