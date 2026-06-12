using System.ComponentModel;
using System.Reflection;
using Dapper;

namespace Monorepo.WebApi.Shared.Persistence.Dapper;

public class DapperMapper
{
    private static void Mapper<T>()
    {
        var type = typeof(T);

        var map = new CustomPropertyTypeMap(type, (t, columnName) =>
            GetPropertyByDescription(t, columnName) ?? GetPropertyByName(t, columnName)!);

        SqlMapper.SetTypeMap(typeof(T), map);
    }

    private static PropertyInfo? GetPropertyByDescription(Type type, string columnName)
    {
        return type.GetProperties().FirstOrDefault(prop =>
            GetDescriptionFromAttribute(prop)?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true);
    }

    private static PropertyInfo? GetPropertyByName(Type type, string columnName)
    {
        return type.GetProperties().FirstOrDefault(prop =>
            prop.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
    }

    private static string? GetDescriptionFromAttribute(MemberInfo member)
    {
        var attrib = (DescriptionAttribute?)Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute), false);
        return attrib?.Description;
    }

    public static void Iniciar()
    {
        SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);

        Mapper<ExampleClass>();
    }
}

public abstract class ExampleClass
{
}
