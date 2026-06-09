using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EthioNegarit.WebUI.App_Start.Startup))]
//[assembly: OwinStartup(typeof(SignalRChat.Startup))]

namespace EthioNegarit.WebUI.App_Start
    {
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Map the SignalR service
            app.MapSignalR();
        //    app.ChatHub<ChatHub>("/chatHub");
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
       /* public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }*/
    }
}
