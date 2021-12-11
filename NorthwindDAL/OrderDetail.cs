namespace NorthwindDAL
{
    public class OrderDetail
    {
        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public float Discount { get; set; }   

        public string ProductName { get; set; }
    }
}