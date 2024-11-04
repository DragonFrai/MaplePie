using MaplePie.Parser;

namespace MaplePie.Errors;


public interface IParseError
{
    public string Format<T>(ReadOnlySpan<T> input, int position);
}
