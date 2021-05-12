using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspnetRun.Application.Dnu_Formating;

namespace AspnetRun.Application.Dnu_DataConversions
{
    class FactoryType
    {


        internal static IfType CreateType(Packet.DataType type)
        {

            switch (type)
            {
                case Packet.DataType.RAW:
                    return new DataRAW();

                default:
                    return null;

            }
        }
    }
}
