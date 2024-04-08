using RecAll.Core.List.Domain.Exceptions;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;

namespace RecAll.Core.List.Domain.AggregateModels;

// 在这里对 AggregrateModels 进行一些解释：
// 1. AggregateModels 是一个文件夹，用于存放领域聚合模型。
// 2. 领域聚合模型是一种设计模式，用于将一组相关的实体和值对象组合在一起，以便在领域中进行操作。
// 3. 在这里，ListType 是一个枚举类，用于定义列表的类型。

/// <summary>
/// 我们要实现的错题本之中有很多类型，例如文本类型、图片类型、音频类型等等。
/// 这里我们定义了一个 ListType 类，用于定义这些类型。
/// 这就是一个没有业务的、没有身份的枚举类。
/// </summary> <summary>
/// List -> Set -> Item
/// 在这个应用场景下 ListType 对应的就是哪一门课：高等数学、线性代数、概率论等等
/// </summary>
public class ListType : Enumeration
{
    // 这里定义了一个 TextId，用于表示文本类型的 Id。
    // 唯一的类型 ID，用于标识类型。
    public const int TextId = 1;

    // 写死了一个 Text 类型，用于表示文本类型。
    public static ListType Text = new(TextId, nameof(Text).ToLowerInvariant(), nameof(Text));
    // id: 1, name: "text", displayName: "Text"

    // Enumerations 要求必须实现一个构造函数，用于初始化枚举值。
    // 包括：id、name、displayName。
    public ListType(int id, string name, string displayName) : base(id, name, displayName)
    { }

    // 下面的都是帮助函数 
    private static ListType[] _list = { Text };

    public static IEnumerable<ListType> List() => _list;

    public static ListType FromName(string name) =>
        List().SingleOrDefault(p => string.Equals(p.Name, name,
            StringComparison.CurrentCultureIgnoreCase)) ??
        throw new ListDomainException(
            $"Possible values for {nameof(ListType)}: {string.Join(",", List().Select(p => p.Name))}");
}
