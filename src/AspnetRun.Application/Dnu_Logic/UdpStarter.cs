
using AspnetRun.Application.Dnu_Formating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRun.Application.Dnu_Logic
{
    public class UdpSender
    {


        //private readonly IRecurringJobManager _recurringJobManager;
        //private readonly IBackgroundJobClient _backgroundJobClient;

        //public UdpSender(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        //{
        //    _backgroundJobClient = backgroundJobClient;
        //    _recurringJobManager = recurringJobManager;
        //}

        public void StartSend15()
        {
          //  RecurringJob.AddOrUpdate(() => SendInvoiceMail(userName), Cron.Monthly);

        }
    }
    public class UdpStarter : IUdpStarter
    {
        private readonly IUdpReceiver _udpReceiver;
        public UdpStarter(IUdpReceiver udpReceiver)
        {
            _udpReceiver = udpReceiver;
        }
        public bool StartListeningPorts(int numberOfPorts, int startingPort)
        {
            Console.WriteLine($"Openning {numberOfPorts} ports ******************");

            

            int port_offset = startingPort;


            UdpReceiver[] rec_port = new UdpReceiver[numberOfPorts];
            for (int i = 0; i < numberOfPorts; i++)
            {
                _udpReceiver.ReceiveMessages(port_offset + i);
            } 
            
            
            return true;
        }


        public bool Send15 ()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 51288);
            UdpClient udpClient = new UdpClient();
           // Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there");

            //example to send req. for Merenja
            Message snd = new ReadMsg();
            Packet.PacketType next_message = snd.createRead(0x01, Packet.DataType.Merenja);
            byte[] sendBytes = snd.GetBytes();
            //snd.GetRealData();

            try
            {
                udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return true;
        }
    }
}
