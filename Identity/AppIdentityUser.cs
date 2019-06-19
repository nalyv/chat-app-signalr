using System.Collections.Generic;
using chat_application.Hubs;
using Microsoft.AspNetCore.Identity;

namespace chat_application.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        public int isActive { get; set; }
        public ICollection<Connection> Connections { get; set; }
    }
}
