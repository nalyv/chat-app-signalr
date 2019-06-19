using chat_application.Hubs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace chat_application.Identity
{
    public class ChatDbContext : IdentityDbContext<AppIdentityUser, AppIdentityRole, string>
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {

        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Connection> Connections { get; set; }
    }
}
