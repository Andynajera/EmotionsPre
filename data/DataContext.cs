using Microsoft.EntityFrameworkCore;
using Classes;

namespace Data
{

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }
       
        public DbSet<Usuario>? Usuarios { get; set; }
        public DbSet<Evento>? Eventos { get; set; }
        public DbSet<Pago>? Pagoss { get; set; }
        public DbSet<OrderPro>? OrderPro { get; set; }
        public DbSet<Preset>? Presets { get; set; }
        public DbSet<Likes>? Likes { get; set; }
         public DbSet<Testimonio>? Testimonios { get; set; }

    }
    }

