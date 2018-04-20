using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// Class contain setting of map
    /// </summary>
    public class GridMetrics : MonoBehaviour
    {

        //Generation Options
        public bool DrawOutlines = true;
        public int Seed = 0; // if Seed == 0 algorithm use time-dependent seed value

        //Map settings
        public MapShape SelectedMapShape = MapShape.Hexagon;
        public int MapWidth;
        public int MapHeight;

        //Hex Settings
        public HexOrientation SelectedHexOrientation = HexOrientation.Flat;
        public const float HexRadius = 10;
        public Material LineMaterial;
        public Material ProjectorMaterial; // used to show range of move
        public List<GameObject> Prefabs = new List<GameObject>();
        public GameObject Label;
        public GameObject Projector;

        public static TileMetrics.HexCoordinate[] Directions =
        {
            new TileMetrics.HexCoordinate(0, -1),
            new TileMetrics.HexCoordinate(-1, 0),
            new TileMetrics.HexCoordinate(-1, 1),
            new TileMetrics.HexCoordinate(1, -1),
            new TileMetrics.HexCoordinate(1, 0),
            new TileMetrics.HexCoordinate(0, 1)
        };


        private static GridMetrics instance_;

        public static GridMetrics Instance
        {
            get { return instance_; }
            set { instance_ = value; }
        }
        

        public GridMetrics()
        {
            if (instance_ == null)
            {
                instance_ = this;
            }
        }

        [Serializable]
        public enum MapShape
        {
            Rectangle,
            Hexagon,
            Parallelogram,
            Triangle
        }

        /// <summary>
        /// Orientation of hexes.
        /// https://www.redblobgames.com/grids/hexagons/#size-and-spacing
        /// </summary>
        [Serializable]
        public enum HexOrientation
        {
            /// <summary>
            /// horizontal orientation
            /// </summary>
            Pointy,
            /// <summary>
            /// vertical orientation
            /// </summary>
            Flat
        }

        [Serializable]
        public struct HexParameter
        {
            public Material Material;
            public Boolean Available;
            public Double Drag;
        }
    }
}
