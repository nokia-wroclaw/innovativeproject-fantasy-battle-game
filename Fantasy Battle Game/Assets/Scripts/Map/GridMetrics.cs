using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class GridMetrics : MonoBehaviour
    {
        //Generation Options
        public bool DrawOutlines = true;
        public int Seed = 0; // if Seed == 0 algorithm use time-dependent seed value

        //Map settings
        public GridGenerator.MapShape SelectedMapShape = GridGenerator.MapShape.Hexagon;
        public int MapWidth;
        public int MapHeight;

        //Hex Settings
        public GridGenerator.HexOrientation SelectedHexOrientation = GridGenerator.HexOrientation.Flat;
        public float HexRadius = 10;
        public Material LineMaterial;
        public Material ProjectorMaterial; // used to show range of move
        public List<GridGenerator.HexParametr> HexParametrs = new List<GridGenerator.HexParametr>();

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
            public Material material;
            public Boolean available;
            public Double drag;
        }
    }
}
