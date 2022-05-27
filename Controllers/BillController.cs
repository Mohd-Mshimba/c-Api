using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ZanMalipo.Data;
using ZanMalipo.Models;
using ZanMalipo.ViewModels;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Signature;


namespace ZanMalipo.Controllers
{
    [ApiController]
    [Route("api/bills")]
    public class BillController : ControllerBase
    {
        private readonly ZanMalipoDbContext _context;
        private Utility util;

        public BillController(ZanMalipoDbContext context)
        {
            _context = context;
            util = new Utility();
        }

        [HttpPost]
        public ActionResult<string> AddBill([FromBody] BillRequest request)
        {
            BillResponse response = new BillResponse();
            Bill b = new Bill();
            List<int> serviceIds = request.Services;
            List<Service> serviceResp = new List<Service>();

            double amount = 0;
            foreach (var id in serviceIds)
            {
                Service service = _context.Services.Where(x => x.Id == id).FirstOrDefault();
                amount += service.Price;
            }

            b.BillName = request.BillName;
            b.Amount = amount;
            b.billStatus = "CREATED";

            _context.Bills.Add(b);
            _context.SaveChanges();

            foreach (var id in serviceIds)
            {
                Service service = _context.Services.Where(x => x.Id == id).FirstOrDefault();
                BillService billService = new BillService();
                billService.BillId = b.Id;
                billService.ServiceId = id;
                billService.Price = service.Price;

                _context.BillServices.Add(billService);
                _context.SaveChanges();
                serviceResp.Add(service);
            }

            response.Id = b.Id;
            response.BillName = request.BillName;
            response.Amount = amount;
            response.billStatus = b.billStatus;
            response.services = serviceResp;

            string xmlBill = util.createBill(response);
            string signature = util.generateSignature(xmlBill);

            return Ok(signature);
        }
    }
}
