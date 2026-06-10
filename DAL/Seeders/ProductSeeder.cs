using DAL.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeders
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            // Ensure categories exist
            await CategoriesSeeder.SeedAsync(context);

            var cats = await context.Categories.ToListAsync();
            int CatId(string name) => cats.First(c => c.Name == name).Id;

            var products = new List<Product>
            {
                // ── Техніка ──────────────────────────────────────────────
                new() {
                    Name = "Смартфон Samsung Galaxy S24",
                    Price = 32999, Description = "6.2\" Dynamic AMOLED, Snapdragon 8 Gen 3, 50 МП камера, 4000 мАг.",
                    ImageUrl = "https://picsum.photos/seed/samsung-s24/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 25
                },
                new() {
                    Name = "iPhone 15 Pro 256 GB",
                    Price = 49999, Description = "Титановий корпус, чіп A17 Pro, ProMotion 120 Гц, USB-C.",
                    ImageUrl = "https://picsum.photos/seed/iphone15pro/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 18
                },
                new() {
                    Name = "Планшет iPad Air 11\" M2",
                    Price = 28500, Description = "Чіп M2, Liquid Retina 11\", 5G, підтримка Apple Pencil Pro.",
                    ImageUrl = "https://picsum.photos/seed/ipad-air/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 12
                },
                new() {
                    Name = "Бездротові навушники Sony WH-1000XM5",
                    Price = 11999, Description = "Найкраще шумозаглушення у класі, 30 год роботи, Hi-Res Audio.",
                    ImageUrl = "https://picsum.photos/seed/sony-headphones/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 40
                },
                new() {
                    Name = "Смарт-годинник Apple Watch Series 9",
                    Price = 16499, Description = "Always-On дисплей, ЧСС, SpO2, GPS, watchOS 10.",
                    ImageUrl = "https://picsum.photos/seed/applewatch9/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 22
                },
                new() {
                    Name = "Навушники TWS AirPods Pro 2",
                    Price = 8999, Description = "Адаптивне шумозаглушення, Spatial Audio, H2 чіп.",
                    ImageUrl = "https://picsum.photos/seed/airpodspro2/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 55
                },
                new() {
                    Name = "Портативна колонка JBL Charge 5",
                    Price = 4799, Description = "IP67, 20 год автономності, PartyBoost, потужний бас.",
                    ImageUrl = "https://picsum.photos/seed/jbl-charge5/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 30
                },

                // ── Комп'ютери ───────────────────────────────────────────
                new() {
                    Name = "Ноутбук ASUS ROG Zephyrus G14",
                    Price = 62999, Description = "AMD Ryzen 9 8945HS, RTX 4070, 32 ГБ RAM, 1 ТБ SSD, 165 Гц.",
                    ImageUrl = "https://picsum.photos/seed/asus-rog/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 8
                },
                new() {
                    Name = "MacBook Air 15\" M3",
                    Price = 57999, Description = "Чіп M3, 15.3\" Liquid Retina, 16 ГБ RAM, 512 ГБ SSD, до 18 год.",
                    ImageUrl = "https://picsum.photos/seed/macbook-air-m3/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 10
                },
                new() {
                    Name = "Механічна клавіатура Keychron K8 Pro",
                    Price = 4299, Description = "Gateron G Pro Red, RGB, бездротова, QMK/VIA.",
                    ImageUrl = "https://picsum.photos/seed/keychron-k8/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 35
                },
                new() {
                    Name = "Ігрова миша Logitech G Pro X Superlight 2",
                    Price = 5199, Description = "HERO 2 сенсор 32K DPI, 60 г, бездротова, 95 год роботи.",
                    ImageUrl = "https://picsum.photos/seed/logitech-gpro/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 28
                },
                new() {
                    Name = "Монітор LG UltraWide 34\" QHD",
                    Price = 22999, Description = "IPS, 3440×1440, 144 Гц, HDR10, USB-C 96 Вт.",
                    ImageUrl = "https://picsum.photos/seed/lg-ultrawide/400/400",
                    CategoryId = CatId("Техніка"), Quantity = 7
                },

                // ── Одяг ─────────────────────────────────────────────────
                new() {
                    Name = "Куртка Nike Windrunner",
                    Price = 3299, Description = "Легка вітрозахисна куртка з капюшоном, 100% нейлон.",
                    ImageUrl = "https://picsum.photos/seed/nike-windrunner/400/400",
                    CategoryId = CatId("Одяг"), Quantity = 50
                },
                new() {
                    Name = "Худі Adidas Essentials",
                    Price = 1899, Description = "Флісовий матеріал, кенгуру-кишеня, унісекс.",
                    ImageUrl = "https://picsum.photos/seed/adidas-hoodie/400/400",
                    CategoryId = CatId("Одяг"), Quantity = 65
                },
                new() {
                    Name = "Джинси Levi's 501 Original",
                    Price = 2799, Description = "Класичний прямий крій, 100% бавовна, синій деним.",
                    ImageUrl = "https://picsum.photos/seed/levis-501/400/400",
                    CategoryId = CatId("Одяг"), Quantity = 44
                },
                new() {
                    Name = "Футболка Tommy Hilfiger Logo",
                    Price = 1299, Description = "Slim Fit, 100% органічна бавовна, вишитий логотип.",
                    ImageUrl = "https://picsum.photos/seed/tommy-tshirt/400/400",
                    CategoryId = CatId("Одяг"), Quantity = 80
                },
                new() {
                    Name = "Зимова куртка The North Face Parka",
                    Price = 8999, Description = "Утеплювач 550-fill goose down, водовідштовхувальне покриття DWR.",
                    ImageUrl = "https://picsum.photos/seed/northface-parka/400/400",
                    CategoryId = CatId("Одяг"), Quantity = 15
                },
                new() {
                    Name = "Спортивний костюм Puma Power",
                    Price = 2499, Description = "Dry Cell технологія, еластичний пояс, бічні кишені.",
                    ImageUrl = "https://picsum.photos/seed/puma-tracksuit/400/400",
                    CategoryId = CatId("Одяг"), Quantity = 38
                },

                // ── Взуття ───────────────────────────────────────────────
                new() {
                    Name = "Кросівки Nike Air Max 270",
                    Price = 4599, Description = "Підошва Air Max 270, сітчастий верх, амортизація на весь день.",
                    ImageUrl = "https://picsum.photos/seed/nike-airmax270/400/400",
                    CategoryId = CatId("Взуття"), Quantity = 45
                },
                new() {
                    Name = "Кеди Adidas Stan Smith",
                    Price = 3499, Description = "Класичний теніс-стиль, шкіряний верх, перфорація з трилисником.",
                    ImageUrl = "https://picsum.photos/seed/adidas-stansmith/400/400",
                    CategoryId = CatId("Взуття"), Quantity = 60
                },
                new() {
                    Name = "Черевики Timberland 6-Inch Premium",
                    Price = 6799, Description = "Водонепроникна нубукова шкіра, підошва EVA, утеплення 200 г.",
                    ImageUrl = "https://picsum.photos/seed/timberland-boots/400/400",
                    CategoryId = CatId("Взуття"), Quantity = 20
                },
                new() {
                    Name = "Кросівки New Balance 574",
                    Price = 3299, Description = "ENCAP підошва, замша + сітка, ретро-силует.",
                    ImageUrl = "https://picsum.photos/seed/nb-574/400/400",
                    CategoryId = CatId("Взуття"), Quantity = 33
                },
                new() {
                    Name = "Кросівки для бігу Brooks Ghost 15",
                    Price = 5199, Description = "BioMoGo DNA амортизація, широка носкова зона, 3D Fit Print.",
                    ImageUrl = "https://picsum.photos/seed/brooks-ghost/400/400",
                    CategoryId = CatId("Взуття"), Quantity = 27
                },

                // ── Книги ────────────────────────────────────────────────
                new() {
                    Name = "Чистий код — Роберт Мартін",
                    Price = 649, Description = "Настільна книга кожного розробника про написання якісного коду.",
                    ImageUrl = "https://picsum.photos/seed/clean-code-book/400/400",
                    CategoryId = CatId("Книги"), Quantity = 100
                },
                new() {
                    Name = "Атомні звички — Джеймс Клір",
                    Price = 429, Description = "Проста система для радикальних змін у поведінці та досягненні цілей.",
                    ImageUrl = "https://picsum.photos/seed/atomic-habits/400/400",
                    CategoryId = CatId("Книги"), Quantity = 85
                },
                new() {
                    Name = "Думай і багатій — Наполеон Гілл",
                    Price = 359, Description = "Класика мотиваційної літератури про 13 принципів успіху.",
                    ImageUrl = "https://picsum.photos/seed/think-grow-rich/400/400",
                    CategoryId = CatId("Книги"), Quantity = 70
                },
                new() {
                    Name = "Майстер і Маргарита — Булгаков",
                    Price = 299, Description = "Культовий роман про добро, зло та творчість.",
                    ImageUrl = "https://picsum.photos/seed/master-margarita/400/400",
                    CategoryId = CatId("Книги"), Quantity = 60
                },
                new() {
                    Name = "1984 — Джордж Орвелл",
                    Price = 289, Description = "Антиутопія про тоталітарне суспільство, мову і свободу думки.",
                    ImageUrl = "https://picsum.photos/seed/orwell-1984/400/400",
                    CategoryId = CatId("Книги"), Quantity = 90
                },

                // ── Спорт ────────────────────────────────────────────────
                new() {
                    Name = "Гантелі в наборі 2×20 кг",
                    Price = 3899, Description = "Покриті гумою, ергономічна рукоятка, набір 2+2+4+6+8+10×2 кг.",
                    ImageUrl = "https://picsum.photos/seed/dumbbells-set/400/400",
                    CategoryId = CatId("Спорт"), Quantity = 15
                },
                new() {
                    Name = "Йога-мат GAIAM Premium 6 мм",
                    Price = 1299, Description = "Нековзке покриття, 6 мм товщина, 173×61 см, + ремінець.",
                    ImageUrl = "https://picsum.photos/seed/yoga-mat-gaiam/400/400",
                    CategoryId = CatId("Спорт"), Quantity = 48
                },
                new() {
                    Name = "Велотренажер Decathlon Essential",
                    Price = 8499, Description = "8 рівнів опору, LCD-дисплей, безшумна стрічка, до 120 кг.",
                    ImageUrl = "https://picsum.photos/seed/exercise-bike/400/400",
                    CategoryId = CatId("Спорт"), Quantity = 6
                },
                new() {
                    Name = "Скакалка швидкісна Crossrope",
                    Price = 899, Description = "Алюмінієві ручки, підшипники, довжина регулюється.",
                    ImageUrl = "https://picsum.photos/seed/jump-rope/400/400",
                    CategoryId = CatId("Спорт"), Quantity = 70
                },
                new() {
                    Name = "Рюкзак для туризму Osprey Talon 44",
                    Price = 6999, Description = "44 л, анатомічна рама, вентиляція AirSpeed, вага 1.25 кг.",
                    ImageUrl = "https://picsum.photos/seed/osprey-talon/400/400",
                    CategoryId = CatId("Спорт"), Quantity = 12
                },

                // ── Косметика ────────────────────────────────────────────
                new() {
                    Name = "Сироватка CeraVe з гіалуроновою кислотою",
                    Price = 899, Description = "3 типи гіалуронової кислоти, ніацинамід, зволожує 24 год.",
                    ImageUrl = "https://picsum.photos/seed/cerave-serum/400/400",
                    CategoryId = CatId("Косметика"), Quantity = 90
                },
                new() {
                    Name = "Сонцезахисний крем La Roche-Posay SPF50+",
                    Price = 729, Description = "Ультралегка текстура, водостійкий, підходить для чутливої шкіри.",
                    ImageUrl = "https://picsum.photos/seed/laroche-spf/400/400",
                    CategoryId = CatId("Косметика"), Quantity = 75
                },
                new() {
                    Name = "Палетка тіней Urban Decay Naked",
                    Price = 2199, Description = "12 відтінків від нюд до смок, формула High-Pigment.",
                    ImageUrl = "https://picsum.photos/seed/urban-decay-palette/400/400",
                    CategoryId = CatId("Косметика"), Quantity = 30
                },
                new() {
                    Name = "Парфуми Chanel Chance Eau Tendre 100 мл",
                    Price = 6499, Description = "Квітково-цитрусовий аромат, свіжий та жіночний шлейф.",
                    ImageUrl = "https://picsum.photos/seed/chanel-chance/400/400",
                    CategoryId = CatId("Косметика"), Quantity = 14
                },

                // ── Меблі ────────────────────────────────────────────────
                new() {
                    Name = "Офісне крісло Herman Miller Aeron",
                    Price = 42999, Description = "Постуральна підтримка, 8Z Pellicle, налаштування під тіло.",
                    ImageUrl = "https://picsum.photos/seed/herman-miller/400/400",
                    CategoryId = CatId("Меблі"), Quantity = 5
                },
                new() {
                    Name = "Стоячий стіл FlexiSpot E7",
                    Price = 18999, Description = "Електрорегулювання висоти 60–125 см, 2 двигуни, пам'ять 4 позицій.",
                    ImageUrl = "https://picsum.photos/seed/flexispot-desk/400/400",
                    CategoryId = CatId("Меблі"), Quantity = 9
                },
                new() {
                    Name = "Диван IKEA KIVIK 3-місний",
                    Price = 14999, Description = "Глибокі сидіння, знімні чохли, 10 варіантів тканини.",
                    ImageUrl = "https://picsum.photos/seed/ikea-kivik/400/400",
                    CategoryId = CatId("Меблі"), Quantity = 4
                },
                new() {
                    Name = "Книжкова полиця Kallax 4×4 IKEA",
                    Price = 4499, Description = "16 комірок, 147×147 см, можна використовувати вертикально та горизонтально.",
                    ImageUrl = "https://picsum.photos/seed/ikea-kallax/400/400",
                    CategoryId = CatId("Меблі"), Quantity = 11
                },
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        public static async Task SeedToysAsync(AppDbContext context)
        {
            var cats = await context.Categories.ToListAsync();
            var toysCat = cats.FirstOrDefault(c => c.Name == "Іграшки");
            if (toysCat == null) return;

            if (await context.Products.AnyAsync(p => p.CategoryId == toysCat.Id)) return;

            var toys = new List<Product>
            {
                new() {
                    Name = "LEGO Technic Bugatti Chiron 42083",
                    Price = 8999, Description = "3599 деталей, масштаб 1:8, функціональна коробка передач, рухомий двигун W16.",
                    ImageUrl = "https://picsum.photos/seed/lego-bugatti/400/400",
                    CategoryId = toysCat.Id, Quantity = 12
                },
                new() {
                    Name = "Конструктор LEGO Star Wars Тисячолітній Сокіл",
                    Price = 15999, Description = "7541 деталей, культовий корабель із серії Зоряних Воєн, колекційна модель.",
                    ImageUrl = "https://picsum.photos/seed/lego-falcon/400/400",
                    CategoryId = toysCat.Id, Quantity = 6
                },
                new() {
                    Name = "Радіокерована машинка Traxxas Slash 4×4",
                    Price = 6499, Description = "Масштаб 1:10, 4WD, швидкість до 60 км/год, водонепроникний корпус.",
                    ImageUrl = "https://picsum.photos/seed/traxxas-slash/400/400",
                    CategoryId = toysCat.Id, Quantity = 8
                },
                new() {
                    Name = "Інтерактивна лялька Baby Born 43 см",
                    Price = 2299, Description = "П'є і мочиться, закриває очі, плаче зі справжніми слізьми, 9 функцій.",
                    ImageUrl = "https://picsum.photos/seed/baby-born/400/400",
                    CategoryId = toysCat.Id, Quantity = 25
                },
                new() {
                    Name = "Настільна гра Monopoly Classic",
                    Price = 999, Description = "Класична версія, 2–8 гравців, вік від 8 років, українська мова.",
                    ImageUrl = "https://picsum.photos/seed/monopoly-classic/400/400",
                    CategoryId = toysCat.Id, Quantity = 40
                },
            };

            await context.Products.AddRangeAsync(toys);
            await context.SaveChangesAsync();
        }
    }
}
