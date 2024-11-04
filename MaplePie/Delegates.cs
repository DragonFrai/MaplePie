namespace MaplePie;


public delegate TOutput SpanFunc<TToken, out TOutput>(ReadOnlySpan<TToken> span);
public delegate TOutput SpanArgFunc<TToken, in TArg, out TOutput>(ReadOnlySpan<TToken> span, TArg arg);
