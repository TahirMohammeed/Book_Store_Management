using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Book_Store.Filters.AdminSessionFilter;
using Book_Store.Models;

namespace Book_Store.Controllers
{
    [ClassAdminSessionFilter]
    public class ManageBooksController : Controller
    {
        private readonly Book_Store_Context _context;


        private readonly IWebHostEnvironment _hostEnvironment;

      
        public ManageBooksController(Book_Store_Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: ManageBooks
        public async Task<IActionResult> Index()
        {
            var sportsWearContext = _context.Books.Include(p => p.FkCategory);
            return View(await sportsWearContext.ToListAsync());
        }

        // GET: ManageBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(p => p.FkCategory)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: ManageBooks/Create
        public IActionResult Create()
        {
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: ManageBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,FkCategoryId,BookName,BookPrice,BookQty,BookAuthor,ImageFile,BookDesc")] Book book)
        {
            if (ModelState.IsValid)
            {
                if (book.ImageFile != null)
                {
                    //save image to folder wwwroth
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                    string extention = Path.GetExtension(book.ImageFile.FileName);
                    book.BookImage = fileName = Guid.NewGuid().ToString() + "_" + fileName + extention;
                    string path = Path.Combine(wwwRootPath + "/images/uploads/bookImages/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.message = "Please select the image";
                }

            }

            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", book.FkCategoryId);
            return View(book);
        }

        // GET: ManageBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", book.FkCategoryId);
            return View(book);
        }

        // POST: ManageBooks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,FkCategoryId,BookName,BookPrice,BookQty,BookAuthor,BookImage,ImageFile,BookDesc")] Book book)
        {
            //admin can edit the books details, also change the image for specfic book
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (book.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    var deleteImagepath = Path.Combine(_hostEnvironment.ContentRootPath, wwwRootPath + "/images/uploads/bookImages/", book.BookImage);

                    //existing image wil be deleted if admin select the new image
                    if (System.IO.File.Exists(deleteImagepath))
                    {
                        System.IO.File.Delete(deleteImagepath);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                    string extention = Path.GetExtension(book.ImageFile.FileName);
                    book.BookImage = fileName = Guid.NewGuid().ToString() + "_" + fileName + extention;
                    string path = Path.Combine(wwwRootPath + "/images/uploads/bookImages/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }
                }
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", book.FkCategoryId);
            return View(book);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(p => p.FkCategory)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: ManageBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var book = await _context.Books.FindAsync(id);
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var deleteImagepath = Path.Combine(_hostEnvironment.ContentRootPath, wwwRootPath + "/images/uploads/bookImages/", book.BookImage);

            if (System.IO.File.Exists(deleteImagepath))
            {
                System.IO.File.Delete(deleteImagepath);
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
