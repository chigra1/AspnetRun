
using AspnetRun.Application.Dnu_DataConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRun.Application.Dnu_Formating
{
    public abstract class Message
    {
        protected Packet m_packet = null;

        abstract public bool IsOk();
        abstract public Packet.PacketType createRead(byte idDest, Packet.DataType type);
        abstract public Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, byte[] hexData);
        abstract public Packet.PacketType createWrite(byte[] hexAddress, Packet.DataType type, string strData);

        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Convert message structure to string
        /// </summary>
        /// <returns></returns>
        /*     override public string ToString()
             {
                 // return exact field values, not ascii
                 return m_packet.binaryToString();
             }
        */



        /// <summary>
        /// Convert message to bytes - ready to send ascii data
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return m_packet.getBytes();
        }


        public byte[] GetAck()
        {
            return m_packet.getAcknowledge();
        }
        public bool IsAcknowledged()
        {
            byte[] ack = m_packet.getAcknowledge();


            return (ack[0] == (byte)(Packet.PacketVerification.Ack));
        }

        public byte[] GetData()
        {
            
            byte[] data = m_packet.getData();
            
           // if (ascii_data == null)
               // logger.Fatal("No Data for this packet!");

            return data;
        }

        public string GetRealDataToString()
        {
            IfType x = FactoryType.CreateType(GetDataType());//(m_packet.getDataType());
            string res = x.ToStringRealData(m_packet.getData());
            return res;
        }

        public object GetRealData()
        {
            IfType x = FactoryType.CreateType(GetDataType());//(m_packet.getDataType());
            object res = x.getRealData(m_packet.getData());
            return res;

        }



        public Packet.DataType GetDataType()
        {
            // calculate from packet.GetLength();
            Packet.DataType type = (Packet.DataType) m_packet.getDataType();
            
           // byte[] result = Packet.AsciiByteToHexNibble(ascii_type);
            //int res = BitConverter.ToInt16(ascii_type, 0);
            return type;

        }

        // <summary>
        // check length of date with length
        // </summary>
        public bool checkLengthOfData()
        {
            byte[] data = GetData();

            IfType x = FactoryType.CreateType((Packet.DataType)GetDataType());
            int type_len = x.getFieldLength();
            return type_len == data.Length;
        }

        public bool checkPattern(Packet.DataType type, string text)
        {
            IfType x = FactoryType.CreateType(type);
            return x.checkStringPattern(text);
        }

        public bool checkMinMax(Packet.DataType type, string input, string min, string max)
        {
            IfType x = FactoryType.CreateType(type);
            return x.checkMinMax(input,min,max);
        }
        public async Task<bool> SaveRealDataToDbAsync(string adress)
        {
            Packet.DataType type = GetDataType();
            IfType x = FactoryType.CreateType(type);
            return await x.SaveToDbAsync(m_packet.getData(), "127.0.0.1");
            
        }
    }
}
