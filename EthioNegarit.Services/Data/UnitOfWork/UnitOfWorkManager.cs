using System;
using System.Data.Entity;
using EthioNegarit.Services.Data.Context;
using EthioNegarit.Core.Interfaces.UnitOfWork;
using EthioNegarit.Core.Constants;
using EthioNegarit.Core.Interfaces;
using EthioNegarit.Services.Migrations;
using EthioNegarit.Utilities;

namespace EthioNegarit.Services.Data.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private static bool _initializerSet;
        private static readonly object InitializerLock = new object();

        private bool _isDisposed;
        private readonly EthioNegaritContext _context;

       public UnitOfWorkManager(IEthioNegaritContext context)
        {
            if (!_initializerSet)
            {
                lock (InitializerLock)
                {
                    if (!_initializerSet)
                    {
                        // Automatic migrations (plus the Seed method) used to run against the
                        // live database on every cold start; a slow or lock-blocked migration
                        // makes the first request hang with no error. Migration-on-startup is
                        // therefore opt-in: set appSetting RunDatabaseMigrationsOnStartup to
                        // "true" when deploying a model change or installing into an empty
                        // database, then set it back to "false" once the schema is updated.
                        var runMigrations = ConfigUtils.GetAppSetting("RunDatabaseMigrationsOnStartup", "false");
                        if (string.Equals(runMigrations, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EthioNegaritContext, Configuration>(SiteConstants.Instance.EthioNegaritContext));
                        }
                        else
                        {
                            Database.SetInitializer<EthioNegaritContext>(null);
                        }
                        _initializerSet = true;
                    }
                }
            }
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

