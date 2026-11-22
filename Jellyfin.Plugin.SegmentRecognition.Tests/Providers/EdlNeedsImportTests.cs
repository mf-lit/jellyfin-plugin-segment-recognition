using System;
using System.IO;
using Jellyfin.Plugin.SegmentRecognition.Providers;
using Xunit;

namespace Jellyfin.Plugin.SegmentRecognition.Tests.Providers;

/// <summary>
/// An EDL written after the item was first analyzed (comskip finishing after the recording was
/// scanned, or a hand edit) must be imported on the next task run.
/// </summary>
public sealed class EdlNeedsImportTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _mediaPath;
    private readonly string _edlPath;

    public EdlNeedsImportTests()
    {
        _tempDir = Path.Join(Path.GetTempPath(), "edl-needs-import-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_tempDir);
        _mediaPath = Path.Join(_tempDir, "Recording - 20261120_131500.mkv");
        _edlPath = Path.ChangeExtension(_mediaPath, ".edl");
    }

    public void Dispose()
    {
        Directory.Delete(_tempDir, recursive: true);
    }

    private void WriteEdl(string content, DateTime lastWriteUtc)
    {
        File.WriteAllText(_edlPath, content);
        File.SetLastWriteTimeUtc(_edlPath, lastWriteUtc);
    }

    [Fact]
    public void NoEdl_DoesNotNeedImport()
    {
        Assert.False(EdlImportProvider.NeedsImport(_mediaPath, null));
    }

    [Fact]
    public void EdlNeverImported_NeedsImport()
    {
        WriteEdl("2428.62\t2633.56\t3\n", new DateTime(2026, 11, 21, 0, 29, 37, DateTimeKind.Utc));

        Assert.True(EdlImportProvider.NeedsImport(_mediaPath, null));
    }

    [Fact]
    public void EdlWrittenAfterLastImport_NeedsImport()
    {
        // Analyzed while comskip was still running; the EDL landed later.
        WriteEdl("2428.62\t2633.56\t3\n", new DateTime(2026, 11, 21, 0, 29, 37, DateTimeKind.Utc));

        Assert.True(EdlImportProvider.NeedsImport(_mediaPath, new DateTime(2026, 11, 20, 16, 46, 50, DateTimeKind.Utc)));
    }

    [Fact]
    public void EdlUnchangedSinceImport_DoesNotNeedImport()
    {
        WriteEdl("2428.62\t2633.56\t3\n", new DateTime(2026, 11, 21, 0, 29, 37, DateTimeKind.Utc));

        Assert.False(EdlImportProvider.NeedsImport(_mediaPath, new DateTime(2026, 11, 22, 2, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public void ImportTimeReadBackAsUnspecifiedKind_StillCompares()
    {
        // EF/SQLite returns stored UTC values with DateTimeKind.Unspecified.
        WriteEdl("2428.62\t2633.56\t3\n", new DateTime(2026, 11, 21, 0, 29, 37, DateTimeKind.Utc));

        Assert.False(EdlImportProvider.NeedsImport(_mediaPath, new DateTime(2026, 11, 22, 2, 0, 0, DateTimeKind.Unspecified)));
        Assert.True(EdlImportProvider.NeedsImport(_mediaPath, new DateTime(2026, 11, 20, 16, 46, 50, DateTimeKind.Unspecified)));
    }

    [Fact]
    public void EdlExportedByThisPlugin_DoesNotNeedImport()
    {
        WriteEdl(EdlImportProvider.GeneratedMarker + "\n10\t20\t3\n", new DateTime(2026, 11, 21, 0, 29, 37, DateTimeKind.Utc));

        Assert.False(EdlImportProvider.NeedsImport(_mediaPath, null));
    }

    [Fact]
    public void EmptyMediaPath_DoesNotNeedImport()
    {
        Assert.False(EdlImportProvider.NeedsImport(string.Empty, null));
    }
}
