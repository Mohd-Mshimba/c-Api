namespace ZanMalipo.Models{
    public class BillService{
        public int Id { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public double Price { get; set; }
    }
}