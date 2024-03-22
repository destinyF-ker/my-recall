using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecAll.Contrib.TextItem.Api.Data;

public class TextItemContext : DbContext
{
    public DbSet<Models.TextItem> TextItems { get; set; }

    public TextItemContext(DbContextOptions<TextItemContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TextItemConfiguration());
    }
}

public class TextItemConfiguration : IEntityTypeConfiguration<Models.TextItem>
{
    public void Configure(EntityTypeBuilder<Models.TextItem> builder)
    {
        // Table name
        builder.ToTable("Textitems");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseHiLo("textitem_hilo"); // 分布式标识生成器

        // Properties
        builder.Property(x => x.ItemId).IsRequired(false);
        builder.HasIndex(x => x.ItemId).IsUnique();

        builder.Property(x => x.Content).IsRequired();

        builder.Property(x => x.UserIdentityGuid).IsRequired();
        builder.HasIndex(x => x.UserIdentityGuid).IsUnique(false);

        builder.Property(x => x.IsDeleted).IsRequired();
    }
}

public class TextItemContextDesignFactory : IDesignTimeDbContextFactory<TextItemContext>
{
    public TextItemContext CreateDbContext(string[] args)
    =>
        new(
            new DbContextOptionsBuilder<TextItemContext>()
                .UseSqlServer(
                    "Server=.;Initial Catalog=RecAll.TextListDb;Integrated Security=true")
                .Options
        );
}
