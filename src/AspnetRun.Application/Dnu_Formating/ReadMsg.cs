using AspnetRun.Application.Dnu_DataConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRun.Application.Dnu_Formating
{
    public class ReadMsg : Message
    {
        public override Packet.PacketType createRead(byte idDest, Packet.DataType command)
        {

            IfType x = FactoryType.CreateType(command);
            byte[] command_data = x.setFieldType();
            byte[] data = x.setFieldData("0");
            //

            m_packet = new Packet(Packet.PacketType.Read, command_data, data, idDest);

            return Packet.PacketType.Read;
        }

        public override Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, byte[] hexData)
        {
            throw new NotImplementedException();
        }

        public override Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, string strData)
        {
            throw new NotImplementedException();
        }

        public override bool IsOk()
        {
            bool startEndOK = m_packet.IsStartEnd();
           
            bool chksOK = m_packet.checkCheksum();

            return startEndOK && chksOK;
        }
    }
}
