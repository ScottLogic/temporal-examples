using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Temporalio.Client;
using Workflows;

namespace RestController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MicroserviceController : ControllerBase
    {
        private readonly string _clientHost;
        private readonly string _taskQueue;
        private readonly IConfiguration _configuration;

        public MicroserviceController(IConfiguration configuration)
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            var temporalConfig = _configuration.GetSection("Temporal");
            _clientHost = temporalConfig["ClientTargetHost"] ?? "host.docker.internal:7233";
            _taskQueue = temporalConfig["TaskQueue"] ?? "example";
        }

        [HttpPost("StartMicroserviceWorkflow")]
        public async Task<ActionResult<string>> StartWorkflow(string workflowId)
        {
            var client = await TemporalClient.ConnectAsync(new(_clientHost));
            // ExecuteWorkflow is not awaited so swagger UI does not wait for the workflow to finish
            var result = client.ExecuteWorkflowAsync(
                (MicroservicesWorkflow wf) => wf.RunAsync(),
                new(id: workflowId, taskQueue: _taskQueue)
            );

            return $"Workflow started with ID {workflowId}";
        }
    }
}
