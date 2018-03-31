using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
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
        public float HexRadius = 10;
        public Material LineMaterial;
        public Material ProjectorMaterial; // used to show range of move
        public List<HexParametr> HexParametrs = new List<HexParametr>();

        [Serializable]
        public enum MapShape
        {
            Rectangle,
            Hexagon,
            Parrallelogram,
            Triangle
        }

        [Serializable]
        public enum HexOrientation
        {
            Pointy,
            Flat
        }

        [Serializable]
        public struct HexParametr
        {
            public Material Material;
            public Boolean Available;
            public Double Drag;
        }
    }
}
