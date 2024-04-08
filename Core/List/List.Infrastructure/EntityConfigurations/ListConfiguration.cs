using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecAll.Core.List.Domain.AggregateModels;

namespace RecAll.Core.List.Infrastructure.EntityConfigurations;

/// <summary>
/// 配置了 List 实例怎么存储到数据库之中 
/// </summary> <summary>
/// 
/// </summary>
public class ListConfiguration : IEntityTypeConfiguration<
    Domain.AggregateModels.ListAggregate.List>
{
    public void Configure(
        EntityTypeBuilder<Domain.AggregateModels.ListAggregate.List> builder)
    {
        RelationalEntityTypeBuilderExtensions.ToTable(
            (EntityTypeBuilder)builder, "lists");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .UseHiLo("listseq", ListContext.DefaultSchema);
        builder.Ignore(p => p.DomainEvents);

        // EF Core 5.0 之后，可以使用 Field 的方式来访问私有字段，这样就可以在设计时尽量提高封装程度 
        builder.Property<string>("_name")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Name").IsRequired();

        builder.Property<int>("_typeId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("TypeId").IsRequired();

        // 主外键关联 
        builder.HasOne(p => p.Type) // 一个错题本对应一个类型
            .WithMany() // 一个 ListType 对应多个 List
            .HasForeignKey("_typeId")
            .OnDelete(DeleteBehavior.NoAction);

        // 下面这段配置和上面这种只有私有成员的配置是不一样的
        // 该种成员除了一个私有字段以外，还有一个公有的属性，这样就可以在外部访问到这个属性，所以在这里做了一些映射
        builder.Property(p => p.UserIdentityGuid)
            .HasField("_userIdentityGuid")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("UserIdentityGuid").IsRequired();

        builder.Property(p => p.IsDeleted)
            .HasField("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("IsDeleted").IsRequired();
    }
}