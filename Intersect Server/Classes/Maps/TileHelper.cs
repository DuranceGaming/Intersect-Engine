﻿using System;
using Intersect.Enums;

namespace Intersect.Server.Classes.Maps
{
    using LegacyDatabase = Intersect.Server.Classes.Core.LegacyDatabase;

    public class TileHelper
    {
        private Guid mMapId;
        private int mTileX;
        private int mTileY;

        /// <summary>
        ///     Creates a new tile helper instance in a position given.
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        public TileHelper(Guid mapId, int tileX, int tileY)
        {
            mMapId = mapId;
            mTileX = tileX;
            mTileY = tileY;
        }

        /// <summary>
        ///     Moves our tile and then attempts to adjust the map location of we walked out of bounds. Will return true if the
        ///     position is valid. False if not.
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        public bool Translate(int xOffset, int yOffset)
        {
            mTileX += xOffset;
            mTileY += yOffset;
            return TryFix();
        }

        public bool TryFix()
        {
            int oldTileX = mTileX;
            int oldTileY = mTileY;
            if (Fix()) return true;
            mTileX = oldTileX;
            mTileY = oldTileY;
            return false;
        }

        public bool Matches(TileHelper other)
        {
            if (GetMapId() == other.GetMapId() && GetX() == other.GetX() && GetY() == other.GetY()) return true;
            return false;
        }

        private bool TransitionMaps(int direction)
        {
            if (!MapInstance.Lookup.Keys.Contains(mMapId)) return false;
            int grid = MapInstance.Get(mMapId).MapGrid;
            int gridX = MapInstance.Get(mMapId).MapGridX;
            int gridY = MapInstance.Get(mMapId).MapGridY;
            switch (direction)
            {
                case (int) Directions.Up:
                    if (gridY > 0 && LegacyDatabase.MapGrids[grid].MyGrid[gridX, gridY - 1] != Guid.Empty)
                    {
                        mMapId = LegacyDatabase.MapGrids[grid].MyGrid[gridX, gridY - 1];
                        mTileY += Options.MapHeight;
                        return true;
                    }
                    return false;
                case (int) Directions.Down:
                    if (gridY + 1 < LegacyDatabase.MapGrids[grid].Height &&
                        LegacyDatabase.MapGrids[grid].MyGrid[gridX, gridY + 1] != Guid.Empty)
                    {
                        mMapId = LegacyDatabase.MapGrids[grid].MyGrid[gridX, gridY + 1];
                        mTileY -= Options.MapHeight;
                        return true;
                    }
                    return false;
                case (int) Directions.Left:
                    if (gridX > 0 && LegacyDatabase.MapGrids[grid].MyGrid[gridX - 1, gridY] != Guid.Empty)
                    {
                        mMapId = LegacyDatabase.MapGrids[grid].MyGrid[gridX - 1, gridY];
                        mTileX += Options.MapWidth;
                        return true;
                    }
                    return false;
                case (int) Directions.Right:
                    if (gridX + 1 < LegacyDatabase.MapGrids[grid].Width &&
                        LegacyDatabase.MapGrids[grid].MyGrid[gridX + 1, gridY] != Guid.Empty)
                    {
                        mMapId = LegacyDatabase.MapGrids[grid].MyGrid[gridX + 1, gridY];
                        mTileX -= Options.MapWidth;
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool Fix()
        {
            if (!MapInstance.Lookup.Keys.Contains(mMapId)) return false;
            MapInstance curMap = MapInstance.Get(mMapId);
            while (mTileX < 0)
            {
                if (!TransitionMaps((int) Directions.Left)) return false;
            }
            while (mTileY < 0)
            {
                if (!TransitionMaps((int) Directions.Up)) return false;
            }
            while (mTileX >= Options.MapWidth)
            {
                if (!TransitionMaps((int) Directions.Right)) return false;
            }
            while (mTileY >= Options.MapHeight)
            {
                if (!TransitionMaps((int) Directions.Down)) return false;
            }
            return true;
        }

        public Guid GetMapId()
        {
            return mMapId;
        }

        public int GetX()
        {
            return mTileX;
        }

        public int GetY()
        {
            return mTileY;
        }

        public static bool IsTileValid(Guid mapId, int tileX, int tileY)
        {
            if (!MapInstance.Lookup.Keys.Contains(mapId)) return false;
            if (tileX < 0 || tileX >= Options.MapWidth) return false;
            if (tileY < 0 || tileY >= Options.MapHeight) return false;
            return true;
        }
    }
}