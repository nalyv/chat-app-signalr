using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task SendMessageToAll(string receiverName, string message)
        {
            var senderName = accessor.HttpContext.User.Identity.Name;
            dbContext.Messages.Add(CreateMessage(senderName, receiverName, message));

            await dbContext.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", senderName, receiverName, message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToUser(string receiverName, string message)
        {
            var senderName = accessor.HttpContext.User.Identity.Name;
            string connectionId = _connections.Find(receiverName);
            
            dbContext.Messages.Add(CreateMessage(senderName, receiverName,message));
            await dbContext.SaveChangesAsync();

            if(connectionId == "")
                await Clients.Caller.SendAsync("ReceiveMessage", message);
            else
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderName, receiverName, message);
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

        public void ReadMessages(string senderName, string receiverName) 
        {
            List<Message> messages = dbContext.Messages
                .Where(x=> x.SenderName == senderName &&
                       x.ReceiverName == receiverName &&
                       x.isRead == false)
                .ToList();
            
            foreach(Message msg in messages)
            {
                msg.isRead = true;
                dbContext.Messages.Update(msg);
            }
            dbContext.SaveChanges();
        }
        public Message CreateMessage(string senderName, string receiverName, string MessageContent)
        {
            return new Message()
            {
                SenderName = senderName,
                ReceiverName = receiverName,
                MessageContent = MessageContent,
                MessageDate = DateTime.Now,
                isRead = false
            };
        }
    }
}