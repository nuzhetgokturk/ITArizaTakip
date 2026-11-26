
using Microsoft.EntityFrameworkCore;

namespace ITArizaTakip.Data
{
    public static class DepartmanSeeder
    {
        public static async Task DepartmanlariOlusturAsync(
            IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            string[] departmanlar =
            {
                "Bilgi İşlem",
                "İnsan Kaynakları",
                "Muhasebe",
                "Satın Alma",
                "Üretim",
                "Pazarlama",
                "Yönetim"
            };

            foreach (var ad in departmanlar)
            {
                bool varMi = await db.Departmanlar
                    .AnyAsync(d => d.Ad == ad);

                if (!varMi)
                {
                    db.Departmanlar.Add(new Departman
                    {
                        Ad = ad,
                        Aciklama = ad + " Departmanı",
                        AktifMi = true
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
