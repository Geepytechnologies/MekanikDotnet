using MekanikApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Infrastructure.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<JobRequest> JobRequests { get; set; }
        public DbSet<Mechanic> Mechanics { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<News> News { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<SafetyTip> SafetyTips { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<ServiceHistory> ServiceHistories { get; set; }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<VehicleBrand> VehicleBrands { get; set; }

        public DbSet<VehicleImage> VehicleImages { get; set; }

        public DbSet<Vendor> Vendors { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            // ApplicationUser




            // Conversation
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Participant)
                .WithMany(u => u.ParticipantConversations)
                .HasForeignKey(c => c.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.FromUser)
                .WithMany(u => u.FeedbacksSent) 
                .HasForeignKey(f => f.FromUserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.ToUser)
                .WithMany(u => u.FeedbacksReceived)
                .HasForeignKey(f => f.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // JobRequest
            modelBuilder.Entity<JobRequest>()
                .HasOne(jr => jr.Service)
                .WithMany()
                .HasForeignKey(jr => jr.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobRequest>()
                .HasOne(jr => jr.Vehicle)
                .WithMany()
                .HasForeignKey(jr => jr.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobRequest>()
                .HasOne(jr => jr.RequestedFrom)
                .WithMany()
                .HasForeignKey(jr => jr.RequestedFromId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobRequest>()
                .HasOne(jr => jr.RequestedFor)
                .WithMany()
                .HasForeignKey(jr => jr.RequestedForId)
                .OnDelete(DeleteBehavior.Restrict);

            // Mechanic
            modelBuilder.Entity<Mechanic>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Mechanic>()
                .HasMany(m => m.VehicleSpecialization)
                .WithMany(vb => vb.Mechanics);

            modelBuilder.Entity<Mechanic>()
                .HasMany(m => m.ServiceSpecialization)
                .WithMany(s => s.Mechanics);



            // Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification
            modelBuilder.Entity<Notification>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure cascading delete for images when the product is deleted


            // ServiceHistory
            modelBuilder.Entity<ServiceHistory>()
                .HasOne(sh => sh.Service)
                .WithMany()
                .HasForeignKey(sh => sh.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceHistory>()
                .HasOne(sh => sh.User)
                .WithMany()
                .HasForeignKey(sh => sh.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Order)
                .WithMany(o => o.Transactions)
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


           

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vendor
            modelBuilder.Entity<Vendor>()
                .HasMany(v => v.Products)
                .WithOne()
                .HasForeignKey("VendorId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vendor>()
                .HasOne<SubscriptionPlan>()
                .WithMany()
                .HasForeignKey(v => v.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
