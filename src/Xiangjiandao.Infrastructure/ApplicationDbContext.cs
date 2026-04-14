using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.DistributedTransactions.CAP.Persistence;
using Xiangjiandao.Domain;
using Xiangjiandao.Domain.AggregatesModel.AdminUserAggregate;
using Xiangjiandao.Domain.AggregatesModel.AppAggregate;
using Xiangjiandao.Domain.AggregatesModel.BannerAggregate;
using Xiangjiandao.Domain.AggregatesModel.GlobalConfigAggregate;
using Xiangjiandao.Domain.AggregatesModel.InformationAggregate;
using Xiangjiandao.Domain.AggregatesModel.MedalAggregate;
using Xiangjiandao.Domain.AggregatesModel.NodeAggregate;
using Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;
using Xiangjiandao.Domain.AggregatesModel.ScoreDistributeRecordAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;

namespace Xiangjiandao.Infrastructure
{
    public partial class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator,
        IServiceProvider provider)
        : AppDbContextBase(options, mediator), IDataProtectionKeyContext, IMySqlCapDataStorage
    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IUpdateAt entity
                    && entry.State is EntityState.Modified)
                {
                    entity.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            ConfigureStronglyTypedIdValueConverter(configurationBuilder);
            base.ConfigureConventions(configurationBuilder);
        }

        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

        /// <summary>
        /// 管理员
        /// </summary>
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

        /// <summary>
        /// 横幅
        /// </summary>
        public DbSet<Banner> Banners => Set<Banner>();

        /// <summary>
        /// 全局配置
        /// </summary>
        public DbSet<GlobalConfig> GlobalConfigs => Set<GlobalConfig>();


        /// <summary>
        /// 节点
        /// </summary>
        public DbSet<Node> Nodes => Set<Node>();

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// 勋章
        /// </summary>
        public DbSet<Medal> Medals => Set<Medal>();

        /// <summary>
        /// 稻米记录
        /// </summary>
        public DbSet<ScoreRecord> ScoreRecords => Set<ScoreRecord>();

        /// <summary>
        /// 稻米分发记录
        /// </summary>
        public DbSet<ScoreDistributeRecord> ScoreDistributeRecords => Set<ScoreDistributeRecord>();

        /// <summary>
        /// 投票记录
        /// </summary>
        public DbSet<Proposal> Proposals => Set<Proposal>();

        /// <summary>
        /// 投票记录
        /// </summary>
        public DbSet<VoteRecord> VoteRecords => Set<VoteRecord>();

        /// <summary>
        /// 提案评论
        /// </summary>
        public DbSet<ProposalComment> ProposalComments => Set<ProposalComment>();

        /// <summary>
        /// 用户勋章
        /// </summary>
        public DbSet<UserMedal> UserMedals => Set<UserMedal>();

        /// <summary>
        /// 用户勋章
        /// </summary>
        public DbSet<Information> Informations => Set<Information>();

        /// <summary>
        /// 应用
        /// </summary>
        public DbSet<App> Apps => Set<App>();
    }
}