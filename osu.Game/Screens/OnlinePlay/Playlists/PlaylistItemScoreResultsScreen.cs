// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Online.API;
using osu.Game.Online.Rooms;
using osu.Game.Scoring;

namespace osu.Game.Screens.OnlinePlay.Playlists
{
    /// <summary>
    /// Shows a given score in a playlist item, with scores around included.
    /// </summary>
    public partial class PlaylistItemScoreResultsScreen : PlaylistItemResultsScreen
    {
        private readonly long scoreId;

        public PlaylistItemScoreResultsScreen(ScoreInfo score, long roomId, PlaylistItem playlistItem)
            : base(score, roomId, playlistItem)
        {
            scoreId = score.OnlineID;
        }

        public PlaylistItemScoreResultsScreen(long scoreId, long roomId, PlaylistItem playlistItem)
            : base(null, roomId, playlistItem)
        {
            this.scoreId = scoreId;
        }

        protected override APIRequest<MultiplayerScore> CreateScoreRequest() => new ShowPlaylistScoreRequest(RoomId, PlaylistItem.ID, scoreId);

        protected override ScoreInfo[] TransformScores(List<MultiplayerScore> scores, MultiplayerScores? pivot = null)
        {
            var scoreInfos = base.TransformScores(scores, pivot);
            Schedule(() => SelectedScore.Value ??= scoreInfos.SingleOrDefault(s => s.OnlineID == scoreId));
            return scoreInfos;
        }
    }
}
