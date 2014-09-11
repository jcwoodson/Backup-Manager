using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Backup_Manager
{
    class Program
    {
        static void CopyFolder(string input, string output, bool copyAll)
        {
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            foreach (string subdirString in Directory.GetDirectories(input))
            {
                try
                {
                    DirectoryInfo subdir = new DirectoryInfo(subdirString);
                    if ((subdir.Name != "AppData" && subdir.Name != ".VirtualBox" && subdir.Name != "VirtualBox VMs") || copyAll)
                    {
                        string outputSubdirString = output + "\\" + subdir.Name;
                        CopyFolder(subdirString, outputSubdirString, copyAll);
                        DirectoryInfo outputSubdir = new DirectoryInfo(outputSubdirString);
                        outputSubdir.Attributes = subdir.Attributes;
                        outputSubdir.LastWriteTimeUtc = subdir.LastWriteTimeUtc;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
            }

            foreach (string fileString in Directory.GetFiles(input))
            {
                FileInfo file = new FileInfo(fileString);
                string outputFileString = output + "\\" + file.Name;
                if (!File.Exists(outputFileString))
                {
                    try
                    {
                        File.Copy(fileString, outputFileString);
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                }
                else
                {
                    FileInfo outputFile = new FileInfo(outputFileString);
                    if (file.LastWriteTimeUtc > outputFile.LastWriteTimeUtc)
                    {
                        try
                        {
                            File.Copy(fileString, outputFileString, true);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            FileAttributes attrs = outputFile.Attributes;
                            outputFile.Attributes ^= (FileAttributes.Hidden | FileAttributes.ReadOnly);
                            try
                            {
                                File.Copy(fileString, outputFileString, true);
                            }
                            catch (UnauthorizedAccessException)
                            {
                            }
                            catch (IOException)
                            {
                            }
                            finally
                            {
                                outputFile.Attributes = attrs;
                            }
                        }
                        catch (IOException)
                        {
                        }
                    }
                }
            }

            Console.WriteLine(input + " Copied");
        }

        static void Main(string[] args)
        {
            if (args.Length == 2)
                CopyFolder(args[0], args[1], false);
            else if (args.Length == 3 && args[2] == "/a")
                CopyFolder(args[0], args[1], true);
            else
                Console.WriteLine("Copies modified files to a different location.\r\nUsage: backupman source destination [options]\r\nOptions:\r\n/a Copies all files.");
        }
    }
}
