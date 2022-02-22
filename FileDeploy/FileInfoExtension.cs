using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDeploy
{
    static class FileInfoExtension
    {
        static internal void CopyFileTime(string pthDest, string pthSrc)
        {
            File.SetCreationTimeUtc(pthDest, File.GetCreationTimeUtc(pthSrc));
            File.SetLastWriteTimeUtc(pthDest, File.GetLastWriteTimeUtc(pthSrc));
            File.SetLastAccessTimeUtc(pthDest, File.GetLastAccessTimeUtc(pthSrc));
        }

        static internal void SetFileTime(string pthDest, DateTime dt)
        {
            File.SetCreationTimeUtc(pthDest, dt);
            File.SetLastWriteTimeUtc(pthDest, dt);
            File.SetLastAccessTimeUtc(pthDest, dt);
        }

        static internal void CopyFileTime(this FileInfo dest, string pthSrc)
        {
            dest.CreationTimeUtc = (File.GetCreationTimeUtc(pthSrc));
            dest.LastWriteTimeUtc = (File.GetLastWriteTimeUtc(pthSrc));
            dest.LastAccessTimeUtc = (File.GetLastAccessTimeUtc(pthSrc));
        }

        static internal void SetFileTime(this FileInfo dest, DateTime dt)
        {
            dest.CreationTimeUtc = (dt);
            dest.LastWriteTimeUtc = (dt);
            dest.LastAccessTimeUtc = (dt);
        }

    } // end - class FileExtension
}
