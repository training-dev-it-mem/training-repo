using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Train.Models.Identity;
using Train.Models;

namespace Train.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<BatchEmployees> BatchEmployees { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}