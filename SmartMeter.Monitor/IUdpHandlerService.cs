namespace SmartMeter.Monitor;

public interface IUdpHandlerService
{
    public Task Handle(byte[] bytes);
}