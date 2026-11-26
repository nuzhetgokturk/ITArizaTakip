
using ITArizaTakip.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITArizaTakip.Services
{
    public class ArizaService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public ArizaService(
            AppDbContext db,
            UserManager<ApplicationUser> userManager,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _db = db;
            _userManager = userManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        // Oturumdaki kullanıcıyı getir
        private async Task<ClaimsPrincipal> KullaniciGetirAsync()
        {
            var durum = await _authenticationStateProvider
                .GetAuthenticationStateAsync();

            return durum.User;
        }

        // Oturumdaki kullanıcının ID bilgisini getir
        private string? KullaniciIdAl(ClaimsPrincipal kullanici)
        {
            if (kullanici.Identity?.IsAuthenticated != true)
                return null;

            return kullanici.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;
        }

        // Kullanıcının yönetici olup olmadığını kontrol et
        private async Task<bool> YoneticiMiAsync()
        {
            var kullanici = await KullaniciGetirAsync();

            return kullanici.Identity?.IsAuthenticated == true
                && kullanici.IsInRole("Yönetici");
        }

        // Kullanıcı bu arıza üzerinde işlem yapabilir mi?
        private async Task<bool> IslemYetkisiVarMiAsync(Ariza ariza)
        {
            var kullanici = await KullaniciGetirAsync();

            if (kullanici.Identity?.IsAuthenticated != true)
                return false;

            if (kullanici.IsInRole("Yönetici"))
                return true;

            if (!kullanici.IsInRole("Personel"))
                return false;

            var kullaniciId = KullaniciIdAl(kullanici);

            return !string.IsNullOrWhiteSpace(kullaniciId)
                && ariza.KullaniciId == kullaniciId;
        }

        // Yeni arıza oluşturma
        public async Task<(bool Basarili, string Mesaj)> ArizaOlusturAsync(
            string baslik,
            string aciklama,
            string oncelik)
        {
            var oturumdakiKullanici = await KullaniciGetirAsync();

            var gercekKullaniciId = KullaniciIdAl(oturumdakiKullanici);

            if (string.IsNullOrWhiteSpace(gercekKullaniciId))
                return (false, "Oturum bilgisi bulunamadı.");

            if (!oturumdakiKullanici.IsInRole("Personel") &&
                !oturumdakiKullanici.IsInRole("Yönetici"))
                return (false, "Bu işlem için yetkiniz yok.");

            var kullanici = await _userManager.FindByIdAsync(
                gercekKullaniciId);

            if (kullanici == null)
                return (false, "Kullanıcı bulunamadı.");

            if (string.IsNullOrWhiteSpace(baslik))
                return (false, "Arıza başlığı zorunludur.");

            if (baslik.Trim().Length > 150)
                return (false, "Arıza başlığı en fazla 150 karakter olabilir.");

            if (string.IsNullOrWhiteSpace(aciklama))
                return (false, "Arıza açıklaması zorunludur.");

            if (aciklama.Trim().Length > 2000)
                return (false, "Açıklama en fazla 2000 karakter olabilir.");

            string[] gecerliOncelikler =
            {
                "Düşük", "Orta", "Yüksek"
            };

            if (!gecerliOncelikler.Contains(oncelik))
                return (false, "Geçersiz arıza önceliği.");

            var ariza = new Ariza
            {
                TalepEden = kullanici.AdSoyad,
                Departman = kullanici.Departman,
                ArizaBasligi = baslik.Trim(),
                Aciklama = aciklama.Trim(),
                Oncelik = oncelik,
                Durum = "Bekliyor",
                KayitTarihi = DateTime.UtcNow,
                KullaniciId = kullanici.Id
            };

            _db.Arizalar.Add(ariza);
            await _db.SaveChangesAsync();

            return (true, "Arıza kaydı başarıyla oluşturuldu!");
        }

        // Arıza listesini getirme
        // Yönetici bütün arızaları görür.
        // Personel kendi oluşturduğu ve kendisine atanan arızaları görür.
        public async Task<List<Ariza>> ArizalariGetirAsync()
        {
            var kullanici = await KullaniciGetirAsync();

            if (kullanici.Identity?.IsAuthenticated != true)
                return new List<Ariza>();

            var gercekKullaniciId = KullaniciIdAl(kullanici);

            if (string.IsNullOrWhiteSpace(gercekKullaniciId))
                return new List<Ariza>();

            var sorgu = _db.Arizalar
                .AsNoTracking()
                .Include(a => a.AtananDepartman)
                .Include(a => a.AtananPersonel)
                .AsQueryable();

            if (kullanici.IsInRole("Yönetici"))
            {
                // Yönetici bütün arızaları görebilir.
            }
            else if (kullanici.IsInRole("Personel"))
            {
                sorgu = sorgu.Where(a =>
                    a.KullaniciId == gercekKullaniciId ||
                    a.AtananPersonelId == gercekKullaniciId);
            }
            else
            {
                return new List<Ariza>();
            }

            return await sorgu
                .OrderByDescending(a => a.KayitTarihi)
                .ToListAsync();
        }

        // Aktif departmanları getir
        public async Task<List<Departman>> DepartmanlariGetirAsync()
        {
            if (!await YoneticiMiAsync())
                return new List<Departman>();

            return await _db.Departmanlar
                .AsNoTracking()
                .Where(d => d.AktifMi)
                .OrderBy(d => d.Ad)
                .ToListAsync();
        }

        // Aktif personelleri getir
        public async Task<List<ApplicationUser>> PersonelleriGetirAsync()
        {
            if (!await YoneticiMiAsync())
                return new List<ApplicationUser>();

            var personeller = await _userManager
                .GetUsersInRoleAsync("Personel");

            return personeller
                .Where(p => p.AktifMi)
                .OrderBy(p => p.AdSoyad)
                .ToList();
        }

        // Arızayı departmana ve personele atama
        public async Task<(bool Basarili, string Mesaj)> ArizaAtaAsync(
            int arizaId,
            int? departmanId,
            string? personelId)
        {
            if (!await YoneticiMiAsync())
                return (false, "Bu işlem için yönetici yetkisi gereklidir.");

            var ariza = await _db.Arizalar.FindAsync(arizaId);

            if (ariza == null)
                return (false, "Arıza kaydı bulunamadı.");

            // Departman kontrolü
            if (departmanId.HasValue)
            {
                var departman = await _db.Departmanlar
                    .FirstOrDefaultAsync(d =>
                        d.Id == departmanId.Value && d.AktifMi);

                if (departman == null)
                    return (false, "Seçilen departman bulunamadı veya aktif değil.");
            }

            // Personel kontrolü
            if (!string.IsNullOrWhiteSpace(personelId))
            {
                var personel = await _userManager
                    .FindByIdAsync(personelId);

                if (personel == null || !personel.AktifMi)
                    return (false, "Seçilen personel bulunamadı veya aktif değil.");

                var personelMi = await _userManager
                    .IsInRoleAsync(personel, "Personel");

                if (!personelMi)
                    return (false, "Seçilen kullanıcı personel rolünde değil.");
            }

            ariza.AtananDepartmanId = departmanId;
            ariza.AtananPersonelId = string.IsNullOrWhiteSpace(personelId)
                ? null
                : personelId;

            await _db.SaveChangesAsync();

            return (true, "Arıza ataması başarıyla yapıldı!");
        }

        // Arıza durumunu güncelleme
        public async Task<(bool Basarili, string Mesaj)> DurumGuncelleAsync(
            int arizaId,
            string yeniDurum)
        {
            if (!await YoneticiMiAsync())
                return (false, "Bu işlem için yönetici yetkisi gereklidir.");

            string[] gecerliDurumlar =
            {
                "Bekliyor",
                "İşlemde",
                "Çözüldü"
            };

            if (string.IsNullOrWhiteSpace(yeniDurum) ||
                !gecerliDurumlar.Contains(yeniDurum))
                return (false, "Geçersiz arıza durumu.");

            var ariza = await _db.Arizalar.FindAsync(arizaId);

            if (ariza == null)
                return (false, "Arıza kaydı bulunamadı.");

            ariza.Durum = yeniDurum;

            await _db.SaveChangesAsync();

            return (true, "Arıza durumu başarıyla güncellendi!");
        }

        // Arıza silme
        public async Task<(bool Basarili, string Mesaj)> ArizaSilAsync(
            int arizaId)
        {
            var ariza = await _db.Arizalar.FindAsync(arizaId);

            if (ariza == null)
                return (false, "Arıza kaydı bulunamadı.");

            if (!await IslemYetkisiVarMiAsync(ariza))
                return (false, "Bu arızayı silme yetkiniz yok.");

            _db.Arizalar.Remove(ariza);

            await _db.SaveChangesAsync();

            return (true, "Arıza kaydı başarıyla silindi!");
        }

        // ID ile arıza getirme
        public async Task<Ariza?> ArizaGetirAsync(int id)
        {
            var ariza = await _db.Arizalar
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ariza == null)
                return null;

            if (!await IslemYetkisiVarMiAsync(ariza))
                return null;

            return ariza;
        }

        // Arıza bilgilerini güncelleme
        public async Task<(bool Basarili, string Mesaj)> ArizaGuncelleAsync(
            int id,
            string talepEden,
            string departman,
            string baslik,
            string aciklama,
            string oncelik)
        {
            var ariza = await _db.Arizalar.FindAsync(id);

            if (ariza == null)
                return (false, "Arıza kaydı bulunamadı.");

            var kullanici = await KullaniciGetirAsync();

            if (kullanici.Identity?.IsAuthenticated != true)
                return (false, "Oturum bilgisi bulunamadı.");

            bool yonetici = kullanici.IsInRole("Yönetici");

            if (!await IslemYetkisiVarMiAsync(ariza))
                return (false, "Bu arızayı düzenleme yetkiniz yok.");

            if (yonetici)
            {
                if (string.IsNullOrWhiteSpace(talepEden) ||
                    string.IsNullOrWhiteSpace(departman))
                    return (false, "Talep eden ve departman bilgileri zorunludur.");

                if (talepEden.Trim().Length > 200 ||
                    departman.Trim().Length > 100)
                    return (false, "Talep eden veya departman bilgisi çok uzun.");

                ariza.TalepEden = talepEden.Trim();
                ariza.Departman = departman.Trim();
            }

            if (string.IsNullOrWhiteSpace(baslik))
                return (false, "Arıza başlığı zorunludur.");

            if (baslik.Trim().Length > 150)
                return (false, "Arıza başlığı en fazla 150 karakter olabilir.");

            if (string.IsNullOrWhiteSpace(aciklama))
                return (false, "Arıza açıklaması zorunludur.");

            if (aciklama.Trim().Length > 2000)
                return (false, "Açıklama en fazla 2000 karakter olabilir.");

            string[] gecerliOncelikler =
            {
                "Düşük", "Orta", "Yüksek"
            };

            if (string.IsNullOrWhiteSpace(oncelik) ||
                !gecerliOncelikler.Contains(oncelik))
                return (false, "Geçersiz arıza önceliği.");

            ariza.ArizaBasligi = baslik.Trim();
            ariza.Aciklama = aciklama.Trim();
            ariza.Oncelik = oncelik;

            await _db.SaveChangesAsync();

            return (true, "Arıza bilgileri başarıyla güncellendi!");
        }
    }
}
