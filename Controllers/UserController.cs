using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ZanMalipo.Data;
using ZanMalipo.ViewModels;
using ZanMalipo.Models;

namespace ZanMalipo.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController:ControllerBase
    {
        private readonly ZanMalipoDbContext _context;

        public UserController(ZanMalipoDbContext ctx)
        {
            _context = ctx;
        }

        [HttpGet]
        public ActionResult<List<UserResponse>> GetAll()
        {
            List<User> userList = _context.Users.ToList();
            List<UserResponse> users = new List<UserResponse>();

            foreach (User user in userList)
            {
                UserResponse ur = new UserResponse();
                ur.Id = user.Id;
                ur.FullName = user.FullName;
                ur.UserName = user.UserName;
                users.Add(ur);
            }
            return Ok(users);
        }
         [HttpPost("add")]
        public ActionResult<UserResponse> CreateService([FromBody] UserRequest request){
            User s = new User();
            s.FullName = request.FullName;
            s.UserName = request.UserName;
            s.Password = request.Password;
            _context.Users.Add(s);
            _context.SaveChanges();

            UserResponse resp = new UserResponse();
            resp.FullName = s.FullName;
            resp.UserName = s.UserName;
            return Ok(resp);
        }
        
        [HttpGet("id")]
        public ActionResult<string> GetServicesById(int Id){

            User u = _context.Users.Find(Id);
            if(u == null){
                return NotFound("Service Not Found");
            }
                UserResponse ur = new UserResponse();
                ur.Id = u.Id;
                ur.FullName = u.FullName;
                ur.UserName = u.UserName;
             return Ok(ur);
        }

         [HttpPut("id")]
        public ActionResult<UserResponse> updateUser([FromBody] UserRequest request, int id){

            User u = _context.Users.Find(id);
            u.FullName = request.FullName;
            u.UserName = request.UserName;
            u.Password = request.Password;
            _context.Users.Update(u);
            _context.SaveChanges();

            UserResponse resp = new UserResponse();
            resp.FullName = u.FullName;
            resp.UserName = u.UserName;
            return Ok(resp);
        }
    }
}
