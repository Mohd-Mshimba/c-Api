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
    public class ServiceController : ControllerBase
    {
        private readonly ZanMalipoDbContext _context;

        public ServiceController(ZanMalipoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<ServiceResponse>> GetServices()
        {
            List<Service> services = _context.Services.ToList();
            List<ServiceResponse> response = new List<ServiceResponse>();
            foreach (Service s in services)
            {
                ServiceResponse sr = new ServiceResponse();
                sr.Id = s.Id;
                sr.ServiceName = s.ServiceName;
                sr.GFSCode = s.GFSCode;
                sr.Price = s.Price;
                response.Add(sr);
            }

            return Ok(response);
        }

        [HttpGet("id")]
        public ActionResult<string> GetServicesById(int id)
        {
            Service s = _context.Services.Where(x => x.Id == id).FirstOrDefault();
            if (s == null)
            {
                return NotFound("Service Not Found");
            }
            ServiceResponse sr = new ServiceResponse();
            sr.Id = s.Id;
            sr.ServiceName = s.ServiceName;
            sr.GFSCode = s.GFSCode;
            sr.Price = s.Price;

            return Ok(sr);
        }

        [HttpPost]
        public ActionResult<ServiceResponse> CreateService([FromBody] ServiceRequest request)
        {
            Service s = new Service();
            s.Price = request.Price;
            s.GFSCode = request.GFSCode;
            s.ServiceName = request.ServiceName;

            //mapping model with viewModel
            ServiceResponse sr = new ServiceResponse();
            sr.Id = s.Id;
            sr.ServiceName = s.ServiceName;
            sr.GFSCode = s.GFSCode;
            sr.Price = s.Price;

            _context.Services.Add(s);
            _context.SaveChanges();

            return Ok(sr);
        }
    }
}