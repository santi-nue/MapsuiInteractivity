﻿using Mapsui.UI;

namespace Mapsui.Interactivity
{
    public abstract class BaseInteractive : IInteractive
    {
        public event EventHandler? InvalidateLayer;

        protected void Invalidate()
        {
            InvalidateLayer?.Invoke(this, EventArgs.Empty);
        }

        public abstract IEnumerable<MPoint> GetActiveVertices();

        public abstract void Starting(MPoint worldPosition);

        public abstract void Moving(MPoint worldPosition);

        public abstract void Ending(MapInfo? mapInfo, Predicate<MPoint>? isEnd = null);

        public abstract void Hovering(MapInfo? mapInfo);

        public abstract void Dispose(MapInfo? mapInfo);
    }
}
