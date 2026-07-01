using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Interfaces.Packages;
namespace EthioNegarit.Packages
{
    [Id("6EF03C66-9094-40B6-9F60-10065BF89104")]
    [Name("Annual")]
    [DisplayName("Package.Annual.Name")]
    [Description("Package.Annual.Desc")]
    [Bonus(30)]
    [Discount(10)]
    [Images(2)]
    [Address(5)]
    [Video(false)]
    [Color("Green")]
    [Sort(10)]
    public class AnnualPackage : IAnnualPackage
    {   
               
    }
}
