using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Temporalio.Client;
using Workflows;

namespace RestController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalController : ControllerBase
    {
        private readonly string _clientHost;
        private readonly string _taskQueue;
        private readonly IConfiguration _configuration;

        public SignalController(IConfiguration configuration)
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            var temporalConfig = _configuration.GetSection("Temporal");
            _clientHost = temporalConfig["ClientTargetHost"] ?? "host.docker.internal:7233";
            _taskQueue = temporalConfig["TaskQueue"] ?? "example";
        }

        [HttpPost("SendSignal")]
        public async Task<ActionResult<string>> PostSignal(string workflowId, string? name)
        {
            var client = await TemporalClient.ConnectAsync(new(_clientHost));

            var workflowHandle = client.GetWorkflowHandle<WaitingSignalWorkflow>(workflowId);
            await workflowHandle.SignalAsync(wf => wf.Continue(name));

            return "Signal sent";
        }

        [HttpPost("StartWorkflow")]
        public async Task<ActionResult<string>> StartWorkflowWaitingSignal(string workflowId)
        {
            var client = await TemporalClient.ConnectAsync(new(_clientHost));
            // ExecuteWorkflow is not awaited so swagger UI does not wait for the workflow to finish
            var result = client.ExecuteWorkflowAsync(
                (WaitingSignalWorkflow wf) => wf.RunAsync(),
                new(id: workflowId, taskQueue: _taskQueue)
            );

            return $"Workflow started with ID {workflowId}";
        }
    }
}
