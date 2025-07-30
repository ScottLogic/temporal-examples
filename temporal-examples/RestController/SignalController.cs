using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;
using workflows;

namespace RestController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalController : ControllerBase
    {
        private readonly string _temporalUrl;

        public SignalController()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() // This line enables environment variable overrides
                .Build();

            _temporalUrl = config["Temporal:ClientUrl"] ?? "localhost:7233";
        }

        [HttpPost("SendSignal")]
        public async Task<ActionResult<string>> PostSignal(string workflowId, string? name)
        {
            var client = await TemporalClient.ConnectAsync(new(_temporalUrl));

            var workflowHandle = client.GetWorkflowHandle<WaitingSignalWorkflow>(workflowId);
            await workflowHandle.SignalAsync(wf => wf.Continue(name));

            return "Signal sent";
        }

        [HttpPost("StartWorkflow")]
        public async Task<ActionResult<string>> StartWorkflowWaitingSignal(string workflowId)
        {
            var client = await TemporalClient.ConnectAsync(new(_temporalUrl));
            // ExecuteWorkflow is not awaited so swagger UI does not wait for the workflow to finish
            var result = client.ExecuteWorkflowAsync(
                (WaitingSignalWorkflow wf) => wf.RunAsync(),
                new(id: workflowId, taskQueue: "example")
            );

            return $"Workflow started with ID {workflowId}";
        }
    }
}
