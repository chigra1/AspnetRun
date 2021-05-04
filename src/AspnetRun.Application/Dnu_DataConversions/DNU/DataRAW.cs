
using AspnetRun.Application.Dnu_Formating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspnetRun.Application.Dnu_DataConversions
{
    class DataRAW : IfType
    {
        /*
        RAW_BYTES

        This type is introduced to enable writing / reading N bytes to/from specified address.

        Example Write command string: “”
        */
        private int RAWlength;
        private int maxRAWlength=114;

        public DataRAW()
        {

        }
        public DataRAW(int length)
        {
            if (length > maxRAWlength)
                RAWlength = maxRAWlength;
            else
                RAWlength = length;
        }
        public int getFieldLength()
        {
            return RAWlength * 2;
        }

        public object getRealData(byte[] data)
        {
            //object res = Packet.AsciiByteToHexNibble(data);

            return null;
        }

        public string ToStringRealData(byte[] data)
        {
            return BitConverter.ToString((byte[])getRealData(data));
        }

        public byte[] setFieldData(string strData)
        {
            byte[] hex_data = Packet.HexStringToBytes(strData);
            //byte[] res = Packet.HexNiblleToAsciiByte(hex_data);
            return null;//res;
        }

        public byte[] setFieldType()
        {

            byte[] hex = BitConverter.GetBytes((short)((short)RAWlength|0x8000));
            //byte[] res = Packet.HexNiblleToAsciiByte(hex);
            return null;// res;
        }

        public bool checkStringPattern(string text)
        {
            if (text == null || text == "")
                return false;
            string pattern = @"^[0-9A-F]+$";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = Regex.Match(text, pattern);
            string res = m.Value;
            return m.Success;
        }

        public bool checkMinMax(string input, string min, string max)
        {
            if (input.Length <= maxRAWlength)
                return true;
            else
                return false;
        }

        public Task<bool> SaveToDbAsync(byte[] data, string address)
        {
            throw new NotImplementedException();
        }
    }
}
