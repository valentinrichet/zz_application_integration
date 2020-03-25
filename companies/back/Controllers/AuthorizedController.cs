using AiCompany.Services.JwtGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCompany.Controllers
{
    [Authorize]
    public abstract class AuthorizedController : Controller
    {
        protected string UserId 
        {
            get
            {
                return User.FindFirst(claim => claim.Type == JwtGenerator.CustomClaimTypes.Id).Value;
            }
        }

        public AuthorizedController(ILogger<AuthorizedController> logger) : base(logger)
        {
        }

        protected bool IsUser(ulong id)
        {
            return User.FindFirst(claim => claim.Type == JwtGenerator.CustomClaimTypes.Type && claim.Value == "COMPANY") != null && User.FindFirst(claim => claim.Type == JwtGenerator.CustomClaimTypes.Id && claim.Value == id.ToString()) != null;
        }
    }
}
