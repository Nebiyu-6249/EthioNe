using EthioNegarit.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace EthioNegarit.Services.Data.Mapping
{
 public class ChatRoomMapping : EntityTypeConfiguration<ChatRoom>
    {
        public ChatRoomMapping()
        {          
             HasKey(x => x.Id);
             Property(x => x.Id).IsRequired();
             Property(x => x.Name).IsRequired().HasMaxLength(450);
            Property(x => x.PageTitle).IsOptional().HasMaxLength(80);
            Property(x => x.Description).IsOptional();
             Property(x => x.DateCreated).IsRequired();
            Property(x => x.Slug).IsRequired().HasMaxLength(450)
                                 .HasColumnAnnotation("Index",
                                 new IndexAnnotation(new IndexAttribute("IX_ChatRoom_Name", 1) { IsUnique = true }));
        
             Property(x => x.IsLocked).IsRequired();
            Property(x => x.LastActive).IsOptional();
            Property(x => x.Freeze).IsOptional();
            Property(x => x.MetaDescription).IsOptional().HasMaxLength(200);
             Property(x => x.Colour).IsOptional().HasMaxLength(50);
             Property(x => x.Image).IsOptional().HasMaxLength(200);
            
            // Ignores
            Ignore(x => x.NiceUrl);        
         }
           
        }
    }

