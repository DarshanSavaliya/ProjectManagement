using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Data {
    public class DataContext: IdentityDbContext {
        public DataContext(DbContextOptions<DataContext> options): base(options) {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.Entity<UserProject>(
                b => {
                    b.ToTable("UserProjects");
                    b.HasOne(x => x.User).WithMany().HasForeignKey("UserId");
                    b.HasOne(x => x.Project).WithMany().HasForeignKey("ProjectId");
                }
            );
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskAssigned> Tasks { get; set; }
    }
}
