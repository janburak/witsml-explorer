using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Serilog;

using Witsml;

using WitsmlExplorer.Api.Jobs;
using WitsmlExplorer.Api.Jobs.Common;
using WitsmlExplorer.Api.Services;
using WitsmlExplorer.Api.Workers;

using Xunit;

namespace WitsmlExplorer.IntegrationTests.Api.Workers
{
    [SuppressMessage("ReSharper", "xUnit1004")]
    public class CopyTrajectoryWorkerTests
    {
        private readonly CopyTrajectoryWorker _worker;

        public CopyTrajectoryWorkerTests()
        {
            var configuration = ConfigurationReader.GetConfig();
            var witsmlClientProvider = new WitsmlClientProvider(configuration);
            var loggerFactory = (ILoggerFactory)new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger);
            var logger = loggerFactory.CreateLogger<CopyTrajectoryJob>();

            _worker = new CopyTrajectoryWorker(logger, witsmlClientProvider);
        }

        [Fact(Skip = "Should only be run manually")]
        public async Task CopyTrajectory()
        {
            var job = new CopyTrajectoryJob
            {
                Source = new TrajectoryReference
                {
                    WellUid = "4d287b3e-9d9c-472a-9b82-d667d9ea1bec",
                    WellboreUid = "a2d2854b-3880-4058-876b-29b14ed7c917",
                    TrajectoryUid = "1YJFL7"
                },
                Target = new WellboreReference
                {
                    WellUid = "fa53698b-0a19-4f02-bca5-001f5c31c0ca",
                    WellboreUid = "70507fdf-4b01-4d62-a642-5f154c57440d"
                }
            };
            await _worker.Execute(job);
        }
    }
}