using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;

namespace RestController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> PostSignal(string workflowId)
        {
            var client = await TemporalClient.ConnectAsync(new("localhost:7233"));

            var workflowHandle = client.GetWorkflowHandle(workflowId);
            // await workflowHandle.SignalAsync(wf => wf.ApproveAsync());

            return "signals sent";
        }
    }
}
