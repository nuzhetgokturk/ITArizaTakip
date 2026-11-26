
using Microsoft.AspNetCore.Identity;

namespace ITArizaTakip.Data
{
    public static class RoleSeeder
    {
        public static async Task RolleriOlusturAsync(
            IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            // Rolleri oluştur
            string[] roller = { "Yönetici", "Personel" };

            foreach (var rol in roller)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    var rolSonucu = await roleManager.CreateAsync(
                        new IdentityRole(rol));

                    if (!rolSonucu.Succeeded)
                    {
                        throw new Exception(
                            string.Join(" ",
                                rolSonucu.Errors.Select(x => x.Description)));
                    }
                }
            }

            // Yönetici bilgilerini ortam değişkenlerinden al
            var email = Environment.GetEnvironmentVariable(
                "ITARIZA_ADMIN_EMAIL");

            var sifre = Environment.GetEnvironmentVariable(
                "ITARIZA_ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(sifre))
            {
                return;
            }

            // Yönetici hesabını bul
            var yonetici = await userManager.FindByEmailAsync(email);

            // Yönetici yoksa oluştur
            if (yonetici == null)
            {
                yonetici = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    AdSoyad = "Sistem Yöneticisi",
                    Departman = "Bilgi İşlem"
                };

                var sonuc = await userManager.CreateAsync(
                    yonetici, sifre);

                if (!sonuc.Succeeded)
                {
                    throw new Exception(
                        string.Join(" ",
                            sonuc.Errors.Select(x => x.Description)));
                }
            }

            // Yönetici rolünü yoksa ata
            if (!await userManager.IsInRoleAsync(
                yonetici, "Yönetici"))
            {
                var rolSonucu = await userManager.AddToRoleAsync(
                    yonetici, "Yönetici");

                if (!rolSonucu.Succeeded)
                {
                    throw new Exception(
                        string.Join(" ",
                            rolSonucu.Errors.Select(x => x.Description)));
                }
            }
        }
    }
}
