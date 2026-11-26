
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITArizaTakip.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ariza> Arizalar { get; set; }

        public DbSet<Departman> Departmanlar { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Personel ile departman arasındaki ilişki
            builder.Entity<ApplicationUser>()
                .HasOne(k => k.DepartmanBilgisi)
                .WithMany()
                .HasForeignKey(k => k.DepartmanId)
                .OnDelete(DeleteBehavior.SetNull);

            // Yeni kullanıcılar varsayılan olarak aktif olsun.
            builder.Entity<ApplicationUser>()
                .Property(k => k.AktifMi)
                .HasDefaultValue(true);
        }
    }
}
