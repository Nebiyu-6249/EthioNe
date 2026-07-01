using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Domain.Enums;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("a54ec5d1-111d-4698-b2d0-78fbdaa52d1b")]
    [Name("Month Package")]
    [DisplayName("Month Package")]
    [Description("MonthPackage")]
    [Bonus(7)]
    [Discount(2)]
    [Images(2)]
    [Video(false)]
    [Color("Green")]
    [Sort(10)]
    [Address(2)]
    public  class MonthPackage : IRegularPackage
    {
       
    }
}
