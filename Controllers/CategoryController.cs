using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using otce_project_final.Data;
using otce_project_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace otce_project_final.Controllers
{
    [Authorize(Roles = GlobalConstants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _rdb;

        public CategoryController(AppDbContext db)
        {
            _rdb = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> list = _rdb.Category;
            return View(list);
        }

        /* SHOW - create a new category page */
        public IActionResult Create()
        {
            return View();
        }


        /* SHOW - update an existing category page */
        public IActionResult Edit(int? id)
        {
            if( id == 0 || id == null ) { return NotFound();  }

            var category = _rdb.Category.Find(id);
            if( category == null ) { return NotFound();  }

            return View(category);
        }

        /* POST - CREATE new category to db */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category cat)
        {
            _rdb.Category.Add(cat);
            _rdb.SaveChanges();
            return RedirectToAction("Index");
        }

        /* POST - EDIT existing category */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category cat)
        {
            if(ModelState.IsValid)
            {
                _rdb.Category.Update(cat);
                _rdb.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat);
        }

        /* SHOW - delete page */
        public IActionResult Delete(int? id)
        {
            if( id == 0 || id == null) { return NotFound(); }

            var category = _rdb.Category.Find(id);
            if (category == null) { return NotFound(); }

            return View(category);
        }

        /* POST - delete category from db */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            var cat = _rdb.Category.Find(id);
            if (cat == null) { return NotFound(); }

            _rdb.Category.Remove(cat);
            _rdb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
