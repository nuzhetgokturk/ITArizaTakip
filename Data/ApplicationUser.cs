
using Microsoft.AspNetCore.Identity;

namespace ITArizaTakip.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string AdSoyad { get; set; } = "";

        // Mevcut departman bilgisi korunuyor.
        public string Departman { get; set; } = "";

        // Yeni departman tablosuyla ilişki.
        public int? DepartmanId { get; set; }

        public Departman? DepartmanBilgisi { get; set; }

        // Personelin sistemde aktif olup olmadığını belirtir.
        public bool AktifMi { get; set; } = true;
    }
}
