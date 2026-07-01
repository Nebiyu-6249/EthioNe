using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("41039498-60b9-4023-8190-b3ff0105798f")]
    [Name("Ultimate")]
    [DisplayName("Package.Ultimate.Name")]
    [Description("Package.Ultimate.Desc")]
    [Price(2)]
    [Bonus(30)]
    [Address(5)]
    [Discount(15)]
    [Video(true)]
    [Files(5)]
    [Color("Green")]
    [Sort(3)]
  public  class UltimatePackage : IUltimatePackage
    {
    }
}
