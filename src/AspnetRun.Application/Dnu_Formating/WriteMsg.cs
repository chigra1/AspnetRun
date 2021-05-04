
using AspnetRun.Application.Dnu_DataConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRun.Application.Dnu_Formating
{
    public class WriteMsg : Message
    {
        public override Packet.PacketType createRead(byte idDest, Packet.DataType type)
        {
            throw new NotImplementedException();
        }

        public override Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, byte[] hexData)
        {
           // byte[] addr = Packet.HexNiblleToAsciiByte(hexAddress);

            //byte[] data = Packet.HexNiblleToAsciiByte(hexData);

            IfType x = FactoryType.CreateType(type);
            byte[] type_ascii = x.setFieldType();

           // m_packet = new Packet(Packet.PacketType.Read, Packet.DataType.Merenja, data, 0x01);

            return Packet.PacketType.Write;
        }

        public override Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, string strData)
        {
            //byte[] addr = Packet.HexNiblleToAsciiByte(hexAddress);
            int lenRaw = strData.Length / 2;
            IfType x = FactoryType.CreateType(type);
            byte[] type_ascii = x.setFieldType();
            byte[] data_ascii = x.setFieldData(strData);
            //m_packet = new Packet(Packet.PacketType.Read, Packet.DataType.Merenja, data_ascii, 0x01);
            return Packet.PacketType.Write;
        }

        public override bool IsOk()
        {
            bool lenOK = checkLengthOfData();
            bool startEndOK = m_packet.IsStartEnd();
            bool cmdOK = m_packet.IsCommand(Packet.PacketType.Write);
            bool chksOK = m_packet.checkCheksum();

            return startEndOK && cmdOK && chksOK && lenOK;
        }
    }
}
