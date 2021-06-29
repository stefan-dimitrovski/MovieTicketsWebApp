using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Services.Interface;
using Stripe;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace EShop.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {

        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(this._shoppingCartService.getShoppingCartInfo(userId));
        }

        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = this._shoppingCartService.getShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.TotalPrice) * 100),
                Description = "Ticket Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if(charge.Status == "succeeded")
            {
                var result = this.OrderNow();

                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteFromShoppingCart(Guid ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.deleteTicketFromShoppingCart(userId, ticketId);

            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private bool OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._shoppingCartService.orderNow(userId);

            return result;
        }

    }
}
