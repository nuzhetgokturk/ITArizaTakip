\# IT Arıza Takip Sistemi



\## 1. Proje Hakkında



IT Arıza Takip Sistemi, kurum ve işletmelerde meydana gelen bilgi teknolojileri (IT) kaynaklı arızaların dijital ortamda kayıt altına alınması, yönetilmesi, ilgili personele atanması ve süreçlerinin takip edilmesi amacıyla geliştirilmiş web tabanlı bir yönetim uygulamasıdır.



Günümüzde kurumlarda bilgisayar, yazıcı, ağ bağlantısı, yazılım ve diğer bilişim teknolojileriyle ilgili çeşitli teknik sorunlar meydana gelebilmektedir. Bu sorunların düzenli şekilde takip edilmemesi; iş süreçlerinde aksamalara, zaman kaybına ve müdahale süreçlerinin uzamasına neden olabilmektedir.



Bu proje, söz konusu süreçleri tek bir platform üzerinden yönetilebilir hâle getirmeyi hedeflemektedir.



Sistem sayesinde kullanıcılar arıza kaydı oluşturabilir, yöneticiler arıza kayıtlarını inceleyebilir, ilgili departman ve personele atama yapabilir ve arızaların durumlarını takip edebilir.



Proje, C# ve Blazor teknolojileri kullanılarak geliştirilmiş olup PostgreSQL veritabanı üzerinde çalışmaktadır.



\## 2. Projenin Amacı ve Hedefleri



Projenin temel amacı, kurumların IT destek ve teknik arıza yönetimi süreçlerini dijitalleştirerek daha düzenli, hızlı ve kontrollü bir yönetim altyapısı oluşturmaktır.



Projenin hedefleri:



\- IT arızalarının merkezi bir sistem üzerinden kayıt altına alınmasını sağlamak.

\- Arıza kayıtlarının düzenli şekilde takip edilmesini sağlamak.

\- Arızaların ilgili departmanlara ve personele atanmasını kolaylaştırmak.

\- Personel ve departman bilgilerinin merkezi olarak yönetilmesini sağlamak.

\- Kullanıcı işlemlerini yetkilendirme sistemiyle kontrol etmek.

\- Arıza süreçlerinin yönetim paneli üzerinden izlenmesini sağlamak.

\- Raporlama özellikleriyle arıza süreçlerinin değerlendirilmesine yardımcı olmak.

\- Manuel takip işlemlerini azaltarak iş süreçlerini kolaylaştırmak.



\## 3. Kullanılan Teknolojiler ve Sürümleri



Projenin geliştirilmesinde aşağıdaki teknolojiler ve yazılım paketleri kullanılmıştır.



| Teknoloji / Paket | Kullanılan Sürüm |

|---|---|

| C# | 14 (varsayılan .NET 10 dil sürümü) |

| .NET SDK | 10.0.401 |

| .NET Target Framework | net10.0 |

| ASP.NET Core | .NET 10 |

| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 10.0.4 |

| Microsoft.EntityFrameworkCore.Design | 10.0.4 |

| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 |

| PostgreSQL | 18.7 |

| Bootstrap | 5.3.3 |

| HTML | HTML5 |

| CSS | CSS3 |

| JavaScript | JavaScript |



\### Kullanılan Teknolojilerin Görevleri



\*\*C#:\*\* Uygulamanın iş mantığının ve sunucu tarafındaki işlemlerinin geliştirilmesinde kullanılmıştır.



\*\*Blazor:\*\* Web arayüzünün oluşturulmasında ve kullanıcı etkileşimlerinin yönetilmesinde kullanılmıştır.



\*\*ASP.NET Core:\*\* Web uygulamasının altyapısını ve sunucu tarafındaki çalışma ortamını sağlamaktadır.



\*\*PostgreSQL:\*\* Kullanıcı, personel, departman ve arıza bilgilerinin saklandığı ilişkisel veritabanıdır.



\*\*Entity Framework Core:\*\* Uygulama ile veritabanı arasındaki veri erişim işlemlerini yönetmekte ve veritabanı işlemlerini kolaylaştırmaktadır.



\*\*Npgsql:\*\* PostgreSQL veritabanı ile Entity Framework Core arasındaki bağlantıyı sağlamaktadır.



