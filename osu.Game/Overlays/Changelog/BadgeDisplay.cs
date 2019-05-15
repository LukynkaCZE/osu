﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Online.API.Requests.Responses;
using System;
using System.Collections.Generic;
using osuTK.Graphics;

namespace osu.Game.Overlays.Changelog
{
    public class BadgeDisplay : CompositeDrawable
    {
        private const float vertical_padding = 20;
        private const float horizontal_padding = 85;

        public event Action<APIChangelogBuild> Selected;

        private readonly FillFlowContainer<StreamBadge> badgesContainer;
        private long selectedStreamId = -1;

        public BadgeDisplay()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(32, 24, 35, 255),
                },
                badgesContainer = new FillFlowContainer<StreamBadge>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Vertical = vertical_padding, Horizontal = horizontal_padding },
                },
            };
        }

        public void Populate(List<APIUpdateStream> streams)
        {
            SelectNone();

            foreach (APIUpdateStream updateStream in streams)
            {
                var streamBadge = new StreamBadge(updateStream);
                streamBadge.Selected += onBadgeSelected;
                badgesContainer.Add(streamBadge);
            }
        }

        public void SelectNone()
        {
            selectedStreamId = -1;

            if (badgesContainer != null)
            {
                foreach (StreamBadge streamBadge in badgesContainer)
                {
                    if (!IsHovered)
                        streamBadge.Activate();
                    else
                        streamBadge.Deactivate();
                }
            }
        }

        public void SelectUpdateStream(string updateStream)
        {
            foreach (StreamBadge streamBadge in badgesContainer)
            {
                if (streamBadge.Stream.Name == updateStream)
                {
                    selectedStreamId = streamBadge.Stream.Id;
                    streamBadge.Activate();
                }
                else
                    streamBadge.Deactivate();
            }
        }

        private void onBadgeSelected(StreamBadge source, EventArgs args)
        {
            selectedStreamId = source.Stream.Id;
            OnSelected(source);
        }

        protected virtual void OnSelected(StreamBadge source)
        {
            Selected?.Invoke(source.Stream.LatestBuild);
        }

        protected override bool OnHover(HoverEvent e)
        {
            foreach (StreamBadge streamBadge in badgesContainer.Children)
            {
                if (selectedStreamId >= 0)
                {
                    if (selectedStreamId != streamBadge.Stream.Id)
                        streamBadge.Deactivate();
                    else
                        streamBadge.EnableDim();
                }
                else
                    streamBadge.Deactivate();
            }

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            foreach (StreamBadge streamBadge in badgesContainer.Children)
            {
                if (selectedStreamId < 0)
                    streamBadge.Activate();
                else if (streamBadge.Stream.Id == selectedStreamId)
                    streamBadge.DisableDim();
            }

            base.OnHoverLost(e);
        }
    }
}
