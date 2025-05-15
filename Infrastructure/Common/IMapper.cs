namespace Infrastructure.Common;

public interface IMapper<TSource, TTarget>
{
    TTarget Map(TSource source);
    TSource MapBack(TTarget target);
} 