namespace FileTreeScanning.Scanning;

public sealed record ScanFile(
    ScanFileType Type,
    string FullPath,
    DateTime? LastModificationTimestamp,
    long? Size);
