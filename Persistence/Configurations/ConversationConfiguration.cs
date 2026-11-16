using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LetsTalk.Domain.Entities;

namespace LetsTalk.Persistence.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.Property(c => c.DirectParticipantKey)
                   .HasMaxLength(80);

            builder.HasIndex(c => c.DirectParticipantKey)
                   .IsUnique();

            builder.Property(c => c.LastMessagePreview)
                   .HasMaxLength(100);
        }
    }
}
