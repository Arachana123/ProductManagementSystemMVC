using Microsoft.AspNetCore.Mvc;
using ProductManagementSystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ProductManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            db = context;
            _environment = environment;
        }

        public IActionResult Index(string searchString, int page = 1)
        {
            int pageSize = 5;

            var products = db.Products.OrderBy(p => p.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = Math.Ceiling(db.Products.Count() / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product p, IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                string folder = "images/";
                folder += Guid.NewGuid().ToString() + "_" + ImageFile.FileName;

                string serverFolder = Path.Combine(_environment.WebRootPath, folder);

                await ImageFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

                p.ImagePath = "/" + folder;
            }

            db.Products.Add(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = db.Products.Find(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product p)
        {
            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = db.Products.Find(id);

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}