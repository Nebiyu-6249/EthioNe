using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("52284d2b-7ed6-4154-9ccc-3a7d99b18cca")]

    [Name("ThreeMonthsPackage")]
    [DisplayName("ThreeMonthsPackage")]
    [Description("ThreeMonthsPackage")]
    [Bonus(15)]
    [Discount(5)]
    [Video(false)]
    [Color("Green")]
    [Sort(10)]
    [Images(2)]
    [Address(2)]
    public class QuarterPackage : IRegularPackage
    {
     
    }
}
