using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bangazon.Models.ProductViewModels;

namespace Bangazon.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //code to grab user
        //  var user = await GetCurrentUserAsync();

        // GET: Products
        public async Task<IActionResult> Index(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                var applicationDbContext = _context.Product
                    .Include(p => p.ProductType)
                    .Include(p => p.User);

                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                if (!CityExist(search))
                {
                    var applicationDbContext = _context.Product
                        .Include(p => p.ProductType)
                        .Include(p => p.User)
                        .Where(p => p.Title.Contains(search));

                    return View(await applicationDbContext.ToListAsync());
                }
                else
                {
                    var applicationDbContext = _context.Product
                        .Include(p => p.ProductType)
                        .Include(p => p.User)
                        .Where(p => p.City.Equals(search));

                    return View(await applicationDbContext.ToListAsync());
                }
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
                   .Include(p => p.ProductType)
                   .Include(p => p.User)
                   .Include(p => p.OrderProducts)
                   .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            var productDetail = new ProductDetailViewModel()
            {
                Product = product,
                Inventory = product.Quantity - product.OrderProducts.Count()
            }; return View(productDetail);
        }
        //
        // GET: Products/Create
        public IActionResult Create()
        {
            //var selectList = new SelectList(_context.ProductType, "ProductTypeId", "Label");
            //SelectListItem newItem = new SelectListItem { Text = "Please choose product type", Value = "0" };

            //ViewData["ProductTypeId"] = new SelectList(selectList, new SelectListItem { Text = "Please choose product type", Value = "0" });

            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", new SelectListItem { Value = "0", Text = "fake" });
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,LocalDeliveryAvailable,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,Active,ProductTypeId")] Product product)
        {
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                product.UserId = currentUser.Id;

                _context.Add(product);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Details), new { id = product.ProductId });
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();

            if ( !product.UserId.Equals(currentUser.Id) )
            {
                return NotFound("Product does not belong to current user");
            }

            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,LocalDeliveryAvailable,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,Active,ProductTypeId")] Product product)
        {
            var currentUser = await GetCurrentUserAsync();

            if ( !product.UserId.Equals( currentUser.Id ) )
            {
                return NotFound("Product does not belong to current user");
            }

            if (id != product.ProductId)
            {
                return NotFound("Wrong product Id");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound("There no this product in table");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
        private bool CityExist(string city)
        {
            return _context.Product.Any(e => e.City.Equals(city));
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);



    }
}
