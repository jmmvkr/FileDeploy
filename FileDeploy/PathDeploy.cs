using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDeploy
{

    internal class PathDeploy : IScan
    {
        internal string DeployTarget = null;
        int posRel = 0;

        static internal void RunDeploy(PathScanOp scanObj, string src, string dst)
        {
            PathDeploy pd = new PathDeploy() { DeployTarget = dst };
            scanObj.ScanDir(src, pd);
            Console.WriteLine(" -- deploy done --");
        }

        void IScan.Scan(PathScanOp.ScanParam sp)
        {
            Deploy(sp);
        }

        void Deploy(PathScanOp.ScanParam sp)
        {
            int lv = sp.Level;
            if (0 == lv)
            {
                string rtPath = sp.PathIn;
                posRel = 1 + rtPath.Length;
                return;
            }

            PathScanOp.EScanPart part = sp.Part;
            string pth = sp.PathIn;
            string pthRel = pth.Substring(posRel);
            string pthDeploy = $@"{DeployTarget}\{pthRel}";

            if (PathScanOp.EScanPart.Dir == part)
            {
                Directory.CreateDirectory(pthDeploy);
            }
            else
            {
                var buf = File.ReadAllBytes(pth);
                File.WriteAllBytes(pthDeploy, buf);
            }
            Console.WriteLine(pthDeploy);
        }

    } // end - class PathDeploy

}
