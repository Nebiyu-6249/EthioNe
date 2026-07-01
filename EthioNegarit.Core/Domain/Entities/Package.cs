using System;
using System.Collections.Generic;
using EthioNegarit.Utilities;
using EthioNegarit.Core.Domain.Enums;
namespace EthioNegarit.Core.Domain.Entities
{
    public partial class Package : Entity
    {
        public Package()
        {
            Id = GuidComb.GenerateComb();
        }
        /// <summary>
        /// Specifies the target Package interface names matched to the corresponding Package type
        /// </summary>
        public static readonly Dictionary<PackageType, string> PackageClassNames = new Dictionary<PackageType, string>
                                                             {
                                                               {PackageType.Basic,"EthioNegarit.Core.Interfaces.Packages.IBasicPackage"},
                                                                 {PackageType.Super,"EthioNegarit.Core.Interfaces.Packages.ISuperPackage"},
                                                                 { PackageType.Ultimate,"EthioNegarit.Core.Interfaces.Packages.IUltimatePackage"},
                                                             };

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int? SortOrder { get; set; }
        public int Discount { get; set; }
        public string Type { get; set; }
        public int? Files { get; set; }
        public bool? EnableVideo { get; set; }
        public double? PriceRate { get; set; }
        public int? Address { get; set; }
        public int Bonus { get; set; }
        public string Color { get; set; }
        public virtual IList<Payment> Payments { get; set; }
    }
}
