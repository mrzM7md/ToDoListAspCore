using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TO_DO_LIST.Models;

namespace TO_DO_LIST.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        public DbSet<TasksCategories> TasksCategories { get; set; }

    }
}