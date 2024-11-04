namespace MaplePie.Errors;


public class ParserNotAdvancedException: Exception
{
    public ParserNotAdvancedException(): base() {}
    public ParserNotAdvancedException(string? message): base(message) {}
}
