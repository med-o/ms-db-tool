namespace Database.Core.IO
{
    public interface IParser
    {
        ParserOutput ParseString(string inputString);

        ParserOutput ParseFile(string filePath);
    }
}