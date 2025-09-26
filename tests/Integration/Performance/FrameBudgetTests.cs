using System.Threading.Tasks;
using Xunit;

namespace Chip8.Emulator.IntegrationTests.Performance;

public class FrameBudgetTests
{
    [Fact(Skip = "Awaiting HUD performance instrumentation.")]
    public async Task Hud_rendering_should_complete_within_budget()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "Awaiting HUD performance instrumentation.")]
    public async Task Cpu_usage_should_remain_under_threshold()
    {
        await Task.CompletedTask;
    }
}
