using System.Data;
using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Core.List.Infrastructure.EntityConfigurations;
using RecAll.Infrastructure.Ddd.Domain.SeedWork;
using RecAll.Infrastructure.Ddd.Infrastructure;

namespace RecAll.Core.List.Infrastructure;

/// <summary>
/// 首先 ListContext 是一个 DbContext，用于访问数据库，其次它就是一个 IUnitOfWork，用于处理事务(真正访问数据库)
/// 关系实际上是这样的：
/// IUnitOfWork 是一个接口，里面定义了两个方法需要实现，实现了该接口的类意味着这个类将处理数据库操作
/// DbContext 是一个类，用于访问数据库
/// 同时实现了 IUnitOfWork 和 DbContext，通过 DbContext 处理数据库操作的能力来实现 IUnitOfWork 的两个方法 
/// </summary> <summary>
/// 
/// </summary>
public class ListContext : DbContext, IUnitOfWork
{
    // HiLo 算法的 schema
    public const string DefaultSchema = "list";

    // 将 List 数据库表注册进来（Dbset）
    public DbSet<Domain.AggregateModels.ListAggregate.List> Lists { get; set; }

    // 将 ListType 数据库表注册进来（Dbset）
    public DbSet<ListType> ListTypes { get; set; }

    private readonly IMediator _mediator;

    // 数据库事务
    private IDbContextTransaction _currentTransaction;

    // 当前正在进行的数据库事务
    public IDbContextTransaction CurrentTransaction => _currentTransaction;

    // 当前有没有事务在进行
    public bool HasActiveTransaction => _currentTransaction != null;

    public ListContext(DbContextOptions<ListContext> options) :
        base(options)
    { }

    public ListContext(DbContextOptions<ListContext> options,
        IMediator mediator) : base(options)
    {
        // 注册中介者模式
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));

        Debug.WriteLine($"TaskContext::ctor -> {GetHashCode()}");
    }

    // 配置数据库表之间的关系
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ListTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ListConfiguration());
        // modelBuilder.ApplyConfiguration(new SetConfiguration());
    }

    // 触发领域事件，也就是说将领域事件广播出去
    public async Task<bool> SaveEntitiesAsync(
        CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);
        await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            return null;
        }

        return _currentTransaction =
            await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task
        CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction is null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        if (transaction != _currentTransaction)
        {
            throw new InvalidOperationException(
                $"Transaction {transaction.TransactionId} is not current");
        }

        try
        {
            await SaveChangesAsync();
            transaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}

// 生成 Migration 文件时需要的工厂类
public class
    ListContextDesignFactory : IDesignTimeDbContextFactory<ListContext>
{
    public ListContext CreateDbContext(string[] args) =>
        new(
            new DbContextOptionsBuilder<ListContext>()
                .UseSqlServer(
                    "Server=.;Initial Catalog=RecAll.ListDb;Integrated Security=true")
                .Options, new NoMediator());
}
