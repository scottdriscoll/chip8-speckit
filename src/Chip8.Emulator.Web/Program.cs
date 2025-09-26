using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Chip8.Emulator.Web;
using Chip8.Emulator.Core.Diagnostics;
using Chip8.Emulator.Core.Input;
using Chip8.Emulator.Core.Timing;
using Chip8.Emulator.Audio;
using Chip8.Emulator.Platform.Web;
using Chip8.Emulator.Core.Machine;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<EmulatorSnapshotPublisher>();
builder.Services.AddScoped<InputMapper>();
builder.Services.AddScoped<TimerService>();
builder.Services.AddScoped<IWebAudioInterop, RuntimeHost>();
builder.Services.AddScoped<WebAudioBeeper>();
builder.Services.AddScoped<RuntimeHost>();
builder.Services.AddScoped<Chip8Machine>();

var host = builder.Build();

var snapshotPublisher = host.Services.GetRequiredService<EmulatorSnapshotPublisher>();
var timerService = host.Services.GetRequiredService<TimerService>();
var beeper = host.Services.GetRequiredService<WebAudioBeeper>();
var runtimeHost = host.Services.GetRequiredService<RuntimeHost>();
var machine = host.Services.GetRequiredService<Chip8Machine>();

beeper.Attach(timerService);
machine.LoadDemoProgram();
snapshotPublisher.PublishSnapshot();

await host.RunAsync();
