﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace otce_project_final.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategorySelector { get; set; }
    }
}
