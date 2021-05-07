
using AspnetRun.Application.Dnu_Formating;
using AspnetRun.Application.Interfaces;
using AspnetRun.Application.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AspnetRun.Application.Dnu_Logic
{
   public class UdpReceiver : IUdpReceiver
    {

        public struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }

        public bool messageReceived = false;
        public IPEndPoint e;
        public UdpClient u;
        public UdpState s;
        int s_listenPort;

        private readonly IProductService _productService;
        public UdpReceiver(IProductService productService)
        {
            _productService = productService;
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            

            Message rcv = new ReadResponseMsg(receiveBytes);
            if (DataReceivedOK(rcv))
            {
                byte[] dataBytes = rcv.GetData();
                string rec = rcv.GetRealDataToString();

                string receiveString = BitConverter.ToString(receiveBytes);
                string receiveData = BitConverter.ToString(dataBytes);

                Console.WriteLine($"\nReceived: {receiveString} on:{s_listenPort} from {e.Address} : {e.Port} on thread {Thread.CurrentThread.ManagedThreadId}");

                Console.WriteLine($"\nData: {receiveData} ");
                Console.WriteLine($"\nDataString:\n {rec} ");

                messageReceived = true;

                var product =  _productService.GetProductById(1);
                product.Wait();
                //product.UnitPrice = 101;
                //await _productService.Update(product);

                //rcv.SaveRealDataToDbAsync("127.0.0.1");
                /*
                Thread.Sleep(5000);

                //example to send req. for Merenja
                Message snd = new ReadMsg();
                Packet.PacketType next_message = snd.createRead(0x01, Packet.DataType.Merenja);
                byte[] sendBytes = snd.GetBytes();

                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 64329);
                u.Send(sendBytes, sendBytes.Length, ipEndPoint);
                */
            }
            u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }

        public void ReceiveMessages(int listenPort)
        {
            s_listenPort = listenPort;
            e = new IPEndPoint(IPAddress.Any, s_listenPort);
            u = new UdpClient(e);

            s = new UdpState();
            s.e = e;
            s.u = u;

            Console.WriteLine($"listening for messages on port {s_listenPort}\n");
            u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

            // Do some work while we wait for a message. For this example, we'll just sleep
            //while (!messageReceived)
            //{
            //    Thread.Sleep(10000);
            //}
        }

        bool DataReceivedOK(Message rcv)
        {
            bool messageOK = false;


            if (rcv.IsOk())
            {
                //bool addr_ok = snd.GetAddress().SequenceEqual(rcv.GetAddress());
                //bool type_ok = snd.GetDataType() == rcv.GetDataType();

                //if (addr_ok && type_ok)
                //{
                    messageOK = true;
                //}

            }
            else
            {
                messageOK = false;
                //MessageBox.Show("RX Packet incorrect!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return messageOK;
        }
    }
}
