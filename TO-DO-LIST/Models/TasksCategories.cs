using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TO_DO_LIST.Models
{
    public class TasksCategories
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tasks")]
        public int TaskId { get; set; }

        [ForeignKey("Categories")]
        public int? CategoryId { get; set; }
        public Task? Task { get; set; }
        public Category? Category { get; set; }
    }
}
