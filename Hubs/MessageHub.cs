using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using chat_application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace chat_application.Hubs
{
    [Authorize]
    public class MessageHub: Hub
    {
        private ChatDbContext dbContext;
        private readonly UserManager<AppIdentityUser> userManager;
        private readonly SignInManager<AppIdentityUser> signInManager;
        private readonly static ConnectionMapping<string> _connections = 
            new ConnectionMapping<string>();

        private string connectionId;

        private IHttpContextAccessor accessor;
        public MessageHub(ChatDbContext dbContext,
                        UserManager<AppIdentityUser> userManager,
                        SignInManager<AppIdentityUser> signInManager,
                        IHttpContextAccessor accessor)
        {
           this.dbContext = dbContext;
           this.userManager = userManager;
           this.signInManager = signInManager;
           this.accessor = accessor;
        }
        public async Task SendMessageToAll(string user,string message)
        {
            var name = accessor.HttpContext.User.Identity.Name;
            dbContext.Messages.Add(CreateMessage(user,message));

            await dbContext.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", name, message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToUser(string user, string message)
        {
            var name = accessor.HttpContext.User.Identity.Name;
            string connectionId = _connections.Find(user);
            
            dbContext.Messages.Add(CreateMessage(user,message));
            await dbContext.SaveChangesAsync();

            if(connectionId == "")
                await Clients.Caller.SendAsync("ReceiveMessage", message);
            else
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", name, message);
        }

        public Task JoinGroup(string group)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public Task SendMessageToGroup(string group, string message)
        {
            return Clients.Group(group).SendAsync("ReceiveMessage", message);
        }
        public override async Task OnConnectedAsync()
        {
            string name = accessor.HttpContext.User.Identity.Name;
            _connections.Add(name,Context.ConnectionId);

            await Clients.Client(Context.ConnectionId).SendAsync("UserConnnectName", name);

            await Clients.All.SendAsync("UserConnected", _connections.GetAllNames());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            string name = accessor.HttpContext.User.Identity.Name;
            _connections.Remove(name, Context.ConnectionId);

            await Clients.All.SendAsync("UserDisconnected", name);
            await base.OnDisconnectedAsync(ex);
        }

        public Message CreateMessage(string Username, string MessageContent)
        {
            return new Message()
            {
                Username = Username,
                MessageContent = MessageContent,
                MessageDate = DateTime.Now
            };
        }
    }
}