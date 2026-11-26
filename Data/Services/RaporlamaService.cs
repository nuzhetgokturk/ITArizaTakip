
using ITArizaTakip.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ITArizaTakip.Services
{
    public class RaporlamaService
    {
        private readonly AppDbContext _db;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public RaporlamaService(
            AppDbContext db,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _db = db;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<RaporlamaOzet> RaporlariGetirAsync()
        {
            var durum = await _authenticationStateProvider
                .GetAuthenticationStateAsync();

            var kullanici = durum.User;

            if (kullanici.Identity?.IsAuthenticated != true ||
                !kullanici.IsInRole("Yönetici"))
            {
                throw new UnauthorizedAccessException(
                    "Bu raporları görüntüleme yetkiniz yok.");
            }

            var sorgu = _db.Arizalar.AsNoTracking();

            var rapor = new RaporlamaOzet
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
                    a => a.Oncelik == "Düşük"),

                DepartmanRaporu = await sorgu
                    .GroupBy(a => a.Departman)
                    .Select(g => new DepartmanRaporu
                    {
                        Departman = g.Key,
                        Sayi = g.Count()
                    })
                    .OrderByDescending(g => g.Sayi)
                    .ToListAsync(),

                AtananDepartmanSayisi = await sorgu.CountAsync(
                    a => a.AtananDepartmanId != null),

                AtananPersonelSayisi = await sorgu.CountAsync(
                    a => a.AtananPersonelId != null),

                AtanmamisDepartmanSayisi = await sorgu.CountAsync(
                    a => a.AtananDepartmanId == null),

                AtanmamisPersonelSayisi = await sorgu.CountAsync(
                    a => a.AtananPersonelId == null)
            };

            // Atama bilgilerini getir
            var atamalar = await sorgu
                .Include(a => a.AtananDepartman)
                .Include(a => a.AtananPersonel)
                .OrderByDescending(a => a.KayitTarihi)
                .ToListAsync();

            // Departmanlara göre atama raporu
            rapor.AtamaDepartmanRaporu = atamalar
                .Where(a => a.AtananDepartmanId != null)
                .GroupBy(a => a.AtananDepartman?.Ad ?? "Departman bilgisi yok")
                .Select(g => new AtamaDepartmanRaporu
                {
                    Departman = g.Key,
                    Sayi = g.Count(),
                    Bekleyen = g.Count(a => a.Durum == "Bekliyor"),
                    Islemde = g.Count(a => a.Durum == "İşlemde"),
                    Cozuldu = g.Count(a => a.Durum == "Çözüldü")
                })
                .OrderByDescending(g => g.Sayi)
                .ToList();

            // Personele göre atama raporu
            rapor.PersonelAtamaRaporu = atamalar
                .Where(a => a.AtananPersonelId != null)
                .GroupBy(a => new
                {
                    a.AtananPersonelId,
                    AdSoyad = a.AtananPersonel?.AdSoyad ?? "Personel bilgisi yok"
                })
                .Select(g => new PersonelAtamaRaporu
                {
                    PersonelId = g.Key.AtananPersonelId!,
                    PersonelAdi = g.Key.AdSoyad,
                    Sayi = g.Count(),
                    Bekleyen = g.Count(a => a.Durum == "Bekliyor"),
                    Islemde = g.Count(a => a.Durum == "İşlemde"),
                    Cozuldu = g.Count(a => a.Durum == "Çözüldü")
                })
                .OrderByDescending(g => g.Sayi)
                .ToList();

            // Ayrıntılı arıza atama listesi
            rapor.AtamaDetaylari = atamalar
                .Select(a => new AtamaDetayRaporu
                {
                    ArizaId = a.Id,
                    ArizaBasligi = a.ArizaBasligi,
                    TalepEden = a.TalepEden,
                    Departman = a.AtananDepartman?.Ad ?? "Atanmadı",
                    Personel = a.AtananPersonel?.AdSoyad ?? "Atanmadı",
                    Oncelik = a.Oncelik,
                    Durum = a.Durum,
                    KayitTarihi = a.KayitTarihi
                })
                .ToList();

            // Yazılı raporu oluştur
            rapor.YaziliDegerlendirme = YaziliRaporOlustur(rapor);

            return rapor;
        }

        private string YaziliRaporOlustur(RaporlamaOzet rapor)
        {
            if (rapor.ToplamAriza == 0)
            {
                return "Sistemde henüz kayıtlı bir arıza bulunmamaktadır. " +
                       "Raporlama yapılabilmesi için öncelikle arıza kaydı oluşturulmalıdır.";
            }

            double cozulmeOrani =
                (double)rapor.CozulduAriza / rapor.ToplamAriza * 100;

            string metin =
                $"Sistemde toplam {rapor.ToplamAriza} arıza kaydı bulunmaktadır. ";

            metin +=
                $"{rapor.BekleyenAriza} arıza beklemekte, " +
                $"{rapor.IslemdeAriza} arıza işlem aşamasında ve " +
                $"{rapor.CozulduAriza} arıza çözüme kavuşturulmuştur. ";

            metin +=
                $"Genel arıza çözülme oranı %{cozulmeOrani:F1} seviyesindedir. ";

            metin +=
                $"Öncelik dağılımında {rapor.YuksekOncelik} yüksek, " +
                $"{rapor.OrtaOncelik} orta ve " +
                $"{rapor.DusukOncelik} düşük öncelikli arıza bulunmaktadır. ";

            metin +=
                $"Toplam {rapor.AtananDepartmanSayisi} arıza bir departmana, " +
                $"{rapor.AtananPersonelSayisi} arıza bir personele atanmıştır. ";

            metin +=
                $"{rapor.AtanmamisDepartmanSayisi} arıza departman ataması, " +
                $"{rapor.AtanmamisPersonelSayisi} arıza personel ataması beklemektedir. ";

            if (rapor.PersonelAtamaRaporu.Any())
            {
                var enCokArizaAlanPersonel = rapor.PersonelAtamaRaporu.First();

                metin +=
                    $"En fazla arıza ataması {enCokArizaAlanPersonel.PersonelAdi} " +
                    $"isimli personele yapılmıştır. Bu personele " +
                    $"{enCokArizaAlanPersonel.Sayi} arıza atanmıştır. ";
            }

            if (rapor.AtamaDepartmanRaporu.Any())
            {
                var enCokAtamaAlanDepartman = rapor.AtamaDepartmanRaporu.First();

                metin +=
                    $"En fazla atama {enCokAtamaAlanDepartman.Departman} " +
                    $"departmanına yapılmıştır. Bu departmana " +
                    $"{enCokAtamaAlanDepartman.Sayi} arıza atanmıştır. ";
            }

            if (rapor.AtanmamisPersonelSayisi > 0 ||
                rapor.AtanmamisDepartmanSayisi > 0)
            {
                metin +=
                    "Ataması tamamlanmamış arızaların incelenmesi ve " +
                    "ilgili departman veya personele yönlendirilmesi önerilmektedir. ";
            }

            if (rapor.YuksekOncelik > 0)
            {
                metin +=
                    "Yüksek öncelikli arızaların öncelikli olarak ele alınması önerilmektedir. ";
            }

            if (rapor.BekleyenAriza > rapor.CozulduAriza)
            {
                metin +=
                    "Bekleyen arıza sayısı çözülen arıza sayısından fazladır. " +
                    "Çözüm süreçlerinin hızlandırılması önerilir.";
            }
            else
            {
                metin +=
                    "Çözülen arıza sayısı bekleyen arıza sayısından az değildir. " +
                    "Mevcut arıza yönetim sürecinin düzenli takip edilmesi önerilir.";
            }

            return metin;
        }
    }

    public class RaporlamaOzet
    {
        public int ToplamAriza { get; set; }
        public int BekleyenAriza { get; set; }
        public int IslemdeAriza { get; set; }
        public int CozulduAriza { get; set; }

        public int YuksekOncelik { get; set; }
        public int OrtaOncelik { get; set; }
        public int DusukOncelik { get; set; }

        public int AtananDepartmanSayisi { get; set; }
        public int AtananPersonelSayisi { get; set; }
        public int AtanmamisDepartmanSayisi { get; set; }
        public int AtanmamisPersonelSayisi { get; set; }

        public List<DepartmanRaporu> DepartmanRaporu { get; set; } = new();

        public List<AtamaDepartmanRaporu> AtamaDepartmanRaporu { get; set; } = new();

        public List<PersonelAtamaRaporu> PersonelAtamaRaporu { get; set; } = new();

        public List<AtamaDetayRaporu> AtamaDetaylari { get; set; } = new();

        public string YaziliDegerlendirme { get; set; } = "";
    }

    public class DepartmanRaporu
    {
        public string Departman { get; set; } = "";
        public int Sayi { get; set; }
    }

    public class AtamaDepartmanRaporu
    {
        public string Departman { get; set; } = "";
        public int Sayi { get; set; }
        public int Bekleyen { get; set; }
        public int Islemde { get; set; }
        public int Cozuldu { get; set; }
    }

    public class PersonelAtamaRaporu
    {
        public string PersonelId { get; set; } = "";
        public string PersonelAdi { get; set; } = "";
        public int Sayi { get; set; }
        public int Bekleyen { get; set; }
        public int Islemde { get; set; }
        public int Cozuldu { get; set; }
    }

    public class AtamaDetayRaporu
    {
        public int ArizaId { get; set; }
        public string ArizaBasligi { get; set; } = "";
        public string TalepEden { get; set; } = "";
        public string Departman { get; set; } = "";
        public string Personel { get; set; } = "";
        public string Oncelik { get; set; } = "";
        public string Durum { get; set; } = "";
        public DateTime KayitTarihi { get; set; }
    }
}
