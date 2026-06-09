namespace EthioNegarit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using System.Text;
    using System.Threading.Tasks;

    using Core.Interfaces.Packages;
    using Data.Context;
    using Core.Interfaces.Services;
    using Core.Domain.Entities;
    using Core.Domain.Attributes;
    using Core.Constants;
    using Utilities;
    using Core.Interfaces;
    using EthioNegarit.Core.Domain.Enums;
    public partial class PackageService : IPackageService
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILoggingService _loggingService;
        private readonly IReflectionService _reflectionService;
        private readonly EthioNegaritContext _context;
        private readonly ICacheService _cacheService;
        public PackageService(ILoggingService loggingService, ILocalizationService localizationService, IReflectionService reflectionService, IEthioNegaritContext context, ICacheService cacheService)
        {
            _loggingService = loggingService;
            _localizationService = localizationService;
            _reflectionService = reflectionService;
            _cacheService = cacheService;
            _context = context as EthioNegaritContext;
        }
        #region Private static methods
        private static Dictionary<PackageType, List<PackageMapping>> _packages;

        private class PackageMapping
        {
            /// <summary>
            /// The domain object representation of the Package
            /// </summary>
            public Package DbPackage { get; set; }

            /// <summary>
            /// The class type that implements the Package rule
            /// </summary>
            public Type PackageClass { get; set; }

            /// <summary>
            /// An instance of the Package class, lazy loaded
            /// </summary>
            public IPackage PackageClassInstance { get; set; }
        }
        private static Package CreateDbPackageFromClass(PackageType packType, Type classType)
        {
            var idAtt = GetAttribute<IdAttribute>(classType);
            var nameAtt = GetAttribute<NameAttribute>(classType);
            var descAtt = GetAttribute<DescriptionAttribute>(classType);
            var displayNameAtt = GetAttribute<DisplayNameAttribute>(classType);
            var priceAtt = GetAttribute<PriceAttribute>(classType);
            var bonusAtt = GetAttribute<BonusAttribute>(classType);
            var discountAtt = GetAttribute<DiscountAttribute>(classType);
            var filessAtt = GetAttribute<FilesAttribute>(classType);
            var videoAtt = GetAttribute<VideoAttribute>(classType);
            var addressAtt = GetAttribute<AddressAttribute>(classType);
            var colorAtt = GetAttribute<ColorAttribute>(classType);
            var sortAtt = GetAttribute<SortAttribute>(classType);
            var package = new Package
            {
                Id = idAtt.Id,
                Name = nameAtt.Name,
                Description = descAtt.Description,
                Address = addressAtt.Address,
                DisplayName = displayNameAtt.DisplayName,
                Bonus = bonusAtt.Bonus,
                PriceRate = priceAtt.PriceRate,
                Discount = discountAtt.Discount,
                Type = packType.ToString().TrimEnd(),
                Files = filessAtt.Files,
                EnableVideo = videoAtt.EnableVideo,
                SortOrder = sortAtt.SortOrder,
                Color = colorAtt.Color
            };
            return package;
        }
        private static T GetAttribute<T>(Type type) where T : class
        {
            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is T)
                {
                    return attribute as T;
                }
            }
            throw new PackageAttributeNotFoundException();
        }

        private static IPackage GetInstance<T>(PackageMapping packageMapping)
        {
            // If we've previously instantiated this class then return the instance
            if (packageMapping.PackageClassInstance != null)
            {
                return packageMapping.PackageClassInstance;
            }

            var ctor = packageMapping.PackageClass.GetConstructors().First();
            var createdActivator = ReflectionUtilities.GetActivator<T>(ctor);

            // Create an instance:
            packageMapping.PackageClassInstance = createdActivator() as IPackage;

            return packageMapping.PackageClassInstance;
        }
        private PackageType? FromString(string packageTypeStr)
        {
            try
            {
                return (PackageType)Enum.Parse(typeof(PackageType), packageTypeStr);
            }
            catch (ArgumentException)
            {
                _loggingService.Error(string.Format(_localizationService.GetResourceString("Package.UnknownPackage"), packageTypeStr));
            }
            return null;
        }
        #endregion
        private void GetPackagesByReflection(IEnumerable<Assembly> assemblies)
        {
            _packages = new Dictionary<PackageType, List<PackageMapping>>();

            var interfaceFilter = new TypeFilter(_reflectionService.InterfaceFilter);
            // Get all the dlls
            foreach (var nextAssembly in assemblies)
            {
                try
                {
                    foreach (var type in nextAssembly.GetTypes())
                    {
                        if (type.IsInterface)
                        {
                            continue;
                        }

                        // See if this type is one of the Package interfaces
                        foreach (PackageType packType in Enum.GetValues(typeof(PackageType)))
                        {
                            // Look for the target package class type
                            if (!Package.PackageClassNames.ContainsKey(packType))
                            {
                                throw new ApplicationException(
                                    string.Format(_localizationService.GetResourceString("Package.PackageEnumNoClass"), packType));
                            }

                            var interfaceType = Package.PackageClassNames[packType];

                            var myInterfaces = type.FindInterfaces(interfaceFilter, interfaceType);
                            if (myInterfaces.Length <= 0)
                            {
                                // Not a match
                                continue;
                            }

                            // This class implements the interface

                            // Create a domain model version
                            var dbPackage = CreateDbPackageFromClass(packType, type);

                            if (!_packages.ContainsKey(packType))
                            {
                                _packages.Add(packType, new List<PackageMapping>());
                            }
                            _packages[packType].Add(new PackageMapping { PackageClass = type, DbPackage = dbPackage });
                        }
                    }
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    var msg =$"Unable to load assembly. Probably not an event assembly, loader exception was: '{rtle.LoaderExceptions[0].GetType()}':'{rtle.LoaderExceptions[0].Message}'.";
                    _loggingService.Error(msg);
                }
                catch (Exception ex)
                {
                    _loggingService.Error(ex);
                }
            }
        }
        public void SyncPackages(List<Assembly> assemblies)
        {
            try
            {
                GetPackagesByReflection(assemblies);
                // Turn the package classes into a set of domain objects
                var packagesFromClasses = new Dictionary<Guid, Package>();
                foreach (var packType in _packages.Keys)
                {
                    foreach (var packageClassMapping in _packages[packType])
                    {
                        if (!packagesFromClasses.ContainsKey(packageClassMapping.DbPackage.Id))
                        {
                            packagesFromClasses.Add(packageClassMapping.DbPackage.Id, packageClassMapping.DbPackage);
                        }
                    }
                }

                var packagesToDelete = new List<Package>();
                var packagesToInsert = new List<Package>();
                var dbPackages = GetAll().ToList();
                // Find badges to delete - ie in database not in classes     
                packagesToDelete.AddRange(dbPackages);
                foreach (var dbPackage in dbPackages)
                {
                    if (packagesFromClasses.Values.Any(classPackage => classPackage.Id == dbPackage.Id))
                    {
                        packagesToDelete.Remove(dbPackage);
                    }
                }
                // Find dbPackages to insert or update
                packagesToInsert.AddRange(packagesFromClasses.Values);
                foreach (var classPackage in packagesFromClasses.Values)
                {
                    foreach (var dbPackage in dbPackages)
                    {
                        if (dbPackage.Id == classPackage.Id)
                        {
                            // This class is found in the database so it's not new, but might be an update
                            if (dbPackage.Name != classPackage.Name)
                            {
                                dbPackage.Name = classPackage.Name;
                            }
                            if (dbPackage.Description != classPackage.Description)
                            {
                                dbPackage.Description = classPackage.Description;
                            }
                            if (dbPackage.DisplayName != classPackage.DisplayName)
                            {
                                dbPackage.DisplayName = classPackage.DisplayName;
                            }                                               
                            if (dbPackage.Bonus != classPackage.Bonus)
                            {
                                dbPackage.Bonus = classPackage.Bonus;
                            }
                            if (dbPackage.PriceRate != classPackage.PriceRate)
                            {
                                dbPackage.PriceRate = classPackage.PriceRate;
                            }
                            if (dbPackage.Discount != classPackage.Discount)
                            {
                                dbPackage.Discount = classPackage.Discount;
                            }
                            if (dbPackage.Files != classPackage.Files)
                            {
                                dbPackage.Files = classPackage.Files;
                            }
                            if (dbPackage.Address != classPackage.Address)
                            {
                                dbPackage.Address = classPackage.Address;
                            }
                           if (dbPackage.EnableVideo != classPackage.EnableVideo)
                            {
                                dbPackage.EnableVideo = classPackage.EnableVideo;
                            }
                             if (dbPackage.SortOrder != classPackage.SortOrder)
                              {
                                  dbPackage.SortOrder = classPackage.SortOrder;
                              }
                              if (dbPackage.Color != classPackage.Color)
                              {
                                  dbPackage.Color = classPackage.Color;
                              }                           
                            // Remove it from insert collection, it's not new
                           packagesToInsert.Remove(classPackage);
                        }
                    }
                }

                foreach (var package in packagesToDelete)
                {
                    Delete(package);
                }
                foreach (var package in packagesToInsert)
                {
                    Add(package);
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
            }
        }
        public IList<Package> GetPackages()
        {
            var cacheKey = string.Concat(CacheKeys.Package.StartsWith, "GetPackages");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Package
                        .OrderBy(x => x.Name)
                        .ToList();
            });
        }
        public IList<Package> GetallPackages()
        {
            var cacheKey = string.Concat(CacheKeys.Package.StartsWith, "GetallPackages");
            return _cacheService.CachePerRequest(cacheKey, () => 
            GetAll()
            .ToList());
        }
        public IEnumerable<Package> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.Package.StartsWith, "GetAll");
            return _cacheService.CachePerRequest(cacheKey, () => _context.Package.ToList())
            .OrderBy(x => x.SortOrder);
        }
        public Package Add(Package package)
        {
            return _context.Package.Add(package);
        }
        public void Delete(Package package)
        {
            _context.Package.Remove(package);
        }
        public Package GetPackage(Guid packageId)
        {
            return _context.Package
            .FirstOrDefault(x => x.Id == packageId);
        }
        public Package GetBasicPackage()
        {
            return _context.Package
            .FirstOrDefault(x => x.SortOrder == AppConstants.BasicPackageSortOrder);
        }
        public Package Get(string name)
        {
            return _context.Package
           .FirstOrDefault(x => x.Name == name);
        }
    }
    public class PackageAttributeNotFoundException : Exception
    {
    }
}
