namespace be.Helpers
{
    public class RandomString
    {
        private static RandomString instance;
        public static RandomString Instance
        {
            get { if (instance == null) instance = new RandomString(); return RandomString.instance; }
            private set { RandomString.instance = value; }
        }
        static Random rd = new Random();
        public string CreateString()
        {
            string result = "";
            string[] items = new string[]
            {
                "0123456789",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                "abcdefghijkmnopqrstuvwxyz",
                "!@$?_-%^&*"
            };
            foreach (string item in items)
            {
                char[] listChar = new char[3];
                for (int i = 0; i < 3; i++)
                {
                    listChar[i] = item[rd.Next(0, item.Length)];
                }
                result += new string(listChar);
            }
            return result;
        }
    }
}
