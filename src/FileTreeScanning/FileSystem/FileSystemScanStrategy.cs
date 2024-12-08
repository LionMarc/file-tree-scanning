using FileTreeScanning.Scanning;

namespace FileTreeScanning;

public sealed class FileSystemScanStrategy : IScanStrategy
{
    public IEnumerable<ScanFile> ScanFolder(string folder)
    {
        var directory = new DirectoryInfo(folder);
        foreach (var item in directory.EnumerateFileSystemInfos())
        {
            if (item is DirectoryInfo)
            {
                yield return new ScanFile(ScanFileType.Folder, item.FullName, item.LastWriteTimeUtc, null);
            }
            else if (item is FileInfo fileInfo)
            {
                yield return new ScanFile(ScanFileType.File, item.FullName, item.LastWriteTimeUtc, fileInfo.Length);
            }
        }
    }
}
