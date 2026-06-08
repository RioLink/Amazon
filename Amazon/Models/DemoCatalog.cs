namespace Amazon.Models;

public static class DemoCatalog
{
    public static IReadOnlyList<ProductCardViewModel> Products { get; } =
    [
        new()
        {
            Id = 1,
            Name = "Бездротові навушники SoundMax Pro",
            Category = "Електроніка",
            ImageUrl = "/images/headphones.svg",
            Price = 3499,
            OldPrice = 4199,
            Rating = 4.8,
            ReviewCount = 328,
            Description = "Активне шумозаглушення, до 30 годин роботи та швидке заряджання."
        },
        new()
        {
            Id = 2,
            Name = "Смарт-годинник Active Watch 5",
            Category = "Гаджети",
            ImageUrl = "/images/smartwatch.svg",
            Price = 5899,
            OldPrice = 6499,
            Rating = 4.7,
            ReviewCount = 214,
            Description = "Яскравий AMOLED-дисплей, GPS і моніторинг активності протягом дня."
        },
        new()
        {
            Id = 3,
            Name = "Механічна клавіатура KeyPro TKL",
            Category = "Комп'ютери",
            ImageUrl = "/images/keyboard.svg",
            Price = 2799,
            Rating = 4.6,
            ReviewCount = 176,
            Description = "Компактний формат, гаряча заміна перемикачів і RGB-підсвічування."
        },
        new()
        {
            Id = 4,
            Name = "Портативна колонка Wave Mini",
            Category = "Аудіо",
            ImageUrl = "/images/speaker.svg",
            Price = 2199,
            OldPrice = 2599,
            Rating = 4.9,
            ReviewCount = 452,
            Description = "Насичений звук, захист від води IPX7 і до 18 годин автономної роботи."
        }
    ];
}
