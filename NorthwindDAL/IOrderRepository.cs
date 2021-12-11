using System;
using System.Collections.Generic;

namespace NorthwindDAL
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();

        public Order AddNew(Order newOrder);

        public Order GetOrderByID(int id, bool detailed);

        public Order Update(Order newOrder);

        public bool Delete(int id);

        public bool SubmitToWork(int id, DateTime orderDate);

        public bool MarkAsComplete(int id, DateTime shippedDate);

        public IEnumerable<CustOrderHist> GetCustOrderHist(string customerID);

        public IEnumerable<CustOrdersDetail> GetCustOrdersDetail(int orderID);
    }
}
