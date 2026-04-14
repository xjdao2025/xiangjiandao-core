using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Repository.EntityFrameworkCore;
using Xiangjiandao.Domain.AggregatesModel.ScoreRecordAggregate;
using Xiangjiandao.Domain.AggregatesModel.UserAggregate;
using Xiangjiandao.Domain.Enums;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class ScoreRecordTypeConfiguration : IEntityTypeConfiguration<ScoreRecord>
{
    public void Configure(EntityTypeBuilder<ScoreRecord> builder)
    {
        builder.ToTable("t_point_record");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .UseGuidVersion7ValueGenerator();
        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .HasComment("所属用户");
        builder.Property(t => t.ParticipatorId)
            .HasColumnName("participator_id")
            .HasComment("积分记录的另一个参与用户 Id");
        builder.Property(t => t.ParticipatorDomainName)
            .HasColumnName("participator_domain_name")
            .HasColumnType("varchar")
            .HasMaxLength(256)
            .HasDefaultValue(string.Empty)
            .HasComment("参与方域名");
        builder.Property(t => t.ParticipatorNickName)
            .HasColumnName("participator_nick_name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("参与方昵称");
        builder.Property(t => t.Type)
            .HasColumnName("type")
            .HasColumnType("int")
            .HasDefaultValue(ScoreSourceType.Unknown)
            .HasComment("积分来源类型");
        builder.Property(t => t.Reason)
            .HasColumnName("reason")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("获得原因");
        builder.Property(t => t.Remark)
            .HasColumnName("remark")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("附言");
        builder.Property(t => t.Score)
            .HasColumnName("score")
            .HasColumnType("bigint")
            .HasComment("积分数量");
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime")
            .HasDefaultValueSql("current_timestamp")
            .HasComment("创建时间");
        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("创建者");
        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("datetime")
            .HasDefaultValueSql("current_timestamp")
            .HasComment("更新时间");
        builder.Property(t => t.UpdatedBy)
            .HasColumnName("updated_by")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("更新者");
        builder.Property(t => t.Deleted)
            .HasColumnName("deleted")
            .HasColumnType("tinyint")
            .HasDefaultValue(new Deleted(false))
            .HasComment("是否删除");
    }
}
