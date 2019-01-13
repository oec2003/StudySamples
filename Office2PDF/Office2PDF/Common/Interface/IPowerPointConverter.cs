using System;
using System.Collections.Generic;
using System.Text;

namespace Office2PDF.Common
{
      interface IPowerPointConverter
    {
        bool OnWork(MQ.Messages message = null);
    }
}
