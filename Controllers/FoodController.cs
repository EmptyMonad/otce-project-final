using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using otce_project_final.Data;
using otce_project_final.Models;
using otce_project_final.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace otce_project_final.Controllers
{
    public class FoodController : Controller
    {
        private readonly ILogger<FoodController> _logger;
        private readonly AppDbContext _rdb; 
        public FoodController(ILogger<FoodController> logger, AppDbContext rdb)
        {
            _logger = logger;
            _rdb = rdb;
        }

        public IActionResult Index()
        {
            HomeVM hvm = new HomeVM()
            {
                Products = _rdb.Product.Include(c => c.Category),
                Categories = _rdb.Category
            };
            return View(hvm);
        }
    }
}