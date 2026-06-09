using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers;

/// <summary>
/// Handles the checkout flow: order form and confirmation page.
/// All cart/order data is currently managed client-side (localStorage).
/// When the backend (BLL/DAL) is ready, replace localStorage logic
/// with proper service calls here.
/// </summary>
public class CheckoutController : Controller
{
    /// <summary>Checkout form — shipping address + payment method.</summary>
    public IActionResult Index() => View();

    /// <summary>Order confirmation page shown after successful checkout.</summary>
    public IActionResult Confirm() => View();
}
