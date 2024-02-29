using System.ComponentModel.DataAnnotations;

namespace TO_DO_LIST.ModelFile
{
    public class TaskUpload : Models.Task
    {
        [Display(Name = "Task image")]
        [DataType(DataType.Upload)]
        public IFormFile PostImageUrl { get; set; }
    }
}
