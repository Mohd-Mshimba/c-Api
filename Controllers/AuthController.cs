using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ZanMalipo.Data;
using ZanMalipo.Models;
using ZanMalipo.ViewModels;

namespace ZanMalipo.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class AuthController : ControllerBase
    {
        private readonly ZanMalipoDbContext _context;
        public AuthController(ZanMalipoDbContext context)
        {
            _context = context;
        }
        [HttpPost("authenticate")]
        // [HttpPost]
        public ActionResult<UserResponse> Authenticate([FromBody] UserRequest model)
        {
            User us = new User();
            us.UserName = model.UserName;
            us.Password = model.Password;

            UserResponse response = new UserResponse();
            response.UserName = us.UserName;

            User user = _context.Users.Where(x => x.UserName == model.UserName && x.Password == model.Password).FirstOrDefault();
            if (user == null)
                return NotFound("Username or password is incorrect");

            return Ok(response);
        }
    }
}