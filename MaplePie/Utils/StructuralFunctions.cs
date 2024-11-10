namespace MaplePie.Utils;


public interface IStructFunc<TF, in TA, out TR>
{
    public static abstract TR Invoke(ref TF self, TA arg);
}

public interface IStructFunc<TF, in TA1, in TA2, out TR>
{
    public static abstract TR Invoke(ref TF self, TA1 arg1, TA2 arg2);
}

public interface IStructFunc<TF, in TA1, in TA2, in TA3, out TR>
{
    public static abstract TR Invoke(ref TF self, TA1 arg1, TA2 arg2, TA3 arg3);
}

public interface IStructFunc<TF, in TA1, in TA2, in TA3, in TA4, out TR>
{
    public static abstract TR Invoke(ref TF self, TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4);
}

public interface IStructAction<TF, in TA>
{
    public static abstract void Invoke(ref TF self, TA arg);
}
