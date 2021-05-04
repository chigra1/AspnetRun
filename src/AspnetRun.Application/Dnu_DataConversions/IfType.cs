using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AspnetRun.Application.Dnu_DataConversions

{
    public interface IfType
    {
        byte[]  setFieldType();
        byte[]  setFieldData(string strData);
        int     getFieldLength();
        object  getRealData(byte[] data);
        string  ToStringRealData(byte[] data);
        bool    checkStringPattern(string text);
        bool  checkMinMax(string input, string min, string max);


        Task<bool> SaveToDbAsync(byte[] data,string address);
    }
}
