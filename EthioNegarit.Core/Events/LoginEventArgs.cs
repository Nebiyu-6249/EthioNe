using EthioNegarit.Core.Interfaces.UnitOfWork;
namespace EthioNegarit.Core.Events
{
   public class LoginEventArgs : EthioNegaritEventArgs
    {
        public string ReturnUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
    }
}
