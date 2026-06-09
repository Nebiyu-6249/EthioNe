namespace EthioNegarit.Services.Data.NPoco
{
    using System.Web;
    using Core.Constants;
   using global::NPoco;
    internal static class ContextPerRequest
    {
         internal static IDatabase Current
         {
             get
             {
                 if (!HttpContext.Current.Items.Contains(SiteConstants.Instance.EthioNegaritContext))
                 {
                     HttpContext.Current.Items.Add(SiteConstants.Instance.EthioNegaritContext, new Database(SiteConstants.Instance.EthioNegaritContext));
                 }
                 return HttpContext.Current.Items[SiteConstants.Instance.EthioNegaritContext] as IDatabase;
             }
         }
    }
}
