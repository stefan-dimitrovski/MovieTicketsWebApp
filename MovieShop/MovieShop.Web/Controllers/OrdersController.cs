using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Domain.DomainModels;
using MovieShop.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace MovieShop.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._orderService.GetAllOrders(userId);

            return View(result);
        }

        [Authorize]
        public IActionResult Details(BaseEntity orderId)
        {
            return View(this._orderService.GetOrderDetails(orderId));
        }

        [Authorize(Roles = "Admin")]
        public FileContentResult CreateInvoice(BaseEntity orderId)
        {
            var order = this._orderService.GetOrderDetails(orderId);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
            document.Content.Replace("{{UserName}}", order.User.UserName.ToString());

            StringBuilder sb = new StringBuilder();

            var totalPrice = 0.0;

            foreach (var item in order.Tickets)
            {
                totalPrice += item.Quantity * item.SelectedTicket.Price;
                sb.AppendLine(item.SelectedTicket.Movie + " with quantity of: " + item.Quantity + " and price of: " + item.SelectedTicket.Price + "$");
            }

            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "$");

            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }
    }
}
