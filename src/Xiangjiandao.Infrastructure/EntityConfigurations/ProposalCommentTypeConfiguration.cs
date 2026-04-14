using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCorePal.Extensions.Domain;
using Xiangjiandao.Domain.AggregatesModel.ProposalAggregate;

namespace Xiangjiandao.Infrastructure.EntityConfigurations;

internal class ProposalCommentTypeConfiguration : IEntityTypeConfiguration<ProposalComment>
{
    public void Configure(EntityTypeBuilder<ProposalComment> builder)
    {
        builder.ToTable("t_proposal_comment");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(t => t.ProposalId)
            .HasColumnName("proposal_id")
            .HasComment("提案 Id");
        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .HasComment("评论用户 Id");
        builder.Property(t => t.UserName)
            .HasColumnName("user_name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .HasDefaultValue(string.Empty)
            .HasComment("评论用户名称");
        builder.Property(t => t.Content)
            .HasColumnName("content")
            .HasColumnType("varchar")
            .HasMaxLength(512)
            .HasDefaultValue(string.Empty)
            .HasComment("评论内容");
        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime")
            .HasDefaultValueSql("current_timestamp")
            .HasComment("创建时间");
        builder.Property(t => t.Deleted)
            .HasColumnName("deleted")
            .HasColumnType("tinyint")
            .HasDefaultValue(new Deleted(false))
            .HasComment("是否删除");
    }
}
