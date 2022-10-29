using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileDeploy.PathScanOp;

namespace FileDeploy
{
    internal class PathCsCompile : IScan
    {
        TextWriter wr;
        int idxRelativePathStart;

        internal PathCsCompile() : this(Console.Out)
        {
        }

        internal PathCsCompile(TextWriter writer)
        {
            wr = writer;
        }

        void IScan.Scan(PathScanOp.ScanParam sp)
        {
            var p = sp.Part;
            if (EScanPart.Dir == p)
            {
                if(0 == sp.Level)
                {
                    idxRelativePathStart = (1 + sp.PathIn.Length);
                }
            }
            if (EScanPart.File == p)
            {
                string pth = sp.PathIn;
                int len = pth.Length;
                int idx = idxRelativePathStart;
                if (len > idx)
                {
                    string pthRel = pth.Substring(idx);
                    if (!pthRel.EndsWith(".cs")) return;
                    wr.WriteLine("    <Compile Include=\"{0}\" />", pthRel);
                }
            }
        }
    } // end - class PathCsCompile
}
