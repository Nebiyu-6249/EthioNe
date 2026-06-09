using EthioNegarit.Core.Domain.Entities;
using System;


namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface ISettingsService
    {
       Settings GetSettings(bool useCache = true);
        Settings Add(Settings settings);
        Settings Get(Guid id);
    }
}
