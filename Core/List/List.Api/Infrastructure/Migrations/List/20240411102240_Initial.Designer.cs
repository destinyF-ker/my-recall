﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecAll.Core.List.Infrastructure;

#nullable disable

namespace RecAll.Core.List.Infrastructure.Migrations
{
    [DbContext(typeof(ListContext))]
    [Migration("20240411102240_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence("listseq", "list")
                .IncrementsBy(10);

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ListAggregate.List", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("Id"), "listseq", "list");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<string>("UserIdentityGuid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserIdentityGuid");

                    b.Property<string>("_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int>("_typeId")
                        .HasColumnType("int")
                        .HasColumnName("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("_typeId");

                    b.ToTable("lists", (string)null);
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ListType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("listtypes", (string)null);
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ListAggregate.List", b =>
                {
                    b.HasOne("RecAll.Core.List.Domain.AggregateModels.ListType", "Type")
                        .WithMany()
                        .HasForeignKey("_typeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Type");
                });
#pragma warning restore 612, 618
        }
    }
}
