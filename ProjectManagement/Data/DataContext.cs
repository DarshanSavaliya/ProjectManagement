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
                    b.HasKey(p => new { p.ProjectId, p.Member });
                }
            );
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskAssigned> Tasks { get; set; }
    }
}
