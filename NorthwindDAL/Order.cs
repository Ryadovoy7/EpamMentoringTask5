using System;
using System.Collections.Generic;
using System.Data.Common;

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
        private string _customerID;
        private int? _employeeID;
        private DateTime? _requiredDate;
        private int? _shipVia;
        private decimal? _freight;
        private string _shipName;
        private string _shipAddress;
        private string _shipCity;
        private string _shipRegion;
        private string _shipPostalCode;
        private string _shipCountry;

        public int OrderID { get; }

        public string CustomerID { get => _customerID; set => _customerID = ValidatePropertyValue(value); }

        public int? EmployeeID { get => _employeeID; set => _employeeID = ValidatePropertyValue(value); }

        public DateTime? OrderDate { get; }

        public DateTime? RequiredDate { get => _requiredDate; set => _requiredDate = ValidatePropertyValue(value); }

        public DateTime? ShippedDate { get; }

        public int? ShipVia { get => _shipVia; set => _shipVia = ValidatePropertyValue(value); }

        public decimal? Freight { get => _freight; set => _freight = ValidatePropertyValue(value); }

        public string ShipName { get => _shipName; set => _shipName = ValidatePropertyValue(value); }

        public string ShipAddress { get => _shipAddress; set => _shipAddress = ValidatePropertyValue(value); }

        public string ShipCity { get => _shipCity; set => _shipCity = ValidatePropertyValue(value); }

        public string ShipRegion { get => _shipRegion; set => _shipRegion = ValidatePropertyValue(value); }

        public string ShipPostalCode { get => _shipPostalCode; set => _shipPostalCode = ValidatePropertyValue(value); }

        public string ShipCountry { get => _shipCountry; set => _shipCountry = ValidatePropertyValue(value); }

        public OrderState State { 
            get 
            {
                if (ShippedDate != null)
                    return OrderState.Completed;
                else if (OrderDate != null)
                    return OrderState.InProgress;
                else
                    return OrderState.New;
            }
        }

        public List<OrderDetail> Details { get; set; }

        public Order()
        { }

        public Order(
            int orderID,
            string customerID,
            int? employeeID,
            DateTime? orderDate,
            DateTime? requiredDate,
            DateTime? shippedDate,
            int? shipVia,
            decimal? freight,
            string shipName,
            string shipAddress,
            string shipCity,
            string shipRegion,
            string shipPostalCode,
            string shipCountry)
        {
            OrderID = orderID;
            _customerID = customerID;
            _employeeID = employeeID;   
            OrderDate = orderDate;
            _requiredDate = requiredDate;
            ShippedDate = shippedDate;
            _shipVia = shipVia;
            _freight = freight;
            _shipName = shipName;
            _shipAddress = shipAddress;
            _shipCity = shipCity;
            _shipRegion = shipRegion;
            _shipPostalCode = shipPostalCode;
            _shipCountry = shipCountry;
        }

        private T ValidatePropertyValue<T>(T value)
        {
            if (State == OrderState.New)
                return value;
            else
                throw new InvalidOrderStateException("Order state must be new to change this property!");
        }
    }
}