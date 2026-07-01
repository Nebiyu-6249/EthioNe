using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EthioNegarit.Core.Interfaces.Services
{
    public partial interface IPackageService
    {
        void SyncPackages(List<Assembly> assemblies);
        IList<Package> GetallPackages();
        IList<Package> GetPackages();
        //  IEnumerable<AdPackage> GetallPackages();
        IEnumerable<Package> GetAll();
        Package Add(Package package);
        Package GetPackage(Guid packageId);
        Package Get(string name);
        Package GetBasicPackage();
    }
}
