using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace NorthwindDAL
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SqlClientFactory _dbProviderFactory = SqlClientFactory.Instance;
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Order> GetOrders()
        {
            var orders = new List<Order>();
            using (var conn = _dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Northwind.Orders";
                    cmd.CommandType = CommandType.Text;
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var order = ReadOrder(dataReader);
                            orders.Add(order);
                        }                           
                    };
                }
            }
            return orders;
        }
      
        public Order GetOrderByID(int id, bool detailed = false)
        {
            var orders = new List<Order>();
            using (var conn = _dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    var cmdStr = "SELECT OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, " +
                        "Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry " +
                        "FROM Northwind.Orders WHERE OrderID = @id";

                    if (detailed)
                        cmdStr += "; SELECT d.OrderID, d.ProductID, d.UnitPrice, d.Quantity, d.Discount, p.ProductName " +
                        "FROM Northwind.[Order Details] AS d " +
                        "JOIN Northwind.Products AS p ON d.ProductID = p.ProductID " +
                        "WHERE OrderID = @id;";

                    cmd.CommandText = cmdStr;
                    cmd.CommandType = CommandType.Text;

                    var paramId = cmd.CreateParameter();
                    paramId.ParameterName = "@id";
                    paramId.Value = id;

                    cmd.Parameters.Add(paramId);

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        if (!dataReader.HasRows) return null;

                        dataReader.Read();

                        var order = ReadOrder(dataReader);

                        dataReader.NextResult();
                        order.Details = new List<OrderDetail>();

                        while (dataReader.Read())
                        {
                            var detail = ReadOrderDetails(dataReader);
                            order.Details.Add(detail);
                        }
                        return order;
                    };
                }
            }
        }

        private Order ReadOrder(DbDataReader dataReader)
        {
            var order = new Order()
            {
                // для получения nullable значений можно использовать IsDBNull + Get(...)
                OrderID = dataReader.GetInt32(0),
                CustomerID = Convert.IsDBNull(dataReader.GetValue(1)) ? null : dataReader.GetString(1),
                EmployeeID = Convert.IsDBNull(dataReader.GetValue(2)) ? null : dataReader.GetInt32(2),
                OrderDate = Convert.IsDBNull(dataReader.GetValue(3)) ? null : dataReader.GetDateTime(3),
                RequiredDate = Convert.IsDBNull(dataReader.GetValue(4)) ? null : dataReader.GetDateTime(4),
                ShippedDate = Convert.IsDBNull(dataReader.GetValue(5)) ? null : dataReader.GetDateTime(5),
                ShipVia = Convert.IsDBNull(dataReader.GetValue(6)) ? null : dataReader.GetInt32(6),
                Freight = Convert.IsDBNull(dataReader.GetValue(7)) ? null : dataReader.GetDecimal(7),
                ShipName = Convert.IsDBNull(dataReader.GetValue(8)) ? null : dataReader.GetString(8),
                ShipAddress = Convert.IsDBNull(dataReader.GetValue(9)) ? null : dataReader.GetString(9),
                ShipCity = Convert.IsDBNull(dataReader.GetValue(10)) ? null : dataReader.GetString(10),
                ShipRegion = Convert.IsDBNull(dataReader.GetValue(11)) ? null : dataReader.GetString(11),
                ShipPostalCode = Convert.IsDBNull(dataReader.GetValue(12)) ? null : dataReader.GetString(12),
                ShipCountry = Convert.IsDBNull(dataReader.GetValue(13)) ? null : dataReader.GetString(13)
            };

            if (order.ShippedDate != null)
                order.State = OrderState.Completed;
            else if (order.OrderDate != null)
                order.State = OrderState.InProgress;
            else
                order.State = OrderState.New;

            return order;
        }

        private OrderDetail ReadOrderDetails(DbDataReader dataReader)
        {
            var detail = new OrderDetail()
            {
                OrderID = dataReader.GetInt32(0),
                ProductID = dataReader.GetInt32(1),
                UnitPrice = dataReader.GetDecimal(2),
                Quantity = dataReader.GetInt16(3),
                Discount = dataReader.GetFloat(4),
                ProductName = dataReader.GetString(5)
            };
            return detail;
        }

        public Order AddNew(Order newOrder)
        {
            int result;
            using (var conn = (SqlConnection)_dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Northwind.Orders(CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, " +
                        "Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry) " +
                        "OUTPUT INSERTED.OrderID " +
                        "VALUES(@CustomerID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, " +
                        "@Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry)";
                    cmd.CommandType = CommandType.Text;
                  
                    cmd.Parameters.AddWithValue("@CustomerID", newOrder.CustomerID);
                    cmd.Parameters.AddWithValue("@EmployeeID", newOrder.EmployeeID);
                    cmd.Parameters.AddWithValue("@OrderDate", newOrder.OrderDate);
                    cmd.Parameters.AddWithValue("@RequiredDate", newOrder.RequiredDate);
                    cmd.Parameters.AddWithValue("@ShippedDate", newOrder.ShippedDate);
                    cmd.Parameters.AddWithValue("@ShipVia", newOrder.ShipVia);
                    cmd.Parameters.AddWithValue("@Freight", newOrder.Freight);
                    cmd.Parameters.AddWithValue("@ShipName", newOrder.ShipName);
                    cmd.Parameters.AddWithValue("@ShipAddress", newOrder.ShipAddress);
                    cmd.Parameters.AddWithValue("@ShipCity", newOrder.ShipCity);
                    cmd.Parameters.AddWithValue("@ShipRegion", newOrder.ShipRegion);
                    cmd.Parameters.AddWithValue("@ShipPostalCode", newOrder.ShipPostalCode);
                    cmd.Parameters.AddWithValue("@ShipCountry", newOrder.ShipCountry);

                    result = (int)cmd.ExecuteScalar();
                }
            }
            
            return GetOrderByID(result);
        }

        public Order Update(Order updatedOrder)
        {
            var originalOrder = GetOrderByID(updatedOrder.OrderID);
            if (HasUnchangeableFields(updatedOrder, originalOrder)) // todo перенести в ордер
                return null; // todo кидать исключение! кастом CustomValidationException

            int result;
            using (var conn = (SqlConnection)_dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Northwind.Orders SET " +
                        "CustomerID = @CustomerID, " +
                        "EmployeeID = @EmployeeID, " +
                        "RequiredDate = @RequiredDate, " +
                        "ShipVia = @ShipVia, " +
                        "Freight = @Freight, " +
                        "ShipName = @ShipName, " +
                        "ShipAddress = @ShipAddress, " +
                        "ShipCity = @ShipCity, " +
                        "ShipRegion = @ShipRegion, " +
                        "ShipPostalCode = @ShipPostalCode, " +
                        "ShipCountry = @ShipCountry " +
                        "WHERE OrderID = @OrderID";
                    cmd.CommandType = CommandType.Text;


                    cmd.Parameters.AddWithValue("@OrderID", updatedOrder.OrderID);
                    cmd.Parameters.AddWithValue("@CustomerID", updatedOrder.CustomerID);
                    cmd.Parameters.AddWithValue("@EmployeeID", updatedOrder.EmployeeID);
                    cmd.Parameters.AddWithValue("@RequiredDate", updatedOrder.RequiredDate);
                    cmd.Parameters.AddWithValue("@ShipVia", updatedOrder.ShipVia);
                    cmd.Parameters.AddWithValue("@Freight", updatedOrder.Freight);
                    cmd.Parameters.AddWithValue("@ShipName", updatedOrder.ShipName);
                    cmd.Parameters.AddWithValue("@ShipAddress", updatedOrder.ShipAddress);
                    cmd.Parameters.AddWithValue("@ShipCity", updatedOrder.ShipCity);
                    cmd.Parameters.AddWithValue("@ShipRegion", updatedOrder.ShipRegion);
                    cmd.Parameters.AddWithValue("@ShipPostalCode", updatedOrder.ShipPostalCode);
                    cmd.Parameters.AddWithValue("@ShipCountry", updatedOrder.ShipCountry);

                    result = cmd.ExecuteNonQuery();
                }
            }

            if (result > 0)
            {
                return GetOrderByID(updatedOrder.OrderID);
            }
            else
                return null;
        }

        private bool HasUnchangeableFields(Order updated, Order original) // todo через ордер
        {
            if (original.State != OrderState.New)
                return true;

            if (updated.OrderDate != original.OrderDate
                || updated.ShippedDate != original.ShippedDate)
                return true;

            return false;
        }

        public bool Delete(int id)
        {
            var order = GetOrderByID(id);
            if (order != null &&
                (order.State != OrderState.New || order.State != OrderState.InProgress))
                return false;

            int result;
            using (var conn = (SqlConnection)_dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Northwind.Orders" +
                        "WHERE OrderID = @OrderID";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@OrderID", id);

                    result = cmd.ExecuteNonQuery();
                }
            }

            return result > 0;
        }

        public bool SubmitToWork(int id, DateTime orderDate)
        {
            var originalOrder = GetOrderByID(id);
            if (originalOrder.OrderDate != null)
                return false;

            int result;
            using (var conn = (SqlConnection)_dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Northwind.Orders SET " +
                        "OrderDate = @OrderDate, " +
                        "WHERE OrderID = @OrderID";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@OrderID", id);
                    cmd.Parameters.AddWithValue("@OrderDate", orderDate);

                    result = cmd.ExecuteNonQuery();
                }
            }

            return result > 0;
        }

        public bool MarkAsComplete(int id, DateTime shippedDate)
        {
            var originalOrder = GetOrderByID(id);
            if (originalOrder.ShippedDate != null)
                return false;

            int result;
            using (var conn = (SqlConnection)_dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Northwind.Orders SET " +
                        "ShippedDate = @ShippedDate, " +
                        "WHERE OrderID = @OrderID";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@OrderID", id);
                    cmd.Parameters.AddWithValue("@ShippedDate", shippedDate);

                    result = cmd.ExecuteNonQuery();
                }
            }

            return result > 0;
        }

        public IEnumerable<CustOrderHist> GetCustOrderHist(string customerID)
        {
            var statistics = new List<CustOrderHist>();
            using (var conn = (SqlConnection)_dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Northwind.CustOrderHist";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var par = cmd.CreateParameter();
                    par.ParameterName = "@CustomerID";
                    par.Value = customerID;
                    par.Direction = ParameterDirection.Input;
                    cmd.Parameters.Add(par);

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var row = new CustOrderHist()
                            {
                                ProductName = (string)dataReader[0],
                                Total = (int)dataReader[1]
                            };
                            statistics.Add(row);
                        }
                    }
                }
            }

            return statistics;
        }

        public IEnumerable<CustOrdersDetail> GetCustOrdersDetail(int orderID)
        {
            var statistics = new List<CustOrdersDetail>();
            using (var conn = (SqlConnection)_dbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Northwind.CustOrdersDetail";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var par = cmd.CreateParameter();
                    par.ParameterName = "@OrderID";
                    par.Value = orderID;
                    par.Direction = ParameterDirection.Input;
                    cmd.Parameters.Add(par);

                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var row = new CustOrdersDetail()
                            {
                                ProductName = (string)dataReader[0],
                                UnitPrice = (decimal)dataReader[1],
                                Quantity = (short)dataReader[2],
                                Discount = (int)dataReader[3],
                                ExtendedPrice = (decimal)dataReader[4]
                            };
                            statistics.Add(row);
                        }
                    }
                }
            }

            return statistics;
        }
    }
}
