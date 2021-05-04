using AspnetRun.Application.Dnu_Formating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace AspnetRun.Application.Dnu_Formating
    {
    public class WriteResponseMsg : Message
    {

        public override Packet.PacketType createRead(byte idDest, Packet.DataType type)
        {
            throw new NotImplementedException();
        }
        public override Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, byte[] hexData)
        {
            throw new NotImplementedException();
        }
        public override Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, string strData)
        {
            throw new NotImplementedException();
        }

        public WriteResponseMsg(byte[] receivedPacket)
        {
            m_packet = new Packet(receivedPacket);
        }
        public override bool IsOk()
        {
            bool startEndOK = m_packet.IsStartEnd();
            bool cmdOK = m_packet.IsCommand(Packet.PacketType.WriteResponse);
            bool chksOK = m_packet.checkCheksum();

            return startEndOK && cmdOK && chksOK;
        }


    }
}
