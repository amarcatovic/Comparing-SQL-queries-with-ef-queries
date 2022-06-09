using AdventureWorks.API.Data.ResponseModels;
using Microsoft.Data.SqlClient;
using System;
using System.Diagnostics;

namespace AdventureWorks.API.Data
{
    public interface ISqlQueryExecutor
    {
        /// <summary>
        /// Executes forwarded SQL query
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Milliseconds</returns>
        void ExecuteSqlQuery(string query, TimerResponse timerResponse);
    }

    public class SqlQueryExecutor : ISqlQueryExecutor
    {
        private SqlConnection _connection = null;
        Stopwatch _timer = null;

        public SqlQueryExecutor()
        {
            _connection = new SqlConnection("Data Source=localhost;Initial Catalog=AdventureWorks2019;Integrated Security=True;TrustServerCertificate=True;");
            _timer = new Stopwatch();
        }

        public void ExecuteSqlQuery(string query, TimerResponse timerResponse)
        {
            SqlCommand command = new SqlCommand(query, _connection);
            SqlDataReader sqlDataReader;
            command.Connection.Open();

            _timer.Start();
            sqlDataReader = command.ExecuteReader();
            if (!sqlDataReader.HasRows)
            {
                Console.WriteLine("No rows!");
            }
            while (sqlDataReader.Read())
            {
                Console.WriteLine(sqlDataReader.ToString()); // ekvivalent kao da sva polja mapiramo
            }
            timerResponse.SqlQueryWithMappingMilliseconds = _timer.ElapsedMilliseconds;
            _timer.Stop();
            _timer.Reset();
            _connection.Close();
        }
    }
}
