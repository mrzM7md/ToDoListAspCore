using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using TO_DO_LIST.Data;

namespace TO_DO_LIST.Controllers
{
    [Authorize] // الشخص الي مسجل دخوله بس مسموحله يشتغل
    public class TasksController : Controller
    {
        private string? userId;
        private readonly Code.FilesHelper filesHelper;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IWebHostEnvironment webHost;
        public TasksController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHost
            )
        {
            _context = context;

            this.userManager = userManager;
            this.webHost = webHost;
            filesHelper = new Code.FilesHelper(this.webHost);
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string? keywordSearch = "")
        {

            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<Models.Task> tasks;

            if (IsThereKeywordSearch(keywordSearch))
            {
                tasks = GetAllTasksAfterFiltering(keywordSearch!);
            }
            else
            {
                tasks = GetAllTasks();
            }

            return View(tasks);
        }



        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {

            var categories = _context.Categories;
            ViewBag.categories = categories.Where(x => x.UserId == userId).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,UserId,TaskName,TaskDescription,PostImageUrl,IsFavorate,IsFinished,PostImageUrl,categories")] ModelFile.TaskUpload uploadTask, List<int> categories)
        {
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var task = new Models.Task();


            if (ModelState.IsValid)
            {
                task = uploadTask;
                task.UserId = userId;
                task.PostImageUrl = await filesHelper.UploadFile(uploadTask.PostImageUrl, Path.Combine("wwwroot", "images")); // خزن الصورة بالمشروع وارجع اسمها وخزنه بالحقل حق صورة البوست (التاسك)شت تعبت نكتب

                //_context.Tasks.Add(task);
                _context.Add(task);

                await _context.SaveChangesAsync();

                // ضف الفئات الي المستخدم اختارها لذي التاسك في جدول العلاقة
                foreach (var categoryId in categories)
                {
                    var taskCategories = new Models.TasksCategories();
                    taskCategories.TaskId = task.TaskId;
                    taskCategories.CategoryId = categoryId;
                    _context.Add(taskCategories);
                }
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }

            ViewBag.categories = _context.Categories.Where(x => x.UserId == userId).ToList();
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // ذا العكين كله عشان نجيب الفئات الي المستخدم مختارها من قبل عشان نعلم عليهن تلقائيا بالتصميم بدل ما نخليه يختر من أول وجديد
            var categories = _context.Categories;
            ViewBag.categories = categories.Where(x => x.UserId == userId).ToList();
            var selectedCategoriesIds = _context.TasksCategories.Where(x => x.TaskId == task.TaskId).Select(y => y.CategoryId).ToList();
            ViewBag.selectedCategoriesIds = selectedCategoriesIds;
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int TaskId, [Bind("TaskId,UserId,TaskName,TaskDescription,PostImageUrl,IsFavorate,IsFinished,PostImageUrl,categories")] ModelFile.TaskUpload uploadTask, List<int> categories)
        {
            if (TaskId != uploadTask.TaskId)
            {
                return NotFound();
            }

            var task = new Models.Task();

            ModelState.Remove("PostImageUrl"); // لا تشيك اذا في صورة لأننا هنا بحالة تعديل، إذا ما أدخل صورة مافي مشكلة لأننا بنعتمد الصورة الي أدخلها بالإنشاء
            if (ModelState.IsValid)
            {
                task = uploadTask;

                if (uploadTask.PostImageUrl != null) // إذا المستخدم بالعل أدخل صورة جديدة
                {
                    task.PostImageUrl = await filesHelper.UploadFile(uploadTask.PostImageUrl, Path.Combine("wwwroot", "images")); // خزنها داخل المشروع + رجع اسمها
                    // using `AsNoTracking` for avoid framework tracking error..
                    string oldImage = _context.Tasks.AsNoTracking().Where(x => x.TaskId == TaskId).First()!.PostImageUrl!; // أحصل على مسار الصورة الي سواها في حالة إنشاء مهمة جديدة عشان نحذفها ونحط مكانها الصورة بعد التعديل
                    filesHelper.DeleteImage(oldImage); // أحذف الصورة القديمة لأن معنا صورة جديدة بالتعديل
                }
                _context.Update(task);

                if (uploadTask.PostImageUrl == null) // إذا المستخدم ما سوا تعديل على الصورة فلا تحط نلل بالداتا بيز مكان مسار الصورة بل تجاهل هذا الحقل ولا تعدله، كذا بيعتمد الصورة في حالة إنشاء التاسك
                    _context.Entry(task).Property(t => t.PostImageUrl).IsModified = false;

                await _context.SaveChangesAsync(); // احفظ التغييرات


                // هنا بنحذف الفئات المختارة القديمة من جدول العلاقة بين التاسك والفئات الي المستخدم أدخلها لأن بيكون في فئات جديدة مختارة
                var catIds = _context.TasksCategories.Where(x => x.TaskId == task.TaskId).Select(x => x.Id).ToList();
                foreach (int catId in catIds)
                {
                    _context.TasksCategories.Remove(_context.TasksCategories.Where(x => x.Id == catId).First());
                }

                //  هنا أضفنا الفئات الي المستخدم عام عليها لجدول العلاقة
                foreach (var categoryId in categories)
                {
                    var taskCategories = new Models.TasksCategories();
                    taskCategories.TaskId = task.TaskId;
                    taskCategories.CategoryId = categoryId;
                    _context.TasksCategories.Add(taskCategories);
                }
                await _context.SaveChangesAsync(); // احفظ التغييرات



                return RedirectToAction(nameof(Index));
            }

            // إذا حصل أيرور رجعنا لنفس الصفحة وعلم على الفئات الي اختارهن لما أنشأ تاسك
            ViewBag.categories = _context.Categories.Where(x => x.UserId == userId).ToList();
            var selectedCategoriesIds = _context.TasksCategories.Where(x => x.TaskId == task.TaskId).Select(y => y.CategoryId).ToList();
            ViewBag.selectedCategoriesIds = selectedCategoriesIds;
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tasks'  is null.");
            }
            string oldImage = _context.Tasks.AsNoTracking().Where(x => x.TaskId == id).First()!.PostImageUrl!;
            filesHelper.DeleteImage(oldImage);

            var task = await _context.Tasks.FindAsync(id);

            var catIds = _context.TasksCategories.Where(x => x.TaskId == id).Select(x => x.Id).ToList();
            foreach (int catId in catIds)
            {
                _context.TasksCategories.Remove(_context.TasksCategories.Where(x => x.Id == catId).First());
            }

            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return (_context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }


        private bool IsThereKeywordSearch(string? keywordSearch)
        {
            if (keywordSearch != null && keywordSearch.Trim() != "")
                return true;
            return false;
        }

        private List<Models.Task> GetAllTasks()
        {
            return _context.Tasks.Where(x => x.UserId == userId)
                .Include(t => t.TaskCategories)!
                    .ThenInclude(tc => tc.Category)
                .ToList();
        }

        // IAsyncEnumerable = List with async...
        private List<Models.Task> GetAllTasksAfterFiltering(string keywordSearch)
        {
            return
               _context.Tasks.Where(x => x.UserId == userId)
               .Include(t => t.TaskCategories)!
               .ThenInclude(tc => tc.Category).Where(
                   x =>
                   x.TaskName!.Contains(keywordSearch) ||
                   x.TaskDescription.Contains(keywordSearch)
               ).ToList();
        }




    }
}
