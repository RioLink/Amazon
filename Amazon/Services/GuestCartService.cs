using Amazon.Models;
using System.Text.Json;

namespace Amazon.Services;

public static class GuestCartService
{
    private const string SessionKey = "guest-cart";

    public static List<CartItemViewModel> GetCart(ISession session)
    {
        var json = session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json)) return new();
        try { return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new(); }
        catch { return new(); }
    }

    public static void SaveCart(ISession session, List<CartItemViewModel> cart)
        => session.SetString(SessionKey, JsonSerializer.Serialize(cart));

    public static void Add(ISession session, int productId, string productName, decimal price, int quantity = 1, string? imageUrl = null)
    {
        var cart = GetCart(session);
        var existing = cart.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
            existing.Quantity += quantity;
        else
            cart.Add(new CartItemViewModel { ProductId = productId, ProductName = productName, Price = price, Quantity = quantity, ImageUrl = imageUrl });
        SaveCart(session, cart);
    }

    public static void Remove(ISession session, int productId)
    {
        var cart = GetCart(session);
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCart(session, cart);
    }

    public static void Clear(ISession session)
        => session.Remove(SessionKey);

    public static int GetCount(ISession session)
        => GetCart(session).Sum(i => i.Quantity);
}
