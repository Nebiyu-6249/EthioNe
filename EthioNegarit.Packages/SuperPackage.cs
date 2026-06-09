using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("6EF03C66-9094-40B6-9F60-10065BF89105")]
    [Name("Super")]
    [DisplayName("Package.Super.Name")]
    [Description("Package.Super.Desc")]
    [Price(1.5)]
    [Bonus(30)]
    [Address(0)]
    [Discount(10)]
    [Video(true)]
    [Files(5)]
    [Color("Blue")]
    [Sort(2)]
    public class SuperPackage : ISuperPackage
    {
    }
}
