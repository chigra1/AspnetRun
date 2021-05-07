using System;

namespace AspnetRun.Application.Dnu_Logic
{
    public interface IUdpReceiver
    {
        void ReceiveCallback(IAsyncResult ar);
        void ReceiveMessages(int listenPort);
    }
}