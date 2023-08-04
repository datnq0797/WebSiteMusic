namespace be.Helpers
{
    public class Validate
    {
        private static Validate instance;
        public static Validate Instance
        {
            get { if (instance == null) instance = new Validate(); return Validate.instance; }
            private set { Validate.instance = value; }
        }
        public bool IsValidFileTypeMp3(IFormFile file)
        {
            var allowedExtensions = new[] { ".mp3" };
            var fileExtension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(fileExtension);
        }
        public bool IsValidFileTypeImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".png", ".jpg" };
            var fileExtension = Path.GetExtension(file.FileName);
            return allowedExtensions.Contains(fileExtension);
        }
    }
}
