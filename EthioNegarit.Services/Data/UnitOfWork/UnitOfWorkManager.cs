using System.Data.Entity;
using EthioNegarit.Services.Data.Context;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Interfaces;
using EthioNegarit.Services.Migrations;

namespace EthioNegarit.Services.Data.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private bool _isDisposed;
        private readonly EthioNegaritContext _context;

       public UnitOfWorkManager(IEthioNegaritContext context)
        {
         
   Database.SetInitializer(new MigrateDatabaseToLatestVersion<EthioNegaritContext, Configuration>(SiteConstants.Instance.EthioNegaritContext));
  _context = context as EthioNegaritContext;
              
        }

        /// <summary>
        /// Provides an instance of a unit of work. This wrapping in the manager
        /// class helps keep concerns separated
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(_context);
        }

        /// <summary>
        /// Make sure there are no open sessions.
        /// In the web app this will be called when the injected UnitOfWork manager
        /// is disposed at the end of a request.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _context.Dispose();
                _isDisposed = true;
            }
        }
        
    }
}

