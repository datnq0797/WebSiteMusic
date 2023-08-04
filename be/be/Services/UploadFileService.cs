using be.Helpers;
using be.Models;
namespace be.Services
{
    public class UploadFileService
    {
        private static UploadFileService instance;
        public static UploadFileService Instance
        {
            get { if (instance == null) instance = new UploadFileService(); return UploadFileService.instance; }
            private set { UploadFileService.instance = value; }
        }
        public bool UploadFileMp3(FileModel file)
        {
            if (Validate.Instance.IsValidFileTypeMp3(file.FormFile))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Musics", file.FileName);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    file.FormFile.CopyTo(stream);
                    return true;
                }
            }
            return false;
        }
        public bool UploadFileImage(FileModel file)
        {
            if (Validate.Instance.IsValidFileTypeImage(file.FormFile))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Images", file.FileName);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    file.FormFile.CopyTo(stream);
                    return true;
                }
            }
            return false;
        }
        public bool DeleteFileImage(string name)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Images", name);
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
