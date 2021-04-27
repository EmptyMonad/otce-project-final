using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace otce_project_final.Models
{
    public class Product
    {
        /* Id, Name, Price, Image, Description, Category */
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(75, ErrorMessage = "Product name should be between {1} and {2} characters long.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public string Image { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Product description should be no longer than {1} characters.")]
        public string Description { get; set; }

        /* mapping entity for foreign key */
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
