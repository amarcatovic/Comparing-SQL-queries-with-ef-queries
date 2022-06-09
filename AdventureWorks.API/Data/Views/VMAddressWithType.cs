using System;

namespace AdventureWorks.API.Data.Views
{
    public class VMAddressWithType
    {
        public int AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public int StateProvinceId { get; set; }
        public string PostalCode { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int AddressTypeId { get; set; }
        public string Name { get; set; }
    }
}
