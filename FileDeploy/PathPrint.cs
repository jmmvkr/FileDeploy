using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileDeploy.PathScanOp;

namespace FileDeploy
{
    internal class PathPrint : IScan
    {
        TextWriter wr;

        internal PathPrint() : this(Console.Out)
        {
        }

        internal PathPrint(TextWriter writer)
        {
            wr = writer;
        }

        public void Scan(ScanParam sp)
        {
            PrintScan(sp);
        }

        void PrintScan(ScanParam sp)
        {
            var p = sp.Part;
            var lv = sp.Level;
            var rt = sp.PathIn;

            if (EScanPart.File == p)
            {
                PrintTab(lv);
                wr.WriteLine("f{1} {0}", rt, lv);
            }
            else if (EScanPart.Dir == p)
            {
                PrintTab(lv);
                wr.WriteLine("d{1} {0}", rt, lv);
            }
        }

        void PrintTab(int lv)
        {
            for (int i = 0; i < lv; i++)
            {
                wr.Write('\t');
            }
        }

    } // end - class PathPrint
}
