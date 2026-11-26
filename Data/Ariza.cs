
namespace ITArizaTakip.Data
{
    public class Ariza
    {
        public int Id { get; set; }

        public string TalepEden { get; set; } = "";

        public string Departman { get; set; } = "";

        public string ArizaBasligi { get; set; } = "";

        public string Aciklama { get; set; } = "";

        public string Oncelik { get; set; } = "Orta";

        public string Durum { get; set; } = "Bekliyor";

        public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

        // Arıza kaydını oluşturan kullanıcının kimliği
        public string? KullaniciId { get; set; }

        // Arızanın atandığı departman
        public int? AtananDepartmanId { get; set; }
        public Departman? AtananDepartman { get; set; }

        // Arızanın atandığı personel
        public string? AtananPersonelId { get; set; }
        public ApplicationUser? AtananPersonel { get; set; }
    }
}
