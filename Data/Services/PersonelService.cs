
using ITArizaTakip.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITArizaTakip.Services
{
    public class PersonelService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public PersonelService(
            UserManager<ApplicationUser> userManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        // Personelleri listele
        public async Task<List<ApplicationUser>> PersonelleriGetirAsync()
        {
            var personelKullanicilari = await _userManager
                .GetUsersInRoleAsync("Personel");

            return personelKullanicilari
                .Where(p => p.AktifMi)
                .OrderBy(p => p.AdSoyad)
                .ToList();
        }

        // Departmanları listele
        public async Task<List<Departman>> DepartmanlariGetirAsync()
        {
            return await _db.Departmanlar
                .Where(d => d.AktifMi)
                .OrderBy(d => d.Ad)
                .ToListAsync();
        }

        // Tüm departmanları listele
        public async Task<List<Departman>> TumDepartmanlariGetirAsync()
        {
            return await _db.Departmanlar
                .OrderBy(d => d.Ad)
                .ToListAsync();
        }

        // Departman ekle
        public async Task<(bool Basarili, string Mesaj)> DepartmanEkleAsync(
            string ad,
            string? aciklama)
        {
            if (string.IsNullOrWhiteSpace(ad))
                return (false, "Departman adı zorunludur.");

            string temizAd = ad.Trim();

            bool mevcutMu = await _db.Departmanlar
                .AnyAsync(d => d.Ad.ToLower() == temizAd.ToLower());

            if (mevcutMu)
                return (false, "Bu departman zaten kayıtlı.");

            var departman = new Departman
            {
                Ad = temizAd,
                Aciklama = string.IsNullOrWhiteSpace(aciklama)
                    ? null
                    : aciklama.Trim(),
                AktifMi = true
            };

            _db.Departmanlar.Add(departman);
            await _db.SaveChangesAsync();

            return (true, "Departman başarıyla eklendi.");
        }

        // Departman düzenle
        public async Task<(bool Basarili, string Mesaj)> DepartmanGuncelleAsync(
            int departmanId,
            string ad,
            string? aciklama)
        {
            var departman = await _db.Departmanlar
                .FirstOrDefaultAsync(d => d.Id == departmanId);

            if (departman == null)
                return (false, "Departman bulunamadı.");

            if (!departman.AktifMi)
                return (false, "Pasif departman düzenlenemez.");

            if (string.IsNullOrWhiteSpace(ad))
                return (false, "Departman adı zorunludur.");

            string temizAd = ad.Trim();

            bool mevcutMu = await _db.Departmanlar
                .AnyAsync(d =>
                    d.Id != departmanId &&
                    d.Ad.ToLower() == temizAd.ToLower());

            if (mevcutMu)
                return (false, "Bu isimde başka bir departman zaten var.");

            departman.Ad = temizAd;
            departman.Aciklama = string.IsNullOrWhiteSpace(aciklama)
                ? null
                : aciklama.Trim();

            await _db.SaveChangesAsync();

            return (true, "Departman başarıyla güncellendi.");
        }

        // Departmanı pasife al
        public async Task<(bool Basarili, string Mesaj)> DepartmanPasifeAlAsync(
            int departmanId)
        {
            var departman = await _db.Departmanlar
                .FirstOrDefaultAsync(d => d.Id == departmanId);

            if (departman == null)
                return (false, "Departman bulunamadı.");

            if (!departman.AktifMi)
                return (false, "Departman zaten pasif.");

            departman.AktifMi = false;

            await _db.SaveChangesAsync();

            return (true, "Departman pasife alındı.");
        }

        // Personel ekle
        public async Task<IdentityResult> PersonelEkleAsync(
            string adSoyad,
            string email,
            string sifre,
            int? departmanId)
        {
            if (string.IsNullOrWhiteSpace(adSoyad) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(sifre))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Ad soyad, e-posta ve şifre zorunludur."
                    });
            }

            if (departmanId.HasValue)
            {
                bool departmanVarMi = await _db.Departmanlar
                    .AnyAsync(d => d.Id == departmanId.Value && d.AktifMi);

                if (!departmanVarMi)
                {
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Seçilen departman bulunamadı veya aktif değil."
                        });
                }
            }

            var kullanici = new ApplicationUser
            {
                UserName = email.Trim(),
                Email = email.Trim(),
                EmailConfirmed = true,
                AdSoyad = adSoyad.Trim(),
                Departman = departmanId.HasValue
                    ? (await _db.Departmanlar.FindAsync(departmanId.Value))?.Ad ?? ""
                    : "",
                DepartmanId = departmanId,
                AktifMi = true
            };

            var sonuc = await _userManager.CreateAsync(kullanici, sifre);

            if (!sonuc.Succeeded)
                return sonuc;

            var rolSonucu = await _userManager.AddToRoleAsync(
                kullanici,
                "Personel"
            );

            if (!rolSonucu.Succeeded)
            {
                await _userManager.DeleteAsync(kullanici);
                return rolSonucu;
            }

            return IdentityResult.Success;
        }

        // Personel bilgilerini düzenle
        public async Task<IdentityResult> PersonelGuncelleAsync(
            string personelId,
            string adSoyad,
            string email,
            int? departmanId)
        {
            var personel = await _userManager.FindByIdAsync(personelId);

            if (personel == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Personel bulunamadı."
                    });
            }

            var roller = await _userManager.GetRolesAsync(personel);

            if (!roller.Contains("Personel"))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Bu kullanıcı personel rolünde değil."
                    });
            }

            if (!personel.AktifMi)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Pasif personel düzenlenemez."
                    });
            }

            if (string.IsNullOrWhiteSpace(adSoyad) ||
                string.IsNullOrWhiteSpace(email))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Ad soyad ve e-posta zorunludur."
                    });
            }

            Departman? secilenDepartman = null;

            if (departmanId.HasValue)
            {
                secilenDepartman = await _db.Departmanlar
                    .FirstOrDefaultAsync(d =>
                        d.Id == departmanId.Value && d.AktifMi);

                if (secilenDepartman == null)
                {
                    return IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Seçilen departman bulunamadı veya aktif değil."
                        });
                }
            }

            personel.AdSoyad = adSoyad.Trim();
            personel.Email = email.Trim();
            personel.UserName = email.Trim();
            personel.DepartmanId = departmanId;
            personel.Departman = secilenDepartman?.Ad ?? "";

            return await _userManager.UpdateAsync(personel);
        }

        // Her aktif departmana otomatik personel oluştur
        public async Task<(bool Basarili, string Mesaj)> OrnekPersonelleriOlusturAsync()
        {
            var departmanlar = await _db.Departmanlar
                .Where(d => d.AktifMi)
                .OrderBy(d => d.Ad)
                .ToListAsync();

            if (departmanlar.Count == 0)
                return (false, "Aktif departman bulunamadı.");

            var personeller = (await _userManager
                .GetUsersInRoleAsync("Personel")).ToList();

            string[] adSoyadlar =
            {
                "Ahmet Yılmaz",
                "Mehmet Demir",
                "Ayşe Kaya",
                "Mustafa Şahin",
                "Zeynep Çelik",
                "Emre Yıldız",
                "Elif Aydın",
                "Burak Arslan",
                "Fatma Özkan",
                "Can Öztürk",
                "Merve Koç",
                "Ali Kurt",
                "Deniz Aksoy",
                "Selin Polat",
                "Onur Kılıç"
            };

            int eklenen = 0;
            int guncellenen = 0;

            for (int i = 0; i < departmanlar.Count; i++)
            {
                var departman = departmanlar[i];

                var mevcutPersonel = personeller.FirstOrDefault(p =>
                    p.AktifMi && p.DepartmanId == departman.Id);

                // Departmanda zaten personel varsa,
                // yalnızca eski Demo Personeli adını düzelt.
                if (mevcutPersonel != null)
                {
                    if (mevcutPersonel.AdSoyad.Contains(
                        "Demo Personeli",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        mevcutPersonel.AdSoyad =
                            adSoyadlar[i % adSoyadlar.Length];

                        mevcutPersonel.Departman = departman.Ad;

                        var guncelleme = await _userManager
                            .UpdateAsync(mevcutPersonel);

                        if (!guncelleme.Succeeded)
                        {
                            return (false, string.Join(" ",
                                guncelleme.Errors.Select(e => e.Description)));
                        }

                        guncellenen++;
                    }

                    continue;
                }

                string email =
                    $"demo.departman{departman.Id}@itariza.local";

                var mevcutKullanici = await _userManager
                    .FindByEmailAsync(email);

                if (mevcutKullanici != null)
                {
                    var roller = await _userManager
                        .GetRolesAsync(mevcutKullanici);

                    if (!roller.Contains("Personel"))
                        return (false, $"{email} başka bir kullanıcıya ait.");

                    mevcutKullanici.AdSoyad =
                        adSoyadlar[i % adSoyadlar.Length];

                    mevcutKullanici.DepartmanId = departman.Id;
                    mevcutKullanici.Departman = departman.Ad;
                    mevcutKullanici.AktifMi = true;

                    var guncelleme = await _userManager
                        .UpdateAsync(mevcutKullanici);

                    if (!guncelleme.Succeeded)
                    {
                        return (false, string.Join(" ",
                            guncelleme.Errors.Select(e => e.Description)));
                    }

                    personeller.Add(mevcutKullanici);
                    eklenen++;
                    continue;
                }

                var yeniPersonel = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    AdSoyad = adSoyadlar[i % adSoyadlar.Length],
                    Departman = departman.Ad,
                    DepartmanId = departman.Id,
                    AktifMi = true
                };

                var sonuc = await _userManager.CreateAsync(
                    yeniPersonel,
                    "Personel123!"
                );

                if (!sonuc.Succeeded)
                {
                    return (false, string.Join(" ",
                        sonuc.Errors.Select(e => e.Description)));
                }

                var rolSonucu = await _userManager.AddToRoleAsync(
                    yeniPersonel,
                    "Personel"
                );

                if (!rolSonucu.Succeeded)
                {
                    await _userManager.DeleteAsync(yeniPersonel);

                    return (false, string.Join(" ",
                        rolSonucu.Errors.Select(e => e.Description)));
                }

                personeller.Add(yeniPersonel);
                eklenen++;
            }

            return (true,
                $"{eklenen} yeni personel eklendi, {guncellenen} eski personel adı güncellendi.");
        }

        // Personeli pasife al
        public async Task<IdentityResult> PersonelPasifeAlAsync(
            string personelId)
        {
            var personel = await _userManager.FindByIdAsync(personelId);

            if (personel == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Personel bulunamadı."
                    });
            }

            var roller = await _userManager.GetRolesAsync(personel);

            if (!roller.Contains("Personel"))
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Bu kullanıcı personel rolünde değil."
                    });
            }

            personel.AktifMi = false;

            return await _userManager.UpdateAsync(personel);
        }
    }
}
