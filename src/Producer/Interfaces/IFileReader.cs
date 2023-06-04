namespace URLHealthChecker.Producer.Interfaces
{
    public interface IFileReader
    {
        List<string> ReadLines(string path);
        List<string> ReadLinesInFolder(string folderPath);
    }
}