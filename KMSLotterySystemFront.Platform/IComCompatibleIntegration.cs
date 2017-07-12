
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace InquiryCompatibleIntegration.ComCompatibleInterface
{
    [ComVisible(true)]
    [Guid("E36BBF07-591E-4959-97AE-D439CBA392FB")] 
    public interface IComCompatibleIntegration
    {
        byte[] Decode(byte[] dynamicParams);
        byte[] ExecuteInquiry(byte[] dynamicParams);
    }
}

