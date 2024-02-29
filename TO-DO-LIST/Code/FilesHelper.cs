namespace TO_DO_LIST.Code
{
    public class FilesHelper
    {
        private readonly IWebHostEnvironment webHost;

        public FilesHelper(IWebHostEnvironment webHost)
        {
            this.webHost = webHost;
        }


        //Upload Files
        public async Task<string> UploadFile(IFormFile? file, string? folder)
        {
            if (file != null)
            {
                var fileDir = Path.Combine(webHost.ContentRootPath, folder); // ..../ images
                var fileName = Guid.NewGuid()/* random name */ + "-" + file.FileName; // 3663627664rgvsvcGGs-imag.JPG
                var filePath = Path.Combine(fileDir, fileName); // ..../images/3663627664rgvsvcGGs-imag.JPG

                //using (var sourseStream = new FileStream(folder, FileMode.Open))
                //{
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    return fileName;
                }
                //}


            }
            else
            {
                return string.Empty;
            }
        }

        public void DeleteImage(string imagePath)
        {
            string sourceImagePath= Path.Combine(webHost.ContentRootPath, "wwwroot", "images", imagePath);
            if (File.Exists(sourceImagePath))
            {
                File.Delete(sourceImagePath);
            }
        }
    }
}
