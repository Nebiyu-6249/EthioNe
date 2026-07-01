using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace EthioNegarit.Core.Interfaces.Services
{
   public partial interface IAttachmentService
    {
        Attachment Add(Attachment attachment);
       Attachment Get(Guid id);
        IList<Attachment> GetAll();
        IList<Attachment> GetAttachments(Guid messageId);
        void Delete(Attachment attachment);             
        Attachment GetAttachment(Guid id);
    }
}
