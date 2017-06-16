using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nugety.Project.Dependencies
{
    public static class FileUtil
    {
        public static void ProcessFiles(string source, string target, string mainAppBinFolder)
        {
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            ProcessDirectory(source, target, mainAppBinFolder);
        }

        public static void ProcessDirectory(string source, string target, string mainAppBinFolder)
        {
            var sourceInfo = new DirectoryInfo(source);
            var targetInfo = new DirectoryInfo(target);
            Directory.CreateDirectory(targetInfo.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in sourceInfo.GetFiles())
            {
                //is the file in the GAC?
                string fileName = fi.Name;
                string assemblyName = string.Empty;
                var inGAC = false;
                if (fileName.Contains(".dll"))
                {
                    assemblyName = System.Reflection.AssemblyName.GetAssemblyName(fi.FullName).ToString();
                    inGAC = GacUtil.IsAssemblyInGAC(assemblyName);
                }

                //if not in the GAC, is the file already in the mainAppBinFolder?
                if (!inGAC)
                {
                    if (!File.Exists(Path.Combine(mainAppBinFolder, fileName)))
                    {
                        //if not in either of the previous locations, then copy to target
                        fi.CopyTo(Path.Combine(targetInfo.FullName, fi.Name), true);
                    }
                }
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in sourceInfo.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = targetInfo.CreateSubdirectory(diSourceSubDir.Name);
                ProcessDirectory(diSourceSubDir.FullName, nextTargetSubDir.FullName, mainAppBinFolder);
            }
        }
    }
}
