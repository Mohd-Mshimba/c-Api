namespace ZanMalipo.Models{
    public class Bill{
        public int Id { get; set; }
        public string BillName { get; set; }
        public string ControlNo { get; set; }
        public string billStatus { get; set; }
        public string ReceiptNo { get; set; }
        public double Amount { get; set; }
    }
}