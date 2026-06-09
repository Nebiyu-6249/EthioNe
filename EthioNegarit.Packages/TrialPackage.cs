using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Domain.Attributes;
using EthioNegarit.Core.Interfaces.Packages;

namespace EthioNegarit.Packages
{
    [Id("79405a62-33ee-4833-80cc-b39e00b028ee")]
    [Name("Trial")]
    [DisplayName("Package.Mega.Name")]
    [Description("Package.Mega.Desc")]
    [Bonus(30)]
    [Images(0)]
    [Video(false)]
    [Color("Green")]
    [Sort(AppConstants.TrialPackageSortOrder)]
    [Address(0)]
    [Discount(100)]
   public class TrialPackage : ITrialPackage
    {
    }
}
