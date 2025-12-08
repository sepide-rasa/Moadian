using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace Avarez
{
    [HubName("S_hub")]
    public class SignalrHub : Hub
    {
        public void ReloadOnlineUser()
        {
            if (Clients != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<SignalrHub>();
                context.Clients.All.LoadOnlineUser();
            }
        }
        public void ReloadTickets()
        {
            if (Clients != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<SignalrHub>();
                context.Clients.All.LoadTickets();
            }
        }
        public void Send(string message)
        {
            if (Clients != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<SignalrHub>();
                context.Clients.All.broadcastMessage(message);
            }
        }
        /*public void ReloadCarExperience()
        {
            if (Clients != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<SignalrHub>();
                context.Clients.All.LoadCarExperience();
            }
        }*/
    }
}