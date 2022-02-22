using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDeploy
{
    internal interface IScan
    {
        void Scan(PathScanOp.ScanParam sp);
    }
}
