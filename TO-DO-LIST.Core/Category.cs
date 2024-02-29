using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TO_DO_LIST.Core
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category name already required")]
        [StringLength(50, ErrorMessage = ("Not avoid 50 character"))]
        public string? CategoryName { get; set; }


        [ForeignKey("IdentityUser")]
        public int Id { get; set; }
        public IdentityUser User { get; set; }
    }

}

