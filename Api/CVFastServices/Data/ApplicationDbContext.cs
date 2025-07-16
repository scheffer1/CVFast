using CVFastServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CVFastServices.Data
{
    /// <summary>
    /// Contexto do banco de dados da aplicação
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="options">Opções de configuração do contexto</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Usuários
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;
        
        /// <summary>
        /// Currículos
        /// </summary>
        public DbSet<Curriculum> Curriculums { get; set; } = null!;
        
        /// <summary>
        /// Experiências profissionais
        /// </summary>
        public DbSet<Experience> Experiences { get; set; } = null!;
        
        /// <summary>
        /// Formações acadêmicas
        /// </summary>
        public DbSet<Education> Educations { get; set; } = null!;
        
        /// <summary>
        /// Habilidades técnicas
        /// </summary>
        public DbSet<Skill> Skills { get; set; } = null!;

        /// <summary>
        /// Idiomas
        /// </summary>
        public DbSet<Language> Languages { get; set; } = null!;

        /// <summary>
        /// Contatos
        /// </summary>
        public DbSet<Contact> Contacts { get; set; } = null!;
        
        /// <summary>
        /// Endereços
        /// </summary>
        public DbSet<Address> Addresses { get; set; } = null!;
        
        /// <summary>
        /// Links curtos
        /// </summary>
        public DbSet<ShortLink> ShortLinks { get; set; } = null!;
        
        /// <summary>
        /// Logs de acesso
        /// </summary>
        public DbSet<AccessLog> AccessLogs { get; set; } = null!;

        /// <summary>
        /// Configuração do modelo
        /// </summary>
        /// <param name="modelBuilder">Builder do modelo</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Curriculum>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Summary).HasMaxLength(2000);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Curriculums)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Experience>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(255);

                entity.HasOne(e => e.Curriculum)
                    .WithMany(c => c.Experiences)
                    .HasForeignKey(e => e.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Education>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Institution).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Degree).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FieldOfStudy).IsRequired().HasMaxLength(255);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.HasOne(e => e.Curriculum)
                    .WithMany(c => c.Educations)
                    .HasForeignKey(e => e.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TechName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Proficiency).IsRequired();

                entity.HasOne(e => e.Curriculum)
                    .WithMany(c => c.Skills)
                    .HasForeignKey(e => e.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LanguageName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Proficiency).IsRequired();

                entity.HasOne(e => e.Curriculum)
                    .WithMany(c => c.Languages)
                    .HasForeignKey(e => e.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Value).IsRequired().HasMaxLength(255);
                entity.Property(e => e.IsPrimary).IsRequired();

                entity.HasOne(e => e.Curriculum)
                    .WithMany(c => c.Contacts)
                    .HasForeignKey(e => e.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Street).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Number).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Complement).HasMaxLength(255);
                entity.Property(e => e.Neighborhood).IsRequired().HasMaxLength(255);
                entity.Property(e => e.City).IsRequired().HasMaxLength(255);
                entity.Property(e => e.State).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.ZipCode).HasMaxLength(20);
                entity.Property(e => e.Type).IsRequired();

                entity.HasOne(e => e.Curriculum)
                    .WithMany(c => c.Addresses)
                    .HasForeignKey(e => e.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade ShortLink
            modelBuilder.Entity<ShortLink>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Hash).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.Hash).IsUnique();
                entity.Property(e => e.IsRevoked).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.Curriculum)
                    .WithMany(c => c.ShortLinks)
                    .HasForeignKey(e => e.CurriculumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<AccessLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IP).IsRequired().HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.AccessedAt).IsRequired();

                entity.HasOne(e => e.ShortLink)
                    .WithMany(s => s.AccessLogs)
                    .HasForeignKey(e => e.ShortLinkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
