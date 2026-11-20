using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;

namespace Jellyfin.Plugin.SegmentRecognition;

/// <summary>
/// Classifies library items for analysis.
/// </summary>
internal static class ItemKinds
{
    /// <summary>
    /// Whether the item is a plain video that is neither a movie nor an episode nor an extra,
    /// e.g. a loose recording or home video.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> for a standalone plain video.</returns>
    internal static bool IsStandaloneVideo(BaseItem? item)
        => item is Video and not Movie and not Episode && item.ExtraType is null;

    /// <summary>
    /// Whether the item should be analysed with movie (rather than episode) heuristics:
    /// movies and standalone plain videos.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> for movie-like items.</returns>
    internal static bool IsMovieLike(BaseItem? item)
        => item is Movie || IsStandaloneVideo(item);
}
