using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Book_Store.Filters.AdminSessionFilter;
using Book_Store.Models;

namespace Book_Store.Controllers
{
    [ClassAdminSessionFilter]
    public class ManageOrdersController : Controller
    {
        private readonly Book_Store_Context _context;

        public ManageOrdersController(Book_Store_Context context)
        {
            _context = context;
        }

        // GET: ManageOrders
        public async Task<IActionResult> Index()
        {
            var sportsWearContext = _context.Orders.Include(o => o.FkCustomer);
            return View(await sportsWearContext.ToListAsync());
        }

        // GET: ManageOrders/Details/5
        public IActionResult Details(int? id)
        {
            //admin can see details about order, how much specific customer bought the books
            if (id == null)
            {
                return NotFound();
            }

            var order_details = (from orders in _context.Orders
                                 join order_detail
                                 in _context.OrderDetails on orders.OrderId equals
                                 order_detail.FkOrderId
                                 join book in _context.Books on order_detail.FkBookId equals book.BookId
                                 where orders.OrderId == id
                                 select new Book
                                 {
                                     BookId=book.BookId,
                                     BookName = book.BookName,
                                     BookPrice = book.BookPrice,
                                     BookQty = order_detail.Qty
                                 }).ToList();

            return View(order_details);
        }


        // GET: ManageOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: ManageOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,FullName,PhoneNumber,AddressDetail,OrderStatus")] Order order)
        {
            //admin can process customer order, what status is here
            //0: Stands for (Received)
            //1: Stands for (Processing)
            //2: Stands for (Shipped)
            //3: Stands for (Cancelled)
            //4: Stands for (Completed)
            var _contextOrder = _context.Orders.Find(id);
            _contextOrder.FullName = order.FullName;
            _contextOrder.PhoneNumber = order.PhoneNumber;
            _contextOrder.AddressDetail = order.AddressDetail;
            _contextOrder.OrderStatus = order.OrderStatus;
            if (order.OrderStatus == 3)
            {
                //if admin cancelled the customer order, then quantity will be increased in Book Table 
                var result = (from orders in _context.Orders
                              join order_detail
                              in _context.OrderDetails on orders.OrderId equals
                              order_detail.FkOrderId
                              where orders.OrderId == id
                              select new
                              {
                                  bookId = order_detail.FkBookId,
                                  bookQty = order_detail.Qty
                              }).ToList();
                foreach (var item in result)
                {
                    var _contextBookQty = _context.Books.Where(x => x.BookId == item.bookId).FirstOrDefault();
                    _contextBookQty.BookQty = _contextBookQty.BookQty + item.bookQty;
                    _context.SaveChanges();
                }
            }
            _context.Orders.Update(_contextOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ManageOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.FkCustomer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: ManageOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
