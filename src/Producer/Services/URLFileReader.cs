using URLHealthChecker.Producer.Interfaces;

namespace URLHealthChecker.Producer.Services
{
    public class URLFileReader : IFileReader
    {
        public List<string> ReadLines(string path)
        {
            List<string> urls = new();
            string? line;
            using StreamReader sr = new(path);
            while ((line = sr.ReadLine()) != null)
            {
                if (line != "")
                {
                    urls.Add(line);
                }
            }

            return urls;
        }

        public List<string> ReadLinesInFolder(string folderPath)
        {
            List<string> urls = new();
            foreach (string urlFile in Directory.GetFiles(folderPath, "*.txt"))
            {
                urls.AddRange(ReadLines(urlFile));
            }

            return urls;
        }
    }
}