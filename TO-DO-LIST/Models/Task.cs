using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TO_DO_LIST.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        [Display(Name = "Task name")]
        [Required(ErrorMessage = "Task name is required !")]
        [MaxLength(20, ErrorMessage = "Not avoid 20 characters")]

        public string? TaskName { get; set; }

        [Display(Name = "Task description")]
        [Required (ErrorMessage = "The Task description field is required.")]
        public string TaskDescription { get; set; } = string.Empty;


        [Display(Name = "Task image")]
        public string? PostImageUrl { get; set; }

        [Display(Name = "Favorite")]
        public bool IsFavorate { get; set; } = false;

        [Display(Name = "Finished")]
        public bool IsFinished { get; set; } = false;

        public DateTime TimeAdded { get; set; } = DateTime.Now;


        [ForeignKey("IdentityUser")]
        public string? UserId { get; set; }

        public IdentityUser? User { get; set; }

        public List<TasksCategories>? TaskCategories { get; set; }

    }
}
