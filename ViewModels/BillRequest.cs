using System.Collections.Generic;

namespace ZanMalipo.ViewModels
{
    public class BillRequest
    {
        public string BillName { get; set; }
        public List<int> Services { get; set; }
    }
}