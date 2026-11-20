using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using Xunit;

namespace Jellyfin.Plugin.SegmentRecognition.Tests;

public sealed class ItemKindsTests
{
    [Fact]
    public void PlainVideo_IsStandaloneAndMovieLike()
    {
        var video = new Video();

        Assert.True(ItemKinds.IsStandaloneVideo(video));
        Assert.True(ItemKinds.IsMovieLike(video));
    }

    [Fact]
    public void Movie_IsMovieLikeButNotStandaloneVideo()
    {
        var movie = new Movie();

        Assert.False(ItemKinds.IsStandaloneVideo(movie));
        Assert.True(ItemKinds.IsMovieLike(movie));
    }

    [Fact]
    public void Episode_IsNeither()
    {
        var episode = new Episode();

        Assert.False(ItemKinds.IsStandaloneVideo(episode));
        Assert.False(ItemKinds.IsMovieLike(episode));
    }

    [Fact]
    public void VideoExtra_IsExcluded()
    {
        var extra = new Video { ExtraType = ExtraType.Featurette };

        Assert.False(ItemKinds.IsStandaloneVideo(extra));
        Assert.False(ItemKinds.IsMovieLike(extra));
    }

    [Fact]
    public void Null_IsNeither()
    {
        Assert.False(ItemKinds.IsStandaloneVideo(null));
        Assert.False(ItemKinds.IsMovieLike(null));
    }
}
