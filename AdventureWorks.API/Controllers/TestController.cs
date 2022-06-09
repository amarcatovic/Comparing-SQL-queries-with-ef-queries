using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorks.API.Data.BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdventureWorks.API.Data.Models;
using AdventureWorks.API.Data.ResponseModels;
using AdventureWorks.API.Data;
using AdventureWorks.API.Utilities;

namespace AdventureWorks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AdventureWorks2019Context _context;
        private readonly ISqlQueryExecutor _sqlQueryExecutor;

        public TestController(AdventureWorks2019Context context, ISqlQueryExecutor sqlQueryExecutor)
        {
            _context = context;
            _sqlQueryExecutor = sqlQueryExecutor;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimerResponse))]
        public async Task<IActionResult> GetPurchaseOrderHeaders()
        {
            var result = new TimerResponse();
            var timer = new Stopwatch();

            // EF Core Query
            timer.Start();
            var efCoreQueryTest = await _context.SalesOrderDetails
                .Include(sod => sod.SalesOrder)
                .ThenInclude(soh => soh.Territory)
                .Where(sod => sod.SalesOrder.DueDate.Year >= 2012 && sod.SalesOrder.DueDate.Year <= 2014)
                .Select(sod => 
                    new OrderDetailTest
                    {
                        SalesOrderID = sod.SalesOrder.SalesOrderId,
                        OrderDate = sod.SalesOrder.OrderDate,
                        DueDate = sod.SalesOrder.DueDate,
                        ShipDate = sod.SalesOrder.ShipDate,
                        SalesOrderNumber = sod.SalesOrder.SalesOrderNumber,
                        Rowguid = sod.Rowguid,
                        UnitPrice = sod.UnitPrice,
                        Name = sod.SalesOrder.Territory.Name
                    })
                .OrderByDescending(sod => sod.DueDate)
                .AsNoTracking()
                .ToListAsync();

            timer.Stop();
            Console.WriteLine($"EF Core Query: { timer.ElapsedMilliseconds }[ms]");
            result.EntityFrameworkCoreQueryMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // EF Core View
            timer.Start();
            var efCoreViewTest = await _context.VWTest
                .OrderByDescending(x => x.DueDate)
                .ToListAsync();
            timer.Stop();
            Console.WriteLine($"EF Core View: { timer.ElapsedMilliseconds }[ms]");
            result.SqlViewMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            _sqlQueryExecutor.ExecuteSqlQuery(Constants.SP_TEST_SQL, result);

            // SQL Procedure
            timer.Start();
            var sqlProcedureTest = await _context
                .VWTest.FromSqlRaw("EXEC SP_Test")
                .ToListAsync();
            timer.Stop();
            Console.WriteLine($"SQL Procedure { timer.ElapsedMilliseconds }[ms]");
            result.SqlProcedureMilliseconds = timer.ElapsedMilliseconds;

            return Ok(result);
        }

        [HttpGet("/addresses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimerResponse))]
        public async Task<IActionResult> GetAddressesWithTypes()
        {
            var result = new TimerResponse();
            var timer = new Stopwatch();

            // EF Core Query
            timer.Start();
            var efCoreQueryTest = await _context.Addresses
                .Include(a => a.BusinessEntityAddresses)
                .ThenInclude(bea => bea.AddressType)
                .Distinct()
                .Select(a => new AddressWithTypeTest
                {
                    AddressId = a.AddressId,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    City = a.City,
                    StateProvinceId = a.StateProvinceId,
                    PostalCode = a.PostalCode,
                    Rowguid = a.Rowguid,
                    ModifiedDate = a.ModifiedDate,
                    AddressTypeId = a.BusinessEntityAddresses.FirstOrDefault(bea => bea.AddressId == a.AddressId).AddressType.AddressTypeId,
                    Name = a.BusinessEntityAddresses.FirstOrDefault(bea => bea.AddressId == a.AddressId).AddressType.Name
                })
                .ToListAsync();

            timer.Stop();
            Console.WriteLine($"EF Core Query: { timer.ElapsedMilliseconds }[ms]");
            result.EntityFrameworkCoreQueryMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // SQL query
            _sqlQueryExecutor.ExecuteSqlQuery(Constants.SQL_ADDRESS_JOIN_ADDRESS_TYPE, result);

            // SQL Procedure
            timer.Start();
            var sqlProcedureTest = await _context
                .VMAddressWithType.FromSqlRaw("EXEC SP_Address_With_Type")
                .ToListAsync();
            timer.Stop();
            Console.WriteLine($"SQL Procedure { timer.ElapsedMilliseconds }[ms]");
            result.SqlProcedureMilliseconds = timer.ElapsedMilliseconds;

            return Ok(result);
        }

        [HttpGet("/slower_query")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimerResponse))]
        public async Task<IActionResult> SlowerQuery()
        {
            var result = new TimerResponse();
            var timer = new Stopwatch();

            // EF Core Query
            timer.Start();
            var efCoreQueryTest = await (from pod in _context.PurchaseOrderDetails
                                   join sod in _context.SalesOrderDetails on pod.ProductId equals sod.ProductId
                                   select new
                                   {
                                       OrderQty = pod.OrderQty,
                                       UnitPrice = pod.UnitPrice,
                                       TrackingNumber = sod.CarrierTrackingNumber
                                   })
                                   .ToListAsync();

            timer.Stop();
            Console.WriteLine($"EF Core Query: { timer.ElapsedMilliseconds }[ms]");
            result.EntityFrameworkCoreQueryMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // EF Core View
            timer.Start();
            var efCoreViewTest = await _context.VWPodSop
                .ToListAsync();
            timer.Stop();
            Console.WriteLine($"EF Core View: { timer.ElapsedMilliseconds }[ms]");
            result.SqlViewMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // SQL query
            _sqlQueryExecutor.ExecuteSqlQuery(Constants.SQL_PURCHASEORDERDETAILANDSALESORDERDETAIL, result);

            // SQL Procedure
            timer.Start();
            var sqlProcedureTest = await _context
                .VWPodSop.FromSqlRaw("EXEC SP_POD_SOP")
                .ToListAsync();
            timer.Stop();
            Console.WriteLine($"SQL Procedure { timer.ElapsedMilliseconds }[ms]");
            result.SqlProcedureMilliseconds = timer.ElapsedMilliseconds;

            return Ok(result);
        }
    }
}
