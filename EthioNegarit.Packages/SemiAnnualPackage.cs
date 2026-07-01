using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("c9913ee2-b8e0-4543-8930-c723497ee65c")]
    [Name("SixMonthPackage")]
    [DisplayName("SixMonthPackage")]
    [Description("SixMonthPackage")]
    [Bonus(20)]
    [Discount(10)] 
    [Images(2)]
    [Address(3)]
    [Video(false)]
    [Color("Green")]
    [Sort(10)]
    public class SemiAnnualPackage : IRegularPackage
    {
     
    }
}
