using System;

namespace AdventureWorks.API.Data.BusinessObjects
{
    public class OrderDetailTest
    {
        public int SalesOrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ShipDate { get; set; }

        public string SalesOrderNumber { get; set; }

        public Guid Rowguid { get; set; }

        public decimal UnitPrice { get; set; }

        public string Name { get; set; }
    }
}
