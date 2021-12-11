using System;
using System.Collections.Generic;

namespace NorthwindDAL
{
    public enum OrderState
    {
        New,
        InProgress,
        Completed
    }

    public class Order
    {
        public int OrderID { get; set; }

#nullable enable
        public string? CustomerID { get; set; }

        public int? EmployeeID { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; } 

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        public decimal? Freight { get; set; }

        public string? ShipName { get; set; }

        public string? ShipAddress { get; set; }

        public string? ShipCity { get; set; }

        public string? ShipRegion { get; set; }

        public string? ShipPostalCode { get; set; }

        public string? ShipCountry { get; set; }
#nullable disable
        public OrderState State { get; set; } 

        public List<OrderDetail> Details { get; set; }

        // todo сделать на уровне свойства неизменяемые поля
    }
}