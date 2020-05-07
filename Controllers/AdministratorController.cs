using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chat_application.Identity;
using chat_application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace chat_application.Controllers
{
    [Authorize]
    public class AdministratorController : Controller
    {
        private readonly IHttpContextAccessor context;
        private readonly ChatDbContext db;
        private readonly UserManager<AppIdentityUser> userManager;

        public AdministratorController(
            IHttpContextAccessor context, 
            ChatDbContext db,
            UserManager<AppIdentityUser> userManager)
        {
            this.context = context;
            this.db = db;
            this.userManager = userManager;
        }

        [Route("administrator/dashboard/{type}/{receiverName}")]
        public IActionResult Dashboard(string type, string receiverName)
        {
            return View(BuildDirectMessage(type, receiverName));
        }

        public DashboardModel BuildDirectMessage(string type, string receiverName) 
        {
            DashboardModel model = new DashboardModel
            {
                users = userManager.Users.ToList(),
                currentUserName = context.HttpContext.User.Identity.Name,
                groups = db.Groups.ToList(),
                memberGroups = GetMemberGroups(GetCurrentUser())
            };

            if(type == "everyone")
            {
                model.messages = db.Messages.Where(x => x.ReceiverName == "All").ToList();
            }
            else if(type == "group")
            {
                model.messages = db.Messages.Where(x => x.ReceiverName == receiverName).ToList();
            }
            else
            {
                model.messages = db.Messages
                    .Where(x => (x.SenderName == model.currentUserName || x.SenderName == receiverName) &&
                                (x.ReceiverName == receiverName || x.ReceiverName == model.currentUserName))
                    .ToList();
            }

            return model;
        }

        public AppIdentityUser GetCurrentUser()
        {
            string userName = context.HttpContext.User.Identity.Name;
            return userManager.Users.Where(x=> x.UserName == userName).FirstOrDefault();
        }
        public List<Group> GetMemberGroups(AppIdentityUser user)
        {
            List<UserGroup> userGroups = db.UserGroups.Where(x=> x.User == user).ToList();

            return db.Groups.Where(x=> userGroups.Any(s=> s.Group == x)).ToList();
        }
    }
}