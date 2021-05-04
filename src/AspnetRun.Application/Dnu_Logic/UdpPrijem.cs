using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AspnetRun.Application.Dnu_Logic
{
    public delegate void MessageRecievedEventHandler(Byte[] message);

    public class UdpPrijem
    {
        private UdpClient udpClient;
        private Encoding encoding;
        private IPEndPoint endPoint;
        private Thread thread;

        /// <summary>
        /// Ovo je dogadjaj koji se podize kada ovaj UDP osluskivac primi poruku
        /// </summary>
        public event MessageRecievedEventHandler MessageRecieved;

        /// <summary>
        /// Ovo je objekat koji osluskuje zadati port na zadatoj IP adresi
        /// Kada primi poruku, salje dogadjaj o prijemu poruke ostalim objektima koji su se pretplatili na taj dogadjaj
        /// </summary>

        /// <param name="port">prijemni port</param>
        public UdpPrijem(int port)
        {
            udpClient = new UdpClient(port);
        }

        /// <summary>
        /// Pokrece osluskivac koji osluskuje na zadatom portu u posebnoj niti
        /// </summary>
        /// <param name="e"></param>
        public void StartListener(Encoding e, IPEndPoint endpoint)
        {
            encoding = e;
            endPoint = endpoint;
            thread = new Thread(new ThreadStart(startListening));
            thread.Start();
        }

        public void StartListener()
        {
            encoding = Encoding.ASCII;
            thread = new Thread(new ThreadStart(startListening));
            thread.Start();
        }

        public void StopListener()
        {
            udpClient.Close();
            //thread.Abort();
        }

        private void startListening()
        {
            if (endPoint == null)
                endPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                Byte[] receiveBytes = udpClient.Receive(ref endPoint);
                MessageRecieved(receiveBytes);
            }
        }

    }
}