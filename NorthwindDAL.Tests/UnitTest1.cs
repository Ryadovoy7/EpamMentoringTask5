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
    public class OrderRepositoryUnitTest
    {
        private const string connectionString = "Initial Catalog=Northwind;Data Source=RYADOVOY-PC;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private readonly OrderRepository repository = new OrderRepository(connectionString);


        [TestMethod]
        public void AddNew_AddEmptyOrder_ReturnNotNullOrder()
        {
            // Arrange
            var order = new Order();
            // Act
            var result = repository.AddNew(order);
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddNew_AddFilledOrder_ReturnNotNullOrder()
        {
            // Arrange
            var order = CreateFilledOrder_FKsNull();
            // Act
            var result = repository.AddNew(order);
            // Assert
            Assert.IsNotNull(result);
        }

        private Order CreateFilledOrder_FKsNull()
        {
            return new Order(0, null, null, DateTime.MaxValue, DateTime.MaxValue,
                DateTime.MaxValue, null, 0, "Test", "Test", "Test", "Test", "Test", "Test");
        }

        [TestMethod]
        public void GetOrderByID_AddFilledOrder_ReturnSameOrder() // фактически тестируется в AddNew...
        {
            // Arrange
            var orderOriginal = CreateFilledOrder_FKsNull();
            var shipNameOriginal = orderOriginal.ShipName;
            // Act
            var id = repository.AddNew(orderOriginal).OrderID;
            var orderReturned = repository.GetOrderByID(id);
            // Assert
            Assert.AreEqual(shipNameOriginal, orderReturned.ShipName);
        }

        [TestMethod]
        public void GetOrderByID_GetExistingOrderWithDetails_GetsMultipleDetails()
        {
            // Arrange
            const int existingOrderID = 10248;
            // Act
            var order = repository.GetOrderByID(existingOrderID, true);
            // Assert
            Assert.IsTrue(order.Details.Count > 0);
        }

        [TestMethod]
        public void GetOrders_AddOrder_ReturnsMultipleOrders()
        {
            // Arrange
            repository.AddNew(new Order());
            // Act
            var orders = repository.GetOrders();
            // Assert
            Assert.IsTrue(orders.Count() > 0);
        }

        [TestMethod]
        public void Update_AddOrder_ChangeShipName_ReturnsUpdatedShipName()
        {
            // Arrange
            var order = repository.AddNew(new Order());
            // Act
            var updatedShipName = Guid.NewGuid().ToString();
            order.ShipName = updatedShipName;
            var updatedOrder = repository.Update(order);
            // Assert
            Assert.AreEqual(updatedOrder.ShipName, updatedShipName);
        }

        [TestMethod]
        public void Delete_AddOrder_DeleteOrder_OrderDoesNotExistInDB()
        {
            // Arrange
            var orderID = repository.AddNew(new Order()).OrderID;
            // Act
            var result = repository.Delete(orderID);
            var deletedOrder = repository.GetOrderByID(orderID);
            // Assert
            Assert.IsNull(deletedOrder);
        }

        [TestMethod]
        public void SubmitToWork_AddOrder_Submit_OrderDateChanged()
        {
            // Arrange
            var orderID = repository.AddNew(new Order()).OrderID;
            // Act
            var maxDateTime = DateTime.MaxValue;
            repository.SubmitToWork(orderID, maxDateTime);
            var submittedOrder = repository.GetOrderByID(orderID);
            // Assert
            Assert.AreEqual(submittedOrder.OrderDate.Value.Date, maxDateTime.Date);
        }

        [TestMethod]
        public void MarkAsComplete_AddOrder_Mark_ShippedDateChanged()
        {
            // Arrange
            var orderID = repository.AddNew(new Order()).OrderID;
            // Act
            var maxDateTime = DateTime.MaxValue;
            repository.MarkAsComplete(orderID, maxDateTime);
            var submittedOrder = repository.GetOrderByID(orderID);
            // Assert
            Assert.AreEqual(submittedOrder.ShippedDate.Value.Date, maxDateTime.Date);
        }

        [TestMethod]
        public void GetCustOrderHist_ReturnsProcedureResult()
        {
            // Arrange
            const string existingCustomerID = "VINET";
            // Act
            var procResult = repository.GetCustOrderHist(existingCustomerID);
            // Assert
            Assert.IsTrue(procResult.Count() > 0);
        }

        [TestMethod]
        public void GetCustOrdersDetail_ReturnsProcedureResult()
        {
            // Arrange
            const int existingOrderID = 10248;
            // Act
            var procResult = repository.GetCustOrdersDetail(existingOrderID);
            // Assert
            Assert.IsTrue(procResult.Count() > 0);
        }

        [TestMethod]
        public void AddOrder_MarkAsComplete_TryToChange_ThrowsInvalidOrderStateException()
        {
            // Arrange
            var orderID = repository.AddNew(new Order()).OrderID;
            repository.MarkAsComplete(orderID, DateTime.MaxValue);
            var completeOrder = repository.GetOrderByID(orderID);
            // Act 
            // Assert
            Assert.ThrowsException<InvalidOrderStateException>(() => completeOrder.ShipName = String.Empty);
        }
    }
}
