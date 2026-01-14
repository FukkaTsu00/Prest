using GestionPrestation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionPrestation.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Application Tables ---
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contrat> Contrats { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<Paiement> Paiements { get; set; }
        public DbSet<Prestation> Prestations { get; set; }
        public DbSet<Prestataire> Prestataires { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Societe> Societes { get; set; }
        public DbSet<TypeDocument> TypeDocuments { get; set; }
        // --------------------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.ApplicationUser)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestataire>()
                .HasOne(p => p.ApplicationUser)
                .WithOne(u => u.Prestataire)
                .HasForeignKey<Prestataire>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestataire>()
                .Property(p => p.TarifHoraire)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Societe>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Societe)
                .HasForeignKey<Societe>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Societe>()
                .HasMany(s => s.Services)
                .WithOne(s => s.Societe)
                .HasForeignKey(s => s.IdSociete)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}