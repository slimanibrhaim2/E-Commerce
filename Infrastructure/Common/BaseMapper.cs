using System.Linq.Expressions;

namespace Infrastructure.Common;

public abstract class BaseMapper<TSource, TTarget> : IMapper<TSource, TTarget>
{
    public abstract TTarget Map(TSource source);
    public abstract TSource MapBack(TTarget target);

    protected void ValidateSource(TSource source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
    }

    protected TTarget MapWithExpression(TSource source, Expression<Func<TSource, TTarget>> mappingExpression)
    {
        ValidateSource(source);
        var compiled = mappingExpression.Compile();
        return compiled(source);
    }

    protected TTarget SafeMap(TSource source, Func<TSource, TTarget> mappingFunc)
    {
        try
        {
            ValidateSource(source);
            return mappingFunc(source);
        }
        catch (Exception ex)
        {
            throw new MappingException($"Error mapping {typeof(TSource)} to {typeof(TTarget)}", ex);
        }
    }

    protected TSource SafeMapBack(TTarget target, Func<TTarget, TSource> mappingFunc)
    {
        try
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            return mappingFunc(target);
        }
        catch (Exception ex)
        {
            throw new MappingException($"Error mapping {typeof(TTarget)} to {typeof(TSource)}", ex);
        }
    }
}

public class MappingException : Exception
{
    public MappingException(string message) : base(message) { }
    public MappingException(string message, Exception innerException) : base(message, innerException) { }
} 