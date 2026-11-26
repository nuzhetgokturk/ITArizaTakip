
using ITArizaTakip.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITArizaTakip.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _db;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public DashboardService(
            AppDbContext db,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _db = db;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<DashboardOzet> OzetGetirAsync()
        {
            var durum = await _authenticationStateProvider
                .GetAuthenticationStateAsync();

            var kullanici = durum.User;

            if (kullanici.Identity?.IsAuthenticated != true)
                return new DashboardOzet();

            bool yonetici = kullanici.IsInRole("Yönetici");
            bool personel = kullanici.IsInRole("Personel");

            if (!yonetici && !personel)
                return new DashboardOzet();

            var sorgu = _db.Arizalar.AsQueryable();

            if (!yonetici)
            {
                string? kullaniciId = kullanici
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(kullaniciId))
                    return new DashboardOzet();

                sorgu = sorgu.Where(a => a.KullaniciId == kullaniciId);
            }

            return new DashboardOzet
            {
                ToplamAriza = await sorgu.CountAsync(),

                BekleyenAriza = await sorgu.CountAsync(
                    a => a.Durum == "Bekliyor"),

                IslemdeAriza = await sorgu.CountAsync(
                    a => a.Durum == "İşlemde"),

                CozulduAriza = await sorgu.CountAsync(
                    a => a.Durum == "Çözüldü"),

                YuksekOncelik = await sorgu.CountAsync(
                    a => a.Oncelik == "Yüksek"),

                OrtaOncelik = await sorgu.CountAsync(
                    a => a.Oncelik == "Orta"),

                DusukOncelik = await sorgu.CountAsync(
                    a => a.Oncelik == "Düşük")
            };
        }
    }

    public class DashboardOzet
    {
        public int ToplamAriza { get; set; }
        public int BekleyenAriza { get; set; }
        public int IslemdeAriza { get; set; }
        public int CozulduAriza { get; set; }

        public int YuksekOncelik { get; set; }
        public int OrtaOncelik { get; set; }
        public int DusukOncelik { get; set; }
    }
}
