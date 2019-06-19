using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chat_application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chat_application.Controllers
{
    [Authorize]
    public class AdministratorController : Controller
    {
        private readonly IHttpContextAccessor context;
        private readonly ChatDbContext db;


        public AdministratorController(IHttpContextAccessor context, ChatDbContext db)
        {
            this.context = context;
            this.db = db;
        }

        [Route("administrator/dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}