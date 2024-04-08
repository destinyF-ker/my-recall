using System.Reflection;

namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;

/// <summary>
/// 和高级语言类型系统之中自带的枚举类型概念不同，在这里指代的是不能成为 Entity 的对象。
/// 没有生存周期，没有业务意义，只是一些常量的集合，可以硬编码在内存之中，也可以存在数据库之中。
/// </summary> <summary>
/// 
/// </summary>
public abstract class Enumeration : IComparable
{
    // 数字上的唯一标识
    public int Id { get; private set; }

    // 用于数据库存储的字段（文字上的唯一标识）
    public string Name { get; private set; }

    // 用于展示的字段
    public string DisplayName { get; set; }

    protected Enumeration(int id, string name, string displayName) =>
        (Id, Name, DisplayName) = (id, name, displayName);

    public override string ToString() => Name;

    public static IEnumerable<TEnumeration> GetAll<TEnumeration>()
        where TEnumeration : Enumeration =>
        typeof(TEnumeration)
            .GetFields(BindingFlags.Public | BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Where(p => p.FieldType == typeof(TEnumeration))
            .Select(p => p.GetValue(null)).Cast<TEnumeration>();

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration enumeration)
        {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(enumeration.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration left, Enumeration right) =>
        Math.Abs(left.Id - right.Id);

    public static TEnumeration FromValue<TEnumeration>(int value)
        where TEnumeration : Enumeration =>
        Parse<TEnumeration, int>(value, "value", p => p.Id == value);

    public static TEnumeration FromDisplayName<TEnumeration>(string displayName)
        where TEnumeration : Enumeration =>
        Parse<TEnumeration, string>(displayName, "display name",
            p => p.Name == displayName);

    private static TEnumeration Parse<TEnumeration, KValue>(KValue value,
        string description, Func<TEnumeration, bool> predicate)
        where TEnumeration : Enumeration =>
        GetAll<TEnumeration>().FirstOrDefault(predicate) ??
        throw new InvalidOperationException(
            $"'{value}' is not a valid {description} in {typeof(TEnumeration)}");

    public static bool IsValidValue<TEnumeration>(int value)
        where TEnumeration : Enumeration =>
        GetAll<TEnumeration>().FirstOrDefault(p => p.Id == value) is not null;

    public int CompareTo(object obj) => Id.CompareTo(((Enumeration)obj).Id);
}
