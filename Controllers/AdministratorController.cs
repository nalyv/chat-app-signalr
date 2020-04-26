using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chat_application.Identity;
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

        [Route("administrator/dashboard")]
        public IActionResult Dashboard()
        {
            var deneme = userManager.Users.ToList();
            return View(deneme);
        }
    }
}