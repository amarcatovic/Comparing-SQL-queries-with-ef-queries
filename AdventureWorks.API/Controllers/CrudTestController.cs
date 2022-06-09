using AdventureWorks.API.Data;
using AdventureWorks.API.Data.Models;
using AdventureWorks.API.Data.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System;
using AdventureWorks.API.Utilities;

namespace AdventureWorks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudTestController : ControllerBase
    {
        private readonly AdventureWorks2019Context _context;
        private readonly ISqlQueryExecutor _sqlQueryExecutor;

        public CrudTestController(AdventureWorks2019Context context, ISqlQueryExecutor sqlQueryExecutor)
        {
            _context = context;
            _sqlQueryExecutor = sqlQueryExecutor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var result = new TimerResponse();
            var timer = new Stopwatch();

            // EF Core Query
            timer.Start();
            var efCoreQueryTest = await _context.Addresses
                .Where(a => EF.Functions.Like(a.AddressLine1, "%street%"))
                .ToListAsync();

            timer.Stop();
            Console.WriteLine($"EF Core Query: { timer.ElapsedMilliseconds }[ms]");
            result.EntityFrameworkCoreQueryMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // EF Core View
            timer.Start();
            var efCoreViewTest = await _context.VWAddress
                .ToListAsync();
            timer.Stop();
            Console.WriteLine($"EF Core View: { timer.ElapsedMilliseconds }[ms]");
            result.SqlViewMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // SQL
            _sqlQueryExecutor.ExecuteSqlQuery(Constants.SQL_ADDRESS_GET, result);

            // SQL Procedure
            timer.Start();
            var sqlProcedureTest = await _context
                .VWAddress.FromSqlRaw("EXEC SP_AddressGet")
                .ToListAsync();
            timer.Stop();
            Console.WriteLine($"SQL Procedure { timer.ElapsedMilliseconds }[ms]");
            result.SqlProcedureMilliseconds = timer.ElapsedMilliseconds;

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddresses()
        {
            var result = new TimerResponse();
            var timer = new Stopwatch();

            // EF Core Query
            timer.Start();
            await _context.Addresses.AddAsync(new Address
            {
                AddressLine1 = "Fakultetska 1",
                AddressLine2 = null,
                City = "Zenica",
                StateProvinceId = 79,
                PostalCode = "72000",
                Rowguid = Guid.NewGuid(),
                ModifiedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

            timer.Stop();
            Console.WriteLine($"EF Core Query: { timer.ElapsedMilliseconds }[ms]");
            result.EntityFrameworkCoreQueryMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // SQL
            _sqlQueryExecutor.ExecuteSqlQuery(Constants.SQL_ADDRESS_POST, result);

            // SQL Procedure
            timer.Start();
            await _context.Database.ExecuteSqlRawAsync("EXEC SP_Address_POST");
            timer.Stop();
            Console.WriteLine($"SQL Procedure { timer.ElapsedMilliseconds }[ms]");
            result.SqlProcedureMilliseconds = timer.ElapsedMilliseconds;

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress()
        {
            var result = new TimerResponse();
            var timer = new Stopwatch();

            // EF Core Query
            timer.Start();

            var address = await _context.Addresses
                .Where(a => a.AddressLine2 == "Kreditorenbuchhaltung" && a.City == "Münster")
                .FirstOrDefaultAsync();
            address.AddressLine1 = "Roßstr 77521";
            await _context.SaveChangesAsync();

            timer.Stop();
            Console.WriteLine($"EF Core Query: { timer.ElapsedMilliseconds }[ms]");
            result.EntityFrameworkCoreQueryMilliseconds = timer.ElapsedMilliseconds;
            timer.Reset();

            // SQL
            _sqlQueryExecutor.ExecuteSqlQuery(Constants.SQL_ADDRESS_PUT, result);

            // SQL Procedure
            timer.Start();
            await _context.Database.ExecuteSqlRawAsync("EXEC SP_Address_PUT");
            timer.Stop();
            Console.WriteLine($"SQL Procedure { timer.ElapsedMilliseconds }[ms]");
            result.SqlProcedureMilliseconds = timer.ElapsedMilliseconds;

            return Ok(result);
        }
    }
}
