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

        [Route("administrator/dashboard/{receiverName}")]
        public IActionResult Dashboard(string receiverName = "All")
        {
            return View(BuildDirectMessage(receiverName));
        }

        public DashboardModel BuildDirectMessage(string receiverName) 
        {
            DashboardModel model = new DashboardModel
            {
                users = userManager.Users.ToList(),
                currentUserName = context.HttpContext.User.Identity.Name
            };

            if(receiverName == "All")
            {
                model.messages = db.Messages.Where(x => x.ReceiverName == "All").ToList();
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
    }
}