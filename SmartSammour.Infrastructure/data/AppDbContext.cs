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

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Website", Description = "Custom website, built to spec", BasePrice = 300m },
                new Service { Id = 2, Name = "Mobile App", Description = "Cross-platform mobile app (iOS + Android)", BasePrice = 500m },
                new Service { Id = 3, Name = "Full Software Solution", Description = "Integrated website, mobile application, and backend/API solution", BasePrice = 700m }

            );

            modelBuilder.Entity<AddOn>().HasData(
                new AddOn { Id = 1, Name = "E-commerce functionality", ExtraPrice = 250m, ServiceId = 1 },
                new AddOn { Id = 2, Name = "Extra page (each)", ExtraPrice = 30m, ServiceId = 1 },
                new AddOn { Id = 3, Name = "Admin dashboard", ExtraPrice = 150m, ServiceId = 1 },
                new AddOn { Id = 4, Name = "Push notifications", ExtraPrice = 100m, ServiceId = 2 },
                new AddOn { Id = 5, Name = "In-app purchases", ExtraPrice = 200m, ServiceId = 2 },
                new AddOn { Id = 6, Name = "Admin dashboard", ExtraPrice = 150m, ServiceId = 3 },
                new AddOn { Id = 7, Name = "E-commerce functionality", ExtraPrice = 300m, ServiceId = 3 }
            );

            modelBuilder.Entity<Plan>().HasData(
                new Plan { Id = 1, Name = "Basic", Description = "The most competitive pricing in Jordan for a professionally built product. You get clean, working software — done right, without the extras. Ideal if you know exactly what you need and want it delivered efficiently."
                , ExtraFee = 0m, IncludeDomainAnalysis = false, IncludeHosting = false, IncludeDomainRegistration = false},
                new Plan { Id = 2, Name = "Professional", Description = "Domain requirement added to your requirement , built to a higher standard — plus domain setup, more thorough testing, and closer attention to detail throughout. The right choice if this product represents your business to real customers and needs to hold up under real use."
                , ExtraFee = 200m, IncludeDomainAnalysis = true, IncludeHosting = true, IncludeDomainRegistration = true},
                new Plan { Id = 3, Name = "Inclusive", Description = "Everything in Professional, but as one complete system — a backend API, a website, and a mobile app, all built together and designed to work as a single product. Best for businesses that want to launch across web and mobile from day one, without stitching together separate builds later."
                , ExtraFee = 500m, IncludeDomainAnalysis = true, IncludeHosting = true, IncludeDomainRegistration = true }
            );

            modelBuilder.Entity<PlanService>().HasData(
                new PlanService { PlanId = 1, ServiceId = 1 },
                new PlanService { PlanId = 1, ServiceId = 2 },
                new PlanService { PlanId = 2, ServiceId = 1 },
                new PlanService { PlanId = 2, ServiceId = 2 },
                new PlanService { PlanId = 2, ServiceId = 3 },
                new PlanService { PlanId = 3, ServiceId = 3 }
            );
        }

    }
}
