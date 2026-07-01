namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Core.Constants;
    using Core.Domain.Entities;
    using Core.Domain.General;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using Utilities;
    public partial class AdvertFileService : IAdvertFileService
    {
        private readonly EthioNegaritContext _context;
        public AdvertFileService(EthioNegaritContext context)
        {
            _context = context;
        }


        public AdvertisementFile Add(AdvertisementFile advertFile)
        {
            advertFile.DateCreated = DateTime.UtcNow;
            return _context.AdvertisementFile.Add(advertFile);
        }

        public void Delete(AdvertisementFile advertFile)
        {
            _context.AdvertisementFile.Remove(advertFile);
        }

        public IList<AdvertisementFile> GetAll()
        {
            return _context.AdvertisementFile.ToList();
        }

        public IList<AdvertisementFile> GetAllByAdvert(Guid advertId)
        {
            return _context.AdvertisementFile.Where(x => x.Advertisement.Id == advertId).ToList();
        }

        public IList<AdvertisementFile> GetAllByUser(Guid membershipUserId)
        {
            return _context.AdvertisementFile.Where(x => x.MembershipUser.Id == membershipUserId).ToList();
        }

        public AdvertisementFile Get(Guid id)
        {
            return _context.AdvertisementFile
                .Include(x => x.Advertisement)
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
