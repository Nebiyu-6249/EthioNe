using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("b39d78c1-f2d7-479f-8728-b39e00e13ca6")]
    [Name("Basic")]
    [DisplayName("Package.Basic.Name")]
    [Description("Package.Basic.Desc")]
    [Price(1)]
    [Bonus(30)]
    [Address(0)]
    [Discount(0)]
    [Video(false)]
    [Files(0)]
    [Color("Dark")]
    [Sort(AppConstants.BasicPackageSortOrder)]
  public  class BasicPackage : IBasicPackage
    {
    }
}
