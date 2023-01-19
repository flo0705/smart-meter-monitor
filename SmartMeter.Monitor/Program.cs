using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartMeter.Monitor;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<UdpListener>()
            .AddTransient<IUdpHandlerService, UdpHandlerService>()
            .AddOptions<UdpListenerOptions>().Configure(p =>
            {
                p.Port = 11000;
                p.PacketTerminator = '#';
            });
    })
    .ConfigureLogging((context, builder) =>
        builder
            .ClearProviders()
            .AddDebug())
    .Build();
host.Run();

public class UdpHandlerService : IUdpHandlerService
{
    private readonly SmartMeterView _view = new ();
    public Task Handle(byte[] bytes)
    {
        // +P:1123|-P:123|+A:99|-A:12|-R:123.123|+R:123.44|V L1:223|V L2:224|V L3:223|A L1:10|A L2:20|A L3:30|PF:1000#
        var decodedString = Encoding.Default.GetString(bytes);

        var values = decodedString
            .Trim()
            .Split("|")
            .Select(v => v.Split(":"))
            .ToList();

        var data = new SmartMeterData
        {
            ActivePowerPlusInWatt = float.Parse(values.FirstOrDefault(p => p[0] == "+P")?[1] ?? "0"),
            ActivePowerMinusInWatt = float.Parse(values.FirstOrDefault(p => p[0] == "-P")?[1] ?? "0"),
            ActiveEnergyPlusInAmpere = float.Parse(values.FirstOrDefault(p => p[0] == "+A")?[1] ?? "0"),
            ActiveEnergyMinusInAmpere = float.Parse(values.FirstOrDefault(p => p[0] == "-A")?[1] ?? "0"),
            ReactiveEnergyPlusInWattHours = float.Parse(values.FirstOrDefault(p => p[0] == "+R")?[1] ?? "0"),
            ReactiveEnergyMinusInWattHours = float.Parse(values.FirstOrDefault(p => p[0] == "-R")?[1] ?? "0"),
            VoltageV1 = float.Parse(values.FirstOrDefault(p => p[0] == "V L1")?[1] ?? "0"),
            VoltageV2 = float.Parse(values.FirstOrDefault(p => p[0] == "V L2")?[1] ?? "0"),
            VoltageV3 = float.Parse(values.FirstOrDefault(p => p[0] == "V L3")?[1] ?? "0"),
            CurrentA1 = float.Parse(values.FirstOrDefault(p => p[0] == "A L1")?[1] ?? "0"),
            CurrentA2 = float.Parse(values.FirstOrDefault(p => p[0] == "A L2")?[1] ?? "0"),
            CurrentA3 = float.Parse(values.FirstOrDefault(p => p[0] == "A L3")?[1] ?? "0"),
            PowerFactor = int.Parse(values.FirstOrDefault(p => p[0] == "PF")?[1] ?? "0"),
            TimeStamp = DateTime.Now,
        };

        _view.Render(data);
        return Task.CompletedTask;
    }
}