using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TO_DO_LIST.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category name already required")]
        [StringLength(50, ErrorMessage = ("Not avoid 50 character"))]
        public string? CategoryName { get; set; }


        [ForeignKey("IdentityUser")]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }

}

