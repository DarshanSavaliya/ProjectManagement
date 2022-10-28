using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Data {
    public class DataContext: IdentityDbContext<ApplicationUser> {
        public DataContext(DbContextOptions<DataContext> options): base(options) {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.Entity<UserProject>(
                b => {
                    b.ToTable("UserProjects");
                }
            );
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskAssigned> Tasks { get; set; }
    }
}
