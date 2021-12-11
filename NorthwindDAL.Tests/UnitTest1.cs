using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace NorthwindDAL.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const string connectionString = "Initial Catalog=Northwind;Data Source=RYADOVOY-PC;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        [TestMethod]
        public void TestMethod1()
        {
            var repository = new OrderRepository(connectionString);
            var orders = repository.GetOrders().ToList();

            var orderTest = repository.GetOrderByID(orders[0].OrderID, false);
        }

        [TestMethod]
        public void AddEmptyOrder_ReceiveID()
        {
            var repository = new OrderRepository(connectionString);
            var result = repository.AddNew(new Order());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void proc1()
        {
            var repository = new OrderRepository(connectionString);
            var test = repository.GetCustOrderHist("VINET");
        }

        [TestMethod]
        public void proc2()
        {
            var repository = new OrderRepository(connectionString);
            var test = repository.GetCustOrdersDetail(10248);
        }

        // todo метод попытка изменить поля у ордера не забыть
    }
}
