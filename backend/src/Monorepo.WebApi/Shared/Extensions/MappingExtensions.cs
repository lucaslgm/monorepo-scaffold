namespace Monorepo.WebApi.Shared.Extensions;

public static class MappingExtensions
{
    public static void CopyPropertiesTo<T>(this object? source, T dest)
    {
        if (source == null || dest == null)
        {
            return;
        }

        var sourceProps = source.GetType().GetProperties();
        var destProps = typeof(T).GetProperties();

        foreach (var sourceProp in sourceProps)
        {
            var destProp = destProps.FirstOrDefault(x =>
                x.Name == sourceProp.Name &&
                x.PropertyType == sourceProp.PropertyType &&
                x.CanWrite);

            destProp?.SetValue(dest, sourceProp.GetValue(source));
        }
    }
}
