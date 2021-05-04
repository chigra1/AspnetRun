using AspnetRun.Application.Dnu_Formating;
using AspnetRun.Application.Interfaces;
using AspnetRun.Application.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace AspnetRun.Application.Dnu_DataConversions
{
    public class DataMerenja : IfType
    {

        private readonly IProductService _productAppService;
        public DataMerenja()
        {
        }
        public DataMerenja(IProductService productAppService)
        {
            _productAppService = productAppService ?? throw new ArgumentNullException(nameof(productAppService));
            
        }

        //private readonly IpmsDbContext _context;
        /*
        MERENJA 0xE0

        PC generiše generiše zahtev za trenutna merenja 

        cc cc cc ADRdst ADRsrc E0 00 01 00 CS

        Primer:
        cc cc cc 01 ff E0 00 01 00 E1

        Dnu 24 odgovara PC-ju na zahtev za trenutna merenja 

        cc cc cc ADRdst ADRsrc e0 00 23  UpH UpL Ub1H Ub1L Ub2H Ub2L Up4H Up4L Up5H Up5L IpH IpL Ub1H Ib1L Ib2H Ib2L TcpuH TcpuL TbH TbL TaH TaL UrH UrL UsH UsL UtH UtL UrfH UrfL UsfH UsfL UtfH UtfL staDIGAL CS

        Merenja su dvobajtna, tako da je 34 bajtova iskorišćeno za merenja,a jedan bajt staDIGAL za status digitalnih alarma.

        Primer:
        cc cc cc ff 01 e0 00 23 03 82 03 7e 00 00 03 ff 03 ff 02 00 02 11 00 01 01 30 00 71 00 70 00 8c 00 8d 00 00 00 8b 00 00 00 00 55 2e


        
             */

        private int len = 35;
        public int getDataLength()
        {
            return len;
        }
        public int getFieldLength()
        {
            return len;
        }


        public object getRealData(byte[] data)
        {
            object res = extractData(data);

            return res;

        }

        public string ToStringRealData(byte[] data)
        {
            string real="";
            decimal[] temp = (decimal[])getRealData(data);
            foreach(decimal merenje in temp)
            { real = real + merenje.ToString("F") + "\n"; }
            return real;
        }

        public byte[] setFieldData(string strData)
        {
            int val = Convert.ToInt16(strData);
            byte[] data = new byte[1] { (byte)(val) };
            //byte[] res = Packet.HexNiblleToAsciiByte(hex_data);

            return data;
        }

        public byte[] setFieldType()
        {
            byte[] hex = BitConverter.GetBytes((short)Packet.DataType.Merenja);
            //byte[] res = Packet.HexNiblleToAsciiByte(hex);
            return hex;
        }

        private decimal[] extractData(byte[] data)
        {
            decimal[] merenje = new decimal[17];
            merenje[0] = (data[1] + data[0] * 256) * ((decimal)60 / 1024);
            merenje[1] = (data[3] + data[2] * 256) * ((decimal)60 / 1024);
            merenje[2] = (data[5] + data[4] * 256) * ((decimal)60 / 1024);
            merenje[3] = (data[7] + data[6] * 256) * ((decimal)60 / 1024);
            merenje[4] = (data[9] + data[8] * 256) * ((decimal)60 / 1024);
            merenje[5] = ((data[11] + data[10] * 256) - 512) * ((decimal)100 / 1024);
            merenje[6] = ((data[13] + data[12] * 256) - 512) * ((decimal)100 / 1024);
            merenje[7] = ((data[15] + data[14] * 256) - 512) * ((decimal)100 / 1024);
            //tproc
            merenje[16] = (data[17] + data[16] * 256) * (decimal)0.25;
            //
            merenje[8] = (data[19] + data[18] * 256) * ((decimal)60 / 1024);
            merenje[9] = (data[21] + data[20] * 256) * ((decimal)60 / 1024);
            merenje[10] = (data[23] + data[22] * 256) * ((decimal)300 / 1024);
            merenje[11] = (data[25] + data[24] * 256) * ((decimal)300 / 1024);
            merenje[12] = (data[27] + data[26] * 256) * ((decimal)300 / 1024);
            merenje[13] = (data[29] + data[28] * 256) * ((decimal)300 / 1024);
            merenje[14] = (data[31] + data[30] * 256) * ((decimal)300 / 1024);
            merenje[15] = (data[33] + data[32] * 256) * ((decimal)300 / 1024);
            return merenje;
        }
        public bool checkMinMax(string input, string min, string max)
        {
            //if (min == "" && max == "")
            //    return true;

            //float enteredFloat;
            //float minFloat;
            //float maxFloat;

            //try
            //{
            //    enteredFloat = Convert.ToSingle(input);
            //    minFloat = Convert.ToSingle(min);
            //    maxFloat = Convert.ToSingle(max);
            //}
            //catch
            //{
            //    return false;
            //}
            //if (minFloat <= enteredFloat && enteredFloat <= maxFloat)
            //    return true;
            //else
            return false;
        }

        public bool checkStringPattern(string text)
        {
            if (text == null || text == "")
                return false;
            string pattern = @"^([+-]?)\d*(\.(?=\d))?\d*$";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = Regex.Match(text, pattern);
            string res = m.Value;
            return m.Success;
        }

        public async Task<bool> SaveToDbAsync(byte[] data,string address)
        {
            decimal[] merenja = extractData(data);
            //get id of device

            ProductModel product = await _productAppService.GetProductById(1);
            //await Update(product);
            //save
            product.UnitPrice = 101;
            await _productAppService.Update(product);

            return true;
        }

    }
}
