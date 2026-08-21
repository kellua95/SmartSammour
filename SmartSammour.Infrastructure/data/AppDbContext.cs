using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartSammour.Core.Entities;
using SmartSammour.Infrastructure.Identity;

namespace SmartSammour.Infrastructure.data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<AddOn> AddOns { get; set; } = null!;
        public DbSet<Inquiry> Inquiries { get; set; } = null!;
        public DbSet<InquiryAddOn> InquiryAddOn { get; set; } = null!;
        public DbSet<Plan> Plans { get; set; } = null!;
        public DbSet<PlanService> PlanServices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InquiryAddOn>()
                .HasKey(ia => new { ia.InquiryId, ia.AddOnId });

            modelBuilder.Entity<InquiryAddOn>()
                .HasOne(ia => ia.Inquiry)
                .WithMany(i => i.SelectedAddOns)
                .HasForeignKey(ia => ia.InquiryId);

            modelBuilder.Entity<InquiryAddOn>()
                .HasOne(ia => ia.AddOn)
                .WithMany()
                .HasForeignKey(ia => ia.AddOnId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inquiry>()
                .HasOne(i => i.Service)
                .WithMany()
                .HasForeignKey(i => i.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<PlanService>()
                .HasKey(ps => new { ps.PlanId, ps.ServiceId });

            modelBuilder.Entity<PlanService>()
                .HasOne(ps => ps.Plan)
                .WithMany(p => p.PlanServices)
                .HasForeignKey(ps => ps.PlanId);

            modelBuilder.Entity<PlanService>()
                .HasOne(ps => ps.Service)
                .WithMany(p => p.PlanServices)
                .HasForeignKey(ps => ps.ServiceId);

            modelBuilder.Entity<Service>().Property(s => s.BasePrice).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<AddOn>().Property(a => a.ExtraPrice).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Inquiry>().Property(i => i.EstimatedPrice).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Plan>().Property(p => p.ExtraFee).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Plan>().Property(p => p.StartFrom).HasColumnType("decimal(10,2)");

        }

    }
}