\*\*ASP.NET Core Identity:\*\* Kullanıcı kayıt, giriş, kimlik doğrulama ve rol tabanlı yetkilendirme işlemlerinde kullanılmaktadır.



\*\*Bootstrap:\*\* Web arayüzünün düzenlenmesi ve farklı ekran boyutlarına uyumlu tasarım oluşturulmasında kullanılmıştır.



\## 4. Sistem Modülleri ve Temel Özellikler



\### 4.1. Kullanıcı Yönetimi



\- Kullanıcı kayıt işlemleri

\- Kullanıcı giriş işlemleri

\- Kimlik doğrulama

\- Kullanıcı hesaplarının yönetimi

\- Rol tabanlı yetkilendirme



\### 4.2. Arıza Yönetimi



\- Yeni arıza kaydı oluşturma

\- Kayıtlı arızaları listeleme

\- Arıza bilgilerini görüntüleme

\- Arıza kayıtlarını düzenleme

\- Arıza süreçlerinin takip edilmesi



\### 4.3. Personel Yönetimi



\- Personel kayıtlarının yönetilmesi

\- Sistemdeki personellerin listelenmesi

\- Personel bilgilerinin düzenlenmesi

\- Personellerin departmanlarıyla ilişkilendirilmesi

\- Arıza atamalarında personel bilgilerinin kullanılması



\### 4.4. Departman Yönetimi



\- Departman kayıtlarının yönetilmesi

\- Departmanların listelenmesi

\- Departman bilgilerinin düzenlenmesi

\- Personel ve departman ilişkisinin yönetilmesi



\### 4.5. Arıza Atama Sistemi



\- Arızaların ilgili departmanlara yönlendirilmesi

\- Departmanlara bağlı personellerin listelenmesi

\- Arızaların sorumlu personele atanması

\- Atama süreçlerinin yönetilmesi



\### 4.6. Yönetim Paneli



Yönetim paneli üzerinden sistemdeki temel yönetim işlemlerine ve genel bilgilere erişilebilmektedir.



\### 4.7. Raporlama Modülü



Sistemdeki arıza verilerinin raporlanması ve değerlendirilmesi amacıyla raporlama modülü geliştirilmiştir.



Bu modül, arıza süreçleri hakkında genel bilgi edinilmesine ve yönetimsel değerlendirmeler yapılmasına yardımcı olmaktadır.



\## 5. Sistem Mimarisi



Proje, ASP.NET Core ve Blazor altyapısı üzerinde geliştirilmiştir.



Uygulama temel olarak aşağıdaki katman ve bileşenlerden oluşmaktadır:



\- \*\*Components:\*\* Blazor sayfaları, kullanıcı arayüzü ve uygulama bileşenleri.

\- \*\*Data:\*\* Veritabanı bağlamı, veri modelleri ve veri yönetimi bileşenleri.

\- \*\*Data/Services:\*\* Arıza, personel, gösterge paneli ve raporlama işlemlerini yöneten servisler.

\- \*\*Migrations:\*\* Entity Framework Core tarafından oluşturulan veritabanı şema değişiklikleri.

\- \*\*wwwroot:\*\* CSS, JavaScript, Bootstrap ve statik dosyalar.

\- \*\*Program.cs:\*\* Uygulamanın servis kayıtları, veritabanı bağlantısı ve başlangıç yapılandırması.



\## 6. Veritabanı Yönetimi



Projenin veritabanı yönetiminde PostgreSQL 18.7 kullanılmıştır.



Entity Framework Core aracılığıyla uygulama modelleri ile veritabanı arasındaki ilişkiler yönetilmektedir.



Veritabanı işlemlerinde kullanılan temel bileşenler:



\- PostgreSQL

\- Entity Framework Core

\- Npgsql Entity Framework Core Provider

\- ASP.NET Core Identity



Veritabanı şemasındaki değişiklikler Entity Framework Core Migration sistemiyle takip edilmektedir.



\### Veritabanı Güvenliği



Veritabanı bağlantı bilgileri güvenlik amacıyla GitHub deposuna eklenmemiştir.



