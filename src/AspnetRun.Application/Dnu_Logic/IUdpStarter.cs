namespace AspnetRun.Application.Dnu_Logic
{
    public interface IUdpStarter
    {
        bool StartListeningPorts(int numberOfPorts, int startingPort);
    }
}