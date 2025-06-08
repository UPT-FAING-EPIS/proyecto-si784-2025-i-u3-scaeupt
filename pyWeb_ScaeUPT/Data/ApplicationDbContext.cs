using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using pyWeb_ScaeUPT.Models;

namespace pyWeb_ScaeUPT.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<estudianteModel> estudiante { get; set; }
        public DbSet<tokenModel> token { get; set; }

    }
}
