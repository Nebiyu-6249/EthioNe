using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Interfaces.Packages;
namespace EthioNegarit.Packages
{
    [Id("d338fd50-6c60-443e-b4d7-b39e00b028a3")]
    [Name("Mega")]
    [DisplayName("Package.Mega.Name")]
    [Description("Package.Mega.Desc")]
    [Price(1.5)]
    [Bonus(30)]
    [Images(5)]
    [Video(true)]
    [Color("Orange")]
    [Sort(2)]
    [Address(0)]
    [Discount(5)]
  public  class MegaPackage : IMegaPackage
    {
    }
}
