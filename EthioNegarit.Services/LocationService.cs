namespace EthioNegarit.Services
{
    using Core.Domain.Entities;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Data.Context;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public partial class LocationService : ILocationService
    {
        private readonly EthioNegaritContext _context;
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;
        private readonly ICacheService _cacheService;
        private readonly IConfigService _configService;
        public LocationService(ISettingsService settingsService, IConfigService configService, IBadgeService badgeService, ILoggingService loggingService, IEthioNegaritContext context, ICacheService cacheService)
        {
            _loggingService = loggingService;
            _cacheService = cacheService;
            _settingsService = settingsService;
            _configService = configService;
            _context = context as EthioNegaritContext;
        }

        public Location Add(Location location)
        {
            return _context.Location.Add(location);
        }
        public Location Get(Guid id)
        {
            return _context.Location
           // .Include(x => x.Advertisement)
            .FirstOrDefault(x => x.Id == id);
        }
           
        public IList<Location> GetLocations(Guid advertId)
        {
            return _context.Location
                .Where(x => x.Advertisement.Id == advertId)
                .ToList();
        }
        public void Delete(Location location)
        {
            _context.Location.Remove(location);
        }
    }
}
