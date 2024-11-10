namespace MaplePie.Utils;


public interface IStaticFunc<out TR>
{
    public static abstract object Invoke();
}

public interface IStaticFunc<in TA, out TR>
{
    public static abstract TR Invoke(TA arg);
}

public interface IStaticFunc<in TA1, in TA2, out TR>
{
    public static abstract TR Invoke(TA1 arg1, TA2 arg2);
}

public interface IStaticFunc<in TA1, in TA2, in TA3, out TR>
{
    public static abstract TR Invoke(TA1 arg1, TA2 arg2, TA3 arg3);
}

public interface IStaticFunc<in TA1, in TA2, in TA3, in TA4, out TR>
{
    public static abstract TR Invoke(TA1 arg1, TA2 arg2, TA3 arg3, TA4 arg4);
}

public interface IStaticAction<in TA>
{
    public static abstract void Invoke(TA arg);
}
