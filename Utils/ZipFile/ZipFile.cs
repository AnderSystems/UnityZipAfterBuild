using UnityEngine;
using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class ZipFile 
{
    /// <summary>
    /// Write the given bytes data under the given filePath. 
    /// The filePath should be given with its path and filename. (e.g. c:/tmp/test.zip)
    /// </summary>
    public static void UnZip(string filePath, byte[] data)
    {
        using (ZipInputStream s = new ZipInputStream(new MemoryStream(data)))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                #if UNITY_EDITOR
                Debug.LogFormat("Entry Name: {0}", theEntry.Name);
                #endif

                string directoryName = Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);

                // create directory
                if (directoryName.Length > 0)
                {
                    var dirPath = Path.Combine (filePath, directoryName);

                    #if UNITY_EDITOR
                    Debug.LogFormat("CreateDirectory: {0}", dirPath);
                    #endif

                    Directory.CreateDirectory(dirPath);
                }

                if (fileName != string.Empty)
                {
                    // retrieve directory name only from persistence data path.
                    var entryFilePath = Path.Combine(filePath, theEntry.Name);
                    using (FileStream streamWriter = File.Create(entryFilePath))
                    {
                        int size = 2048;
                        byte[] fdata = new byte[size];
                        while (true)
                        {
                            size = s.Read(fdata, 0, fdata.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(fdata, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            } //end of while
        } //end of using
    }
    public static void Zip(string DirectoryPath, string OutputFilePath, int CompressionLevel = 9)
    {
        //copyDirectory(@"C:\x", @"D:\1");
        ZipOutputStream zip = new ZipOutputStream(File.Create(OutputFilePath));

        zip.SetLevel(9);

        string folder = DirectoryPath;

        ZipFolder(folder, folder, zip);
        zip.Finish();
        zip.Close();
    }

    public static void ZipFolder(string RootFolder, string CurrentFolder, ZipOutputStream zStream)
    {
        string[] SubFolders = Directory.GetDirectories(CurrentFolder);

        foreach (string Folder in SubFolders)
            ZipFolder(RootFolder, Folder, zStream);

        string relativePath = (CurrentFolder.Substring(RootFolder.Length) + "/").Replace(@"\", "");

        if (relativePath.Length > 1)
        {
            ZipEntry dirEntry;

            dirEntry = new ZipEntry(relativePath.Replace(@"\", ""));
            dirEntry.DateTime = DateTime.Now;
        }

        foreach (string file in Directory.GetFiles(CurrentFolder))
        {
            if (!file.EndsWith(".zip"))
            {
                AddFileToZip(zStream, relativePath, file);
            }
        }
    }

    private static void AddFileToZip(ZipOutputStream zStream, string relativePath, string file)
    {
        byte[] buffer = new byte[4096];
        string fileRelativePath = (relativePath.Length > 1 ? relativePath : string.Empty) + Path.GetFileName(file);
        ZipEntry entry = new ZipEntry(fileRelativePath);

        entry.DateTime = DateTime.Now;
        zStream.PutNextEntry(entry);

        using (FileStream fs = File.OpenRead(file))
        {
            int sourceBytes;

            do
            {
                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                zStream.Write(buffer, 0, sourceBytes);
            } while (sourceBytes > 0);
        }
    }
}
