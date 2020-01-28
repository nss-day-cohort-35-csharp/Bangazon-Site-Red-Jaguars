using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Bangazon.Models.OrderViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Bangazon.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            
            var user = await GetCurrentUserAsync();

            

            var applicationDbContext = _context.Order
                                .Include(o => o.PaymentType)
                                .Include(o => o.User)
                                .Include(o => o.OrderProducts)
                                    .ThenInclude(op => op.Product)
                                .Where(m => m.UserId == user.Id && m.DateCompleted != null);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/
        public async Task<IActionResult> Details(int? id)
        {
            var user = await GetCurrentUserAsync();

            if (id > 0)
            {
                var completedOrder = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(m => m.UserId == user.Id && m.OrderId == id);
                if (completedOrder == null)
                {
                    return NotFound();
                }
                var completedShoppingCart = new OrderDetailViewModel(completedOrder);
                completedShoppingCart.LineItems = completedOrder.OrderProducts.GroupBy(op => op.ProductId)
                                   .Select(pr => new OrderLineItem
                                   {
                                       Units = pr.Count(),
                                       Cost = pr.Sum(c => c.Product.Price),
                                       Product = pr.First().Product


                                   }).ToList();

                return View(completedShoppingCart);
            }
            else
            {
                var order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(m => m.UserId == user.Id && m.DateCompleted == null);
                if (order == null)
                {
                    TempData["ErrorMessage"] = $"Sorry {user.FirstName}, your shopping cart is empty!"; 
                    return RedirectToAction("Index");
                    
                }

                if (order.OrderProducts.Count() == 0)
                {
                    TempData["ErrorMessage"] = $"Sorry {user.FirstName}, your shopping cart is empty!";
                    return RedirectToAction("Index");

                }

                var shoppingCart = new OrderDetailViewModel(order);
                shoppingCart.LineItems = order.OrderProducts.GroupBy(op => op.ProductId)
                                   .Select(pr => new OrderLineItem
                                   {
                                       Units = pr.Count(),
                                       Cost = pr.Sum(c => c.Product.Price),
                                       Product = pr.First().Product


                                   }).ToList();
               // ViewBag.Total = shoppingCart.LineItems.
                return View(shoppingCart);
            }

        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,DateCreated,DateCompleted,UserId,PaymentTypeId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToOrder(Product product)
        {
            var user = await GetCurrentUserAsync();

            //check if product is in stock

            var productToCheck = await _context.Product
               .Include(p => p.ProductType)
               .Include(p => p.OrderProducts)
               .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (productToCheck.Quantity <= productToCheck.OrderProducts.Count)
            {
                TempData["OutOfStock"] = $"Beep-Boop!  Sorry {user.FirstName}, This item is currently out of stock!";
                return RedirectToAction(nameof(Details));
            }

            

            //check to see if user has incompleted order
                var order = await _context.Order
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.DateCompleted == null);

            if (order == null)
            {
                //create order
                var newOrder = new Order
                {
                    DateCreated = DateTime.Now,
                    UserId = user.Id
                };
                _context.Add(newOrder);
                await _context.SaveChangesAsync();
                int id = newOrder.OrderId;

                //add product to order
                var newOrderProduct = new OrderProduct
                {
                    OrderId = id,
                    ProductId = product.ProductId
                };

                _context.OrderProduct.Add(newOrderProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details));

            } else
            {
                //add product to existing order
                var newOrderProduct = new OrderProduct
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId
                };

                _context.OrderProduct.Add(newOrderProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details));
            }
  
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            //var userPaymentTypes = _context.PaymentType.Include(p => p.User).Where(a => a.User.Id == user.Id);

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType.Include(p => p.User).Where(a => a.User.Id == user.Id), "PaymentTypeId", "PaymentDetails", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,DateCreated,UserId,PaymentTypeId")] Order order)
        {
            var user = await GetCurrentUserAsync();
            order.User = user;
            if (id != order.OrderId || order.UserId != user.Id)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));

                // Breakpoint, Log or examine the list with Exceptions.
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.DateCompleted = DateTime.Now;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Wooooo Hoo!  You have placed your order successfully.";
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType.Include(p => p.User).Where(a => a.User.Id == user.Id), "PaymentTypeId", "PaymentDetails", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(OrderProduct item)
        {
            var user = await GetCurrentUserAsync();

            var itemToDelete = await _context.OrderProduct
                           .FirstOrDefaultAsync(op => op.OrderProductId == item.OrderProductId);


            _context.OrderProduct.Remove(itemToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details));
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(OrderProduct item)
        {
            var user = await GetCurrentUserAsync();

            var itemsToDelete = _context.OrderProduct
                           .Where(op => op.OrderId == item.OrderId);

            foreach (OrderProduct product in itemsToDelete)
            {
                _context.OrderProduct.Remove(product);
               
            }
            await _context.SaveChangesAsync();

            var orderToDelete = _context.Order.Where(o => o.OrderId == item.OrderId && o.UserId == user.Id);

            foreach (Order order in orderToDelete)
            {
                _context.Order.Remove(order);
                
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