`appsettings.json` ve `appsettings.Development.json` dosyaları `.gitignore` aracılığıyla Git takibinin dışında tutulmaktadır.



Veritabanı yedek dosyası da kaynak kodlarından ayrı olarak saklanmaktadır.



\## 7. Projenin Kurulumu ve Çalıştırılması



Projeyi yerel bilgisayarında çalıştırmak için aşağıdaki adımlar izlenmelidir.



\### Gereksinimler



\- .NET 10 SDK

\- PostgreSQL 18.7 veya uyumlu bir PostgreSQL sürümü

\- pgAdmin 4

\- Visual Studio veya Visual Studio Code

\- Git



\### 7.1. Projeyi GitHub Üzerinden İndirme



Terminal veya PowerShell üzerinden:



```bash

git clone https://github.com/nuzhetgokturk/ITArizaTakip.git

```



Proje klasörüne geçiş:



```bash

cd ITArizaTakip

```



\### 7.2. Veritabanının Oluşturulması



PostgreSQL üzerinde proje için bir veritabanı oluşturulmalıdır.



Örnek veritabanı adı:



```text

ITArizaTakip

```



\### 7.3. Veritabanı Bağlantısının Yapılandırılması



Proje ana klasöründe `appsettings.json` dosyası oluşturulmalıdır.



Örnek yapılandırma:



```json

{

&#x20; "ConnectionStrings": {

&#x20;   "DefaultConnection": "Host=localhost;Port=5432;Database=ITArizaTakip;Username=postgres;Password=BURAYA\_SIFRENIZ"

&#x20; }

}

```



Yukarıdaki kullanıcı adı ve parola örnektir. Kendi PostgreSQL bağlantı bilgilerinize göre düzenlenmelidir.



Bu dosya güvenlik nedeniyle GitHub'a yüklenmemelidir.



\### 7.4. Gerekli Paketlerin Yüklenmesi



Proje klasöründe aşağıdaki komut çalıştırılmalıdır:



```bash

dotnet restore

```



\### 7.5. Veritabanı Migration İşlemleri



Entity Framework Core araçları kurulu değilse:



```bash

dotnet tool install --global dotnet-ef

```



Daha sonra veritabanı tablolarını oluşturmak veya mevcut şemayı güncellemek için:



```bash

dotnet ef database update

```



\### 7.6. Projenin Çalıştırılması



Uygulamayı başlatmak için:



```bash

dotnet run

```



Terminalde gösterilen yerel adres üzerinden uygulamaya erişilebilir.



\## 8. Güvenlik



Projede ASP.NET Core Identity kullanılarak kullanıcı kimlik doğrulama ve yetkilendirme altyapısı oluşturulmuştur.



Veritabanı bağlantı bilgilerinin kaynak kodlarından ayrı tutulmasına dikkat edilmiştir.



GitHub deposunda gerçek veritabanı parolaları ve yedek dosyaları paylaşılmamaktadır.



\## 9. Proje Dosya Yapısı



```text

ITArizaTakip/

│

├── Components/

│   ├── Layout/

│   └── Pages/

│

├── Data/

│   ├── Services/

│   ├── AppDbContext.cs

│   ├── ApplicationUser.cs

│   ├── Ariza.cs

│   └── departman.cs

│

├── Migrations/

│

├── Properties/

│

├── wwwroot/

│   ├── lib/

│   └── app.css

│

├── .gitignore

├── ITArizaTakip.csproj

└── Program.cs

```



\## 10. Projenin Geliştirilme Amacı



Bu proje, yazılım geliştirme ve veritabanı yönetimi alanlarında edinilen bilgilerin uygulamaya aktarılması amacıyla geliştirilmiştir.



Proje kapsamında web tabanlı uygulama geliştirme, ilişkisel veritabanı tasarımı, kullanıcı yönetimi, kimlik doğrulama, yetkilendirme, veri erişim işlemleri ve raporlama konularında uygulamalı çalışmalar gerçekleştirilmiştir.



\## 11. Geliştirici



\*\*Nüzhet Göktürk\*\*



\## 12. GitHub Proje Bağlantısı



\[IT Arıza Takip Sistemi – GitHub](https://github.com/nuzhetgokturk/ITArizaTakip)





