using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using otce_project_final.Data;
using otce_project_final.Models;
using otce_project_final.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace otce_project_final.Controllers
{
    [Authorize(Roles = GlobalConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly AppDbContext _rdb;

        public ProductController(AppDbContext db, IWebHostEnvironment webHostEnv)
        {
            _rdb = db;
            _webHostEnv = webHostEnv;
        }

        /* Category Mainpage - GET */
        public IActionResult Index()
        {
            /* enumerable method - use eager loading instead 
            IEnumerable<Product> list = _rdb.Product;

            foreach(var product in list)
            {
                product.Category = _rdb.Category.FirstOrDefault(c => c.Id == product.CategoryId);
            }; */

            IEnumerable<Product> list = _rdb.Product.Include(c => c.Category);
            return View(list);
        }

        /* CreateUpdate Product - GET */
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM pvm = new ProductVM()
            {
                Product = new Product(),
                CategorySelector = _rdb.Category.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            if( id == null)
            {
                /* Create New Product */
                return View(pvm);
            }
            else
            {
                pvm.Product = _rdb.Product.Find(id);

                if(pvm.Product==null) { return NotFound();  }
                return View(pvm);
            }
        }        

        /* CreateUpdate Product - POST */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductVM pvm)
        {
            if( ModelState.IsValid )
            {
                var files = HttpContext.Request.Form.Files;

                // path to wwwroot to grab images folder
                string DocumentRoot = _webHostEnv.WebRootPath;

                if( pvm.Product.Id == 0 )
                {
                    /* IF-CASE: Creating */

                    // relative path to product images
                    string uploadPath = DocumentRoot + GlobalConstants.ImagePath;
                    string fName = Guid.NewGuid().ToString();
                    string fType = Path.GetExtension(files[0].FileName);

                    using (var fStream = new FileStream( Path.Combine(uploadPath, fName + fType), FileMode.Create ))
                    {
                        files[0].CopyTo(fStream);
                    }

                    //pvm.Product.Image = uploadPath + fName + fType;
                    pvm.Product.Image = fName + fType;

                    _rdb.Product.Add(pvm.Product);
                }
                else
                {
                    /* IF-CASE: Updating */

                    // use AsNoTracking to prevent duplicate tracking on local Product vs ProductViewModel
                    var product = _rdb.Product.AsNoTracking().FirstOrDefault(m => m.Id == pvm.Product.Id);

                    if( files.Count > 0)
                    {
                        // relative path to product images
                        string uploadPath = DocumentRoot + GlobalConstants.ImagePath;
                        string fName = Guid.NewGuid().ToString();
                        string fType = Path.GetExtension(files[0].FileName);

                        var fOld = Path.Combine(uploadPath, product.Image);

                        // delete existing image
                        if( System.IO.File.Exists(fOld))
                        {
                            System.IO.File.Delete(fOld);
                        }

                        // update to new image
                        using (var fStream = new FileStream( Path.Combine(uploadPath, fName + fType), FileMode.Create ))
                        {
                            files[0].CopyTo(fStream);
                        }

                        pvm.Product.Image = fName + fType;
                    }
                    else
                    {
                        pvm.Product.Image = product.Image;
                    }

                    _rdb.Product.Update(pvm.Product);
                }

                _rdb.SaveChanges();
                return RedirectToAction("Index");
            }

            // if model state was invalid, make sure to still initialize category dropdown
            pvm.CategorySelector = _rdb.Category.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            return View(pvm);
        }
    
        /* Delete Product - GET */
        public IActionResult Delete(int? id)
        {
            // semi-validate ID
            if( id == 0 || id == null) { return NotFound(); }

            // eager load category
            Product product = _rdb.Product.Include(c => c.Category).FirstOrDefault(i => i.Id == id); 

            if (product == null) { return NotFound(); }

            return View(product);
        }

        /* Delete Product - POST */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int? id)
        {
            var product = _rdb.Product.Find(id);
            if (product == null) { return NotFound(); }

            // relative path to product images
            string uploadPath = _webHostEnv.WebRootPath + GlobalConstants.ImagePath;

            var mFile = Path.Combine(uploadPath, product.Image);

            // delete existing image
            if( System.IO.File.Exists(mFile))
            {
                System.IO.File.Delete(mFile);
            }

            _rdb.Product.Remove(product);
            _rdb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}