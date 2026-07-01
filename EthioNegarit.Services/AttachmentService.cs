namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Core.Domain.Entities;
    using Core.Interfaces.Services;
    using Data.Context;
    public partial class AttachmentService : IAttachmentService
    {
        private readonly EthioNegaritContext _context;
        public AttachmentService(EthioNegaritContext context)
        {
            _context = context;
        }
        public Attachment Add(Attachment attachment)
        {
            attachment.DateCreated = DateTime.UtcNow;
            return _context.Attachment.Add(attachment);
        }     
        public Attachment Get(Guid id)
        {
            return _context.Attachment
                .Include(x => x.PrivateMessage)
                .FirstOrDefault(x => x.Id == id);
        }
        public IList<Attachment> GetAll()
        {
            return _context.Attachment.ToList();
        }
        public IList<Attachment> GetAttachments(Guid messageId)
        {
            return _context.Attachment
               // .Include(x => x.PrivateMessage)
               // .Include(x => x.MembershipUser)
                .Where(x => x.PrivateMessage.Id == messageId).ToList();
        }
        public Attachment GetAttachment(Guid id)
        {
            return _context.Attachment
                .Include(x => x.PrivateMessage)
                .FirstOrDefault(x => x.Id == id);
        }
        public void Delete(Attachment attachment)
        {
            var entityToDelete = _context.Attachment.SingleOrDefault(e => e.Id == attachment.Id);
            if (entityToDelete != null)
            {
                _context.Attachment.Remove(entityToDelete);
                //_context.Entry(message).State = EntityState.Deleted;
              //  _context.SaveChanges();
            }
            }         
    }
}
