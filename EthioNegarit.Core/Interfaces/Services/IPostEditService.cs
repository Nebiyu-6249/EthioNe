using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;


namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IPostEditService
    {
       
         void Delete(IList<PostEdit> entity);
        void Delete(PostEdit entity);
        PostEdit Add(PostEdit entity);
        PostEdit Get(Guid id);
        IList<PostEdit> GetByPost(Guid postId);
        IList<PostEdit> GetByMember(Guid memberId);
       
    }
}
