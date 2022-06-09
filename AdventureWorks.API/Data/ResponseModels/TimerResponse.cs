namespace AdventureWorks.API.Data.ResponseModels
{
    public class TimerResponse
    {
        public long SqlProcedureMilliseconds { get; set; }

        public long SqlQueryWithMappingMilliseconds { get; set; }

        public long SqlViewMilliseconds { get; set; }

        public long EntityFrameworkCoreQueryMilliseconds { get; set; }
    }
}
