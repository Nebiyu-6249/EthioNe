using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;

namespace EthioNegarit.Core.Interfaces.Services
{
  public  interface ILocationService
    {
        Location Add(Location location);
        IList<Location> GetLocations(Guid advertId);
        void Delete(Location location);
        Location Get(Guid id);
    }
}
