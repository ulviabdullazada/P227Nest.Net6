namespace Nest.Utlis.Extensions
{
    public static class FileExtension
    {
        public static bool CheckFileExtension(this IFormFile file, string key)
        {
            return file.ContentType.Contains(key);
        }
        //file.CutFileName()
        //FileExtension.CutFileName(file)
        public static string CutFileName(this IFormFile file,int maxSize = 60)
        {
            if (file.FileName.Length > maxSize)
            {
                return file.FileName.Substring(file.FileName.Length - maxSize);
            }
            return file.FileName;
        }
        public static bool CheckFileSize(this IFormFile file, int mb)
        {
            return file.Length > mb * 1024 * 1024;
        }
        public static void SaveFile(this IFormFile file,string path)
        {
            path = Path.Combine(Nest.Utlis.Constants.Constants.RootPath,path);
            using (var writer = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(writer);
            }
        }
    }
}
