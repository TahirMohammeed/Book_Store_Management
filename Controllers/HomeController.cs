using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Book_Store.Filters.CustomerSessionFilter;
using Book_Store.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Book_Store_Context _context;
        public HomeController(ILogger<HomeController> logger, Book_Store_Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            var customerId = HttpContext.Session.GetString("customerId");
            if (customerId != null)
            {
                //this method will be used to check the cart items for specific customer
                var result = CustomersCartItems();
                int totalCartItems = result.Count;
                HttpContext.Session.SetString("totalCartItems", totalCartItems.ToString());
            }
            ViewBag.Categories = _context.Categories.OrderBy(x=>x.CategoryId).ToList();
            if (id != null)
            {
                var _categoryContext = _context.Categories.Find(id);
                if (_categoryContext != null)
                {
                    ViewBag.categoryName = _categoryContext.CategoryName;
                    var _contextBooks = _context.Books.Include(p => p.FkCategory).Where(x => x.FkCategoryId == id);
                    return View(_contextBooks.ToList());
                }
            }
            else
            {
                var _contextBooks = _context.Books.Include(p => p.FkCategory);
                return View(_contextBooks.ToList());
            }
            return View();
        }


        [ClassCustomerSessionFilter]
        public  IActionResult Cart()
        { 
            //return the cart view, also cart products for specific customer
            var result = CustomersCartItems();
            int totalCartItems=result.Count;
            HttpContext.Session.SetString("totalCartItems", totalCartItems.ToString());
            return View(result);
        }
        [HttpPost]
        [ClassCustomerSessionFilter]
        public  IActionResult AddToCart(int bookId, int bookQty)
        {
            //add items into cart
            //if product quantity will be 0, then customer will not able to add book into cart
            var customerId =HttpContext.Session.GetString("customerId");
            var _contextBook = _context.Books.Find(bookId);
            if (_contextBook.BookQty < bookQty)
            {
                TempData["itemOutOffStock"] = "Hi, item is out of stock";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var _contextCart = _context.Carts.Where(x => x.FkBookId == bookId && x.FkCustomerId==Int32.Parse(customerId)).FirstOrDefault();
                if (_contextCart != null)
                {
                    //returing the total price and quanity for cart for specific customer,
                    //if book is already added into then only quanity will be increased for specific book
                    _contextCart.TotalPrice = _contextCart.TotalPrice+( _contextBook.BookPrice * bookQty);
                    _contextCart.Qty = _contextCart.Qty + bookQty;
                    _context.Carts.Update(_contextCart);
                    //updating the quantity after adding the book into cart
                    _contextBook.BookQty = _contextBook.BookQty - bookQty;
                    _context.Books.Update(_contextBook);
                    _context.SaveChanges();
                }
                else
                {
                    //if book is adding first time into cart then this condition will be executed, for specific customer
                    var data = new Cart();
                    data.FkCustomerId = Int32.Parse(customerId);
                    data.FkBookId = bookId;
                    data.Qty = bookQty;
                    data.TotalPrice = _contextBook.BookPrice * bookQty;
                    _context.Carts.Add(data);
                    _contextBook.BookQty = _contextBook.BookQty - bookQty;
                    _context.Books.Update(_contextBook);
                    _context.SaveChanges();
                }
                TempData["itemAddedIntoCart"] = "Book is added into cart";
                return RedirectToAction("Index", "Home");
            }
        }
        [ClassCustomerSessionFilter]

        public IActionResult RemoveFromCart(int id)
        {
            //remoing item from cart, if cart item is removed then quantity will increased 
            //for specific product that customer was selected
            var _contextCart = _context.Carts.Find(id);
            var _contextBook = _context.Books.Find(_contextCart.FkBookId);
            _contextBook.BookQty = _contextBook.BookQty + _contextCart.Qty;
            _context.Books.Update(_contextBook);
            _context.Carts.Remove(_contextCart);
            _context.SaveChanges();
            return RedirectToAction("Cart", "Home");
        }

        public IActionResult DecreaseQuantity(int id)
        {
            //if customer want to decrease quantity for specific product then customer can decreased
            var _contextCart = _context.Carts.Find(id);
            var _contextBook = _context.Books.Find(_contextCart.FkBookId);
            if (_contextCart.Qty<2)
            {
                //quantity can be decreased less then by 1, 1 quantity will be must for specific product
                TempData["cartBookQntyCount"] = "Hi, quantity can not be decrease, minimum should be 1 for book";
            }
            else
            {
                //otherwise quantity will be decreased from cart, also from Book table
                _contextBook.BookQty = _contextBook.BookQty + 1;
                _contextCart.Qty = _contextCart.Qty - 1;
                _contextCart.TotalPrice = _contextCart.TotalPrice - _contextBook.BookPrice;
                _context.Books.Update(_contextBook);
                _context.Carts.Update(_contextCart);
                _context.SaveChanges();
            }
            
            return RedirectToAction("Cart", "Home");
        }
        public IActionResult IncreaseQuantity(int id)
        {
            //same as above
            var _contextCart = _context.Carts.Find(id);
            var _contextBook = _context.Books.Find(_contextCart.FkBookId);
            if (_contextBook.BookQty>0)
            {
                _contextBook.BookQty = _contextBook.BookQty - 1;
                _contextCart.Qty = _contextCart.Qty + 1;
                _contextCart.TotalPrice = _contextCart.TotalPrice + _contextBook.BookPrice;
                _context.Books.Update(_contextBook);
                _context.Carts.Update(_contextCart);
                _context.SaveChanges();
            }
            else
            {
                //if customer demand more quantity that is not available then this message will be shown
                TempData["itemOutOffStock"] = "Hi, item is out of stock";
            }

            return RedirectToAction("Cart", "Home");
        }
        [HttpGet]
        [ClassCustomerSessionFilter]
        public IActionResult Checkout()
        {
            //return the checkout view and cart items
            var result = CustomersCartItems();
            ViewBag.checkOutItems = result;
            return View();
        }

   

        [HttpPost]
        [ClassCustomerSessionFilter]
        public IActionResult Checkout(Order order, string chargeTotalAmount, string stripeToken, string stripeEmail)
        {
            //cheking out the products and processing the payments
            return ProcessPaymentsAndOrder(order, chargeTotalAmount,stripeToken, stripeEmail);
        }


        public IActionResult ProcessPaymentsAndOrder(Order order, string chargeTotalAmount, string stripeToken, string stripeEmail)
        {
            //processing the payments, payment will be transfer to your account at stripe.net
            //here i'm using stripe payment gateway
            var optionsCust = new Stripe.CustomerCreateOptions
            {
                Email = stripeEmail,
                Name = order.FullName,
                Phone = order.PhoneNumber

            };
            var serviceCust = new Stripe.CustomerService();

            Stripe.Customer customer = serviceCust.Create(optionsCust);
            var optionsCharge = new Stripe.ChargeCreateOptions
            {
                Amount = long.Parse(chargeTotalAmount),
                Currency = "USD",
                Description = "Buying EBooks Books",
                Source = stripeToken,
                ReceiptEmail = stripeEmail,

            };
            var service = new Stripe.ChargeService();
            Stripe.Charge charge = service.Create(optionsCharge);
            if (charge.Status == "succeeded")
            {
                //if payments made succeeded, then items from cart will be remove for specfic customer
                //also their orders, and orders details will be added into orders, order_details table
                var customerId = Int32.Parse(HttpContext.Session.GetString("customerId"));
                double subTotals = _context.Carts.Where(c => c.FkCustomerId == customerId)
                .Select(i => Convert.ToDouble(i.TotalPrice)).Sum();
                order.FkCustomerId = customerId;
                order.TotalPrice = subTotals;
                order.OrderDate = DateTime.Now.ToString();
                order.OrderStatus = 0;
                _context.Orders.Update(order);
                _context.SaveChanges();
                var _contextCart = _context.Carts.Where(x => x.FkCustomerId == customerId).ToList();
                foreach (var items in _contextCart)
                {
                    var itemPrice = _context.Books.Find(items.FkBookId);
                    var orderDetail = new OrderDetail();
                    orderDetail.FkOrderId = order.OrderId;
                    orderDetail.FkBookId = items.FkBookId;
                    orderDetail.Qty = items.Qty;
                    orderDetail.Price = itemPrice.BookPrice;
                    _context.OrderDetails.Add(orderDetail);
                    _context.SaveChanges();
                    _context.Carts.Remove(items);
                    _context.SaveChanges();
                }
                TempData["checkoutConfirm"] = "Your order has been confirmed successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["checkoutConfirm"] = "Something went wrong, while processing with payment";
                return RedirectToAction("Index", "Home");
            }

        }



        public List<CustomerCartItems> CustomersCartItems()
        {
            //returing the all cart items for spefcific product
            var customerId = HttpContext.Session.GetString("customerId");
            var result = (from Books in _context.Books
                          join cart in _context.Carts
                          on Books.BookId equals cart.FkBookId
                          join customer in _context.Customers on cart.FkCustomerId equals customer.CustomerId
                          where customer.CustomerId == Int32.Parse(customerId)
                          select new CustomerCartItems
                          {
                              cartId = cart.CartId,
                              customerId = customer.CustomerId,
                              bookId = Books.BookId,
                              bookName = Books.BookName,
                              bookPrice = Books.BookPrice,
                              cartQty = cart.Qty,
                              bookTotalPrice = cart.TotalPrice

                          }).ToList();
            return result;
        }
        [ClassCustomerSessionFilter]
        public IActionResult MyOrders()
        {
            var customerId = Int32.Parse(HttpContext.Session.GetString("customerId"));
            var result = _context.Orders.Where(x => x.FkCustomerId == customerId).OrderByDescending(x=>x.OrderId).ToList();
            return View(result);
        }
        [ClassCustomerSessionFilter]
        public IActionResult CancelOrder(int id)
        {
            //customer can also cancel their order, if order is not processed by admin
            //Order Status: 0:Received, 1:Processing, 2:Shipped, 3:Cancelled, 4:Completed
            //if Order status is (Received) then customer can cancel their order
            var _contextOrder = _context.Orders.Find(id);
            _contextOrder.OrderStatus = 3;
            _context.Orders.Update(_contextOrder);
            _context.SaveChanges();
            var result = (from orders in _context.Orders
                          join order_detail
                          in _context.OrderDetails on orders.OrderId equals
                          order_detail.FkOrderId
                          where orders.OrderId == id
                          select new
                          {
                              bookId=order_detail.FkBookId,
                              bookQty=order_detail.Qty
                          }).ToList();
            foreach(var item in result)
            {
                var _contextBookQty = _context.Books.Where(x => x.BookId == item.bookId).FirstOrDefault();
                _contextBookQty.BookQty = _contextBookQty.BookQty + item.bookQty;
                _context.SaveChanges();
            }
            TempData["orderCancelled"] = "Your order has been cancelled";
            return RedirectToAction("MyOrders");
        }


        // GET: Home/OrderDetails/5
        public IActionResult OrderDetails(int? id)
        {
            //return the order details, that customer has checkout, 
            //all details will be return how much books he bought
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
                                     BookId = book.BookId,
                                     BookName = book.BookName,
                                     BookPrice = book.BookPrice,
                                     BookQty = order_detail.Qty
                                 }).ToList();

            return View(order_details);
        }

        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            //customer can search about product
            var result=_context.Books.Where(x => EF.Functions.Like(x.BookName, $"%{searchQuery}%")).ToList();
            ViewBag.searchQuery = searchQuery;
            return View(result);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
