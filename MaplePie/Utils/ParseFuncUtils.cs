namespace MaplePie.Utils;

public static class ParseFuncUtils
{
 
    
    public static bool MoveN<T>(int count, ReadOnlySpan<T> input, ref int position)
    {
        if (input.Length - position < count) return false;
        position += count;
        return true;
    }

    public static bool MoveOne<T>(ReadOnlySpan<T> input, ref int position)
    {
        if (position != input.Length) return false;
        position += 1;
        return true;
    }
    
    public static bool MoveOneEq<T>(T expectedToken, ReadOnlySpan<T> input, ref int position)
    {
        if (position == input.Length) return false;

        var token = input[position];
        var isEqual = EqualityComparer<T>.Default.Equals(token, expectedToken);

        if (!isEqual) return false;

        position += 1;
        return true;
    }
    
    public static bool MoveOneIf<T>(Func<T, bool> condition, ReadOnlySpan<T> input, ref int position)
    {
        if (position == input.Length) return false;

        var token = input[position];
        var isSuccessful = condition(token);

        if (!isSuccessful) return false;

        position += 1;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="input"></param>
    /// <param name="position"></param>
    /// <typeparam name="T"></typeparam>
    public static void MoveWhile<T>(Func<T, bool> condition, ReadOnlySpan<T> input, ref int position)
    {
        for (var i = position; i < input.Length; i++)
        {
            var token = input[i];
            if (condition(token)) continue;
            position = i;
            return;
        }
        position = input.Length;
    }
    
    public static bool MoveWhile1<T>(Func<T, bool> condition, ReadOnlySpan<T> input, ref int position)
    {
        var beginPosition = position;
        MoveWhile(condition, input, ref position);
        var isProgressed = position != beginPosition;
        return isProgressed;
    }
    

    
    public static bool MoveManyEq<T>(ReadOnlySpan<T> expectedSeq, ReadOnlySpan<T> input, ref int position)
    {
        var seqLength = expectedSeq.Length;
        var remLength = input.Length - position;
        
        if (remLength < seqLength) return false;

        var segment = input.Slice(position, seqLength);

        var isEqual = segment.SequenceEqual(expectedSeq);
        if (!isEqual) return false;

        position += seqLength;

        return true;
    }


    
    
}