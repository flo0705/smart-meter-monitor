using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace SmartMeter.Monitor;

public class UdpListener : BackgroundService
{
    private readonly UdpListenerOptions _options;
    private readonly IUdpHandlerService _handlerService;
    private List<byte> _buffer;

    public UdpListener(IOptions<UdpListenerOptions> options, IUdpHandlerService handlerService)
    {
        _options = options.Value;
        _handlerService = handlerService;
        _buffer = new List<byte>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var groupEp = new IPEndPoint(IPAddress.Any, _options.Port);
        var listener = new UdpClient(groupEp);

        while (!stoppingToken.IsCancellationRequested)
        {
            var bytes = await listener.ReceiveAsync(stoppingToken);

            foreach (var bC in bytes.Buffer)
            {
                if (bC == _options.PacketTerminator)
                {
                    var _ = _handlerService.Handle(_buffer.ToArray());
                    _buffer = new List<byte>(); 
                    continue;
                }
                
                _buffer.Add(bC);
            }
        }

        listener.Close();
    }
}