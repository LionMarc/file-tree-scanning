namespace FileTreeScanning.Scanning;

public sealed record FolderStatsResult(string Folder, int FilesCount) : FolderScanResult(Folder);
