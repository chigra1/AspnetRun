using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspnetRun.Application.Dnu_Formating
{
    /*
     * 
     * 
       CCh	CCh	CCh	ADRdst	ADRsrc	CMD	DATAlenH	DATAlenL	DATA0	..	DATAlen-1	CS

        Preambula: 3 bajta 0xCC
        ADRdst                          – 1 bajt; Kod uređaja kom je upućena poruka
        ADRsrc                          – 1 bajt; Kod uređaja koji šalje poruku
        CMD                              – 1 bajt; kod poruke


        DATAlenH,DATAlenL – 2 bajta dužine poruke(DATAlenH cemo da koristimo za visi bajt adrese DNU-a, adresa centra ostaje 0xFF????????)


        DATA0-DATAlen-1     – korisni podaci, ’DATAlen’ bajtova 
        CS                                  – 1 bajt;  kontrolna suma

        ADRdst i ADRsrc mogu biti u opsegu 0-255. Rezervisana vrednost je adresa PC-ja 0xFF.

        CS – Kontrolna suma se računa po sledećem algoritmu

        CS=  ~(ADRdst+ADRsrc+CMD+DATAlenH+DATAlenL+DATA0+....+DATAlen-1)

        CMD – kod poruke
    */
    public class Packet
    {
        public enum DataType { RAW = 0, Merenja = 0xE0 };

        public enum PacketFields { Start1 = 0xCC, Start2 = 0xCC, Start3 = 0xCC };
        public enum PacketVerification { Ack = 0x41, Nack = 0x4E };
        public enum PacketType { Read = 0x52, ReadResponse = 0x58, Write = 0x57, WriteResponse = 0x59 };

        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private byte[] start = new byte[3];
        private byte[] idSrc = new byte[1] { 0xFF };
        private byte[] idDest = new byte[1] {  0x01 };
        private byte[] command = new byte[1];
        private byte[] len = new byte[2];

        private byte[] data = null;
        private byte[] acknowledge = null;
        private byte[] checksum = new byte[1];

        //private byte[] type = new byte[4];

        public Packet(PacketType type, DataType dataCommand, byte idDest,  byte[] data)//crate Write const
        {
            start = new byte[3] { (byte)PacketFields.Start1, (byte)PacketFields.Start2, (byte)PacketFields.Start3};

            //id
            this.idDest[0] = idDest;
            //command
            command[0] = (byte)dataCommand;

            if (command[0] == (byte)PacketType.Write)
            {
                data = new byte[data.Length];
                Array.Copy(data, 0, data, 0, data.Length);
            }
            else
            {
                //logger.Fatal("Wrong PacketType: 0x" + BitConverter.ToString(command));
                throw new NotImplementedException();
            }

            //checksum
            insertChecksum();
            //end
            //end[0] = (byte)PacketFields.EndCR;
            //end[1] = (byte)PacketFields.EndLF;

            byte[] packet_tx = getBytes();
            //logger.Debug("Created TX packet: " + BitConverter.ToString(packet_tx) + " ASCII: " + Encoding.ASCII.GetString(packet_tx, 0, packet_tx.Length - 2));
        }
        public Packet(PacketType packetType, byte[] dataCommand, byte[]data, byte idDest)//create Read const
        {
            start = new byte[3] { (byte)PacketFields.Start1, (byte)PacketFields.Start2, (byte)PacketFields.Start3 };

            //id
            this.idDest[0] = idDest;
            //command
            command[0] = dataCommand[0];




            //length
            if (packetType == PacketType.Read)
            {
                this.data = new byte[data.Length];
                Array.Copy(data, 0, this.data, 0, data.Length);
                len[1] = (byte)(data.Length);
            }
            else
            {
                //logger.Fatal("Wrong PacketType: 0x" + BitConverter.ToString(command));
                throw new NotImplementedException();
            }

            //checksum
            insertChecksum();


            byte[] packet_tx = getBytes();
            //logger.Debug("Created TX packet: " + BitConverter.ToString(packet_tx) + " ASCII: " + Encoding.ASCII.GetString(packet_tx, 0, packet_tx.Length - 2));
        }



        /// <summary>
        /// Ascii bytes
        /// </summary>
        /// <param name="packet"></param>
        public Packet(byte[] packet)
        {
            // start
            Array.Copy(packet, 0, start, 0, start.Length);
            // id
            Array.Copy(packet, start.Length, idDest, 0, idDest.Length);
            // id
            Array.Copy(packet, start.Length+ idDest.Length, idSrc, 0, idSrc.Length);
            // command
            Array.Copy(packet, start.Length + idDest.Length + idSrc.Length, command, 0, command.Length);

            // length
            Array.Copy(packet, start.Length + idDest.Length + idSrc.Length+ command.Length, len, 0, len.Length);
            // checksum
            Array.Copy(packet, packet.Length - checksum.Length, checksum, 0, checksum.Length);


            //...if ReadResponse -> extractData
            //...if WriteResponse -> extractAcknowledge
            //if (command[0] == (byte)PacketType.ReadResponse)
                extractData(packet);
           // else if (command[0] == (byte)PacketType.WriteResponse)//
            //    extractAcknowledge(packet);
            //else
            //{
                //logger.Fatal("Wrong PacketType: 0x" + BitConverter.ToString(command));
            //}
            //logger.Debug("Created RX packet: " + BitConverter.ToString(packet) + " ASCII: " + Encoding.ASCII.GetString(packet, 0, packet.Length - 2));
        }

        /// <summary>
        /// Extract data from LEN(start + command + address + length) to LEN(packet) - LEN(checksum + end)
        /// </summary>
        /// <param name="Packet"></param>
        public void extractData(byte[] packet)
        {
            
            int temp_DataLength = packet.Length - start.Length - idDest.Length - idSrc.Length - command.Length - len.Length  - checksum.Length;
            data = new byte[temp_DataLength];
            Array.Copy(packet, start.Length + idDest.Length + idSrc.Length + command.Length + len.Length, data, 0, temp_DataLength);
       
        }
        /// <summary>
        /// Extract data from LEN(start + command + address + length) to LEN(packet) - LEN(checksum + end)
        /// </summary>
        public void extractAcknowledge(byte[] Packet)
        {
            /*
            int temp_AckLength = Packet.Length - start.Length - id.Length - command.Length - address.Length - type.Length - checksum.Length - end.Length;
            acknowledge = new byte[temp_AckLength];
            Array.Copy(Packet, start.Length + id.Length + command.Length + address.Length + type.Length, acknowledge, 0, temp_AckLength);
        */
            }

        public bool IsCommand(PacketType packetType)
        {
            return (command[0] == (byte)packetType);
        }
        public bool IsStartEnd()
        {
            
            //return (start[0] == (byte)PacketFields.Start && end[0] == (byte)PacketFields.EndCR && end[1] == (byte)PacketFields.EndLF);
            return true;
        }

        /// <summary>
        /// calculate checsum for all fields
        /// </summary>
        /// <returns></returns>
        public byte calcChecksum()
        {

            byte calculatedChksum;
            int temp_Chksum = 0;


            //idSrc
            for (int i = 0; i < idSrc.Length; i++)
            {
                temp_Chksum += idSrc[i];
            }
            //idDest
            for (int i = 0; i < idDest.Length; i++)
            {
                temp_Chksum += idDest[i];
            }
            //command
            for (int i = 0; i < command.Length; i++)
            {
                temp_Chksum += command[i];
            }
            //length
            for (int i = 0; i < len.Length; i++)
            {
                temp_Chksum += len[i];
            }
            //data
             for (int i = 0; i < data.Length; i++)
             {
                temp_Chksum += data[i];
             }
            

            byte[] intBytes = BitConverter.GetBytes((short)temp_Chksum);
            //calculatedChksum = Packet.HexNiblleToAsciiByte(intBytes);
            calculatedChksum = (byte)(0x00 + intBytes[0]);
            //logger.Debug("Calculated checksum: 0x" + temp_Chksum.ToString("X" + checksum.Length) + " ASCII bytes: " + BitConverter.ToString(calculatedChksum));


            return calculatedChksum;
        }

        /// <summary>
        /// insert calculated checksum
        /// </summary>
        public void insertChecksum()
        {
            byte calculated = calcChecksum();
            //Array.Copy(calculated, checksum, checksum.Length);
            checksum[0] = calculated;
        }
        /// <summary>
        /// check if checksum is same as calculated checksum
        /// </summary>
        /// <returns></returns>
        public bool checkCheksum()
        {
            bool res = true;
            byte calculated = calcChecksum();

                if (calculated != checksum[0])
                {
                    res = false;

                }

            return res;
        }

        /// <summary>
        /// calculate length
        /// </summary>
        /// <returns></returns>
        public byte[] calcLength()
        {
            byte[] intBytes = BitConverter.GetBytes(data.Length);
            
            return intBytes;
        }


        ///// <summary>
        ///// calculate and insert length
        ///// </summary>
        //public void insertLength()
        //{
        //    byte[] calcLen = calcLength();
        //    Array.Copy(calcLen, type, type.Length);
        //}

        ///// <summary>
        ///// calculate and check length
        ///// </summary>
        //public bool checkLength()
        //{
        //    bool res = true;
        //    byte[] calculated = calcLength();
        //    for (int i = 0; i < type.Length; i++)
        //    {
        //        if (calculated[i] != type[i])
        //        {
        //            res = false;
        //            break;
        //        }
        //    }
        //    return res;
        //}

        /// <summary>
        /// put all fields in ascii byte array
        /// </summary>
        /// <returns></returns>
        public byte[] getBytes()
        {
            byte[] sendBytes;
            sendBytes = start.Concat(idSrc).Concat(idDest).Concat(command).Concat(len).Concat(data).ToArray();
            /*if (data != null)
            {
                sendBytes = sendBytes.Concat(data).ToArray();
            }
            else if (acknowledge != null)
            {
                sendBytes = sendBytes.Concat(acknowledge).ToArray();
            }
            */
            sendBytes = sendBytes.Concat(checksum).ToArray();

            return sendBytes;
        }

        /// <summary>
        /// Convert message structure to string
        /// </summary>
        /// <returns></returns>
       /* override public string ToString()
        {
            // return exact field values
            string res = "";
            res += "Start:" + BitConverter.ToString(start);
            //res += " Id:" + BitConverter.ToString(id);
            res += " Cmd:" + BitConverter.ToString(command);
            res += " Addr:" + BitConverter.ToString(address);
            res += " Len:" + BitConverter.ToString(type);
            if (data != null)
                res += " Data:" + BitConverter.ToString(data);
            else if (acknowledge != null)
                res += " Ack:" + BitConverter.ToString(acknowledge);
            res += " Chks:" + BitConverter.ToString(checksum);
            res += " End:" + BitConverter.ToString(end);
            return res;
        }
*/
   /*     public byte[] getBinaryBytes()
        {
            byte[] sendBytes;

            byte[] bin_id = Packet.AsciiByteToHexNibble(id);

            byte[] bin_addr = Packet.AsciiByteToHexNibble(address);

            byte[] bin_len = Packet.AsciiByteToHexNibble(type);

            byte[] bin_chks = Packet.AsciiByteToHexNibble(checksum);

            sendBytes = start.Concat(bin_id).Concat(command).Concat(bin_addr).Concat(bin_len).ToArray();
            if (data != null)
            {
                byte[] bin_data = Packet.AsciiByteToHexNibble(data);
                sendBytes = sendBytes.Concat(bin_data).ToArray();
            }
            else if (acknowledge != null)
            {
                sendBytes = sendBytes.Concat(acknowledge).ToArray();
            }

            sendBytes = sendBytes.Concat(bin_chks).Concat(end).ToArray();

            return sendBytes;

        }
   */
   
        /// <summary>
        /// return data field
        /// </summary>
        /// <returns></returns>
        public byte[] getData()
        {
            return data;
        }
        /// <summary>
        ///  return dataType field
        /// </summary>
        /// <returns></returns>
        public byte getDataType()
        {
            return command[0];
        }

        /// <summary>
        /// return acknowledge field
        /// </summary>
        /// <returns></returns>
        public byte[] getAcknowledge()
        {
            return acknowledge;
        }




        /// <summary>
        /// Convert hex string to byte array
        /// string 019F314A to bytes[] { 0x01, 0x9F, 0x31, 0x4A }
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string source)
        {
            
            int numberOfChars = source.Length;
            int numberOfBytes = numberOfChars / 2;
            byte[] bytes = new byte[numberOfBytes];
            for (int i = 0; i < numberOfChars-1; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(source.Substring(i, 2), 16);
            }

            return bytes;
        }
    }

}
