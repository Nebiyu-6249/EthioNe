using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("d68c289a-e3f7-4f55-ae4f-fc7ac2147781")]
    [Name("WeekPackage")]
    [DisplayName("WeekPackage")]
    [Description("WeekPackage")]
    [Bonus(1)]
    [Discount(1)]
    [Video(false)]
    [Color("Green")]
    [Sort(10)]
    [Images(2)]
    [Address(0)]
    public class WeekPackage : IWeekPackage
    {
      
    }
}
