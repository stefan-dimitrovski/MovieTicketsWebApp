using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieShop.Domain.DomainModels;
using MovieShop.Domain.DTO;
using MovieShop.Services.Interface;

namespace MovieShop.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            var allTickets = this._ticketService.GetAllTickets();
            return View(allTickets);
        }

        [Authorize(Roles = "Admin")]
        public FileContentResult ExportTickets(string genre)
        {
            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            List<Ticket> allTickets;

            if (genre.Equals("All"))
            {
                allTickets = this._ticketService.GetAllTickets().ToList();
            }
            else
            {
                allTickets = this._ticketService.GetAllTickets().Where(z => z.Genre.ToString().Equals(genre)).ToList();
            }

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Tickets");

                worksheet.Cell(1, 1).Value = "Ticket Id";
                worksheet.Cell(1, 2).Value = "Movie";
                worksheet.Cell(1, 3).Value = "Genre";
                worksheet.Cell(1, 4).Value = "Date";
                worksheet.Cell(1, 5).Value = "Price";


                for (int i = 1; i <= allTickets.Count(); i++)
                {
                    var item = allTickets[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id;
                    worksheet.Cell(i + 1, 2).Value = item.Movie;
                    worksheet.Cell(i + 1, 3).Value = item.Genre;
                    worksheet.Cell(i + 1, 4).Value = item.ValidTime;
                    worksheet.Cell(i + 1, 5).Value = item.Price;
   
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }

        }

        public IActionResult GetValidTickets()
        {
            var model = this._ticketService.GetValidTickets();

            return View("~/Views/Tickets/Index.cshtml", model);
        }

        [Authorize]
        public IActionResult AddTicketToCart(Guid? id)
        {
            var model = this._ticketService.GetShoppingCartInfo(id);
            return View(model);
        }
 
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCart([Bind("TicketId,Quantity")] AddToShoppingCartDto item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._ticketService.AddToShoppingCart(item, userId);

            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }

            return View(item);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Movie,ValidTime,Price,Genre")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                this._ticketService.CreateNewTicket(ticket);
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,Movie,ValidTime,Price,Genre")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._ticketService.UpdeteExistingTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsForTicket(id) != null;
        }
    }
}
