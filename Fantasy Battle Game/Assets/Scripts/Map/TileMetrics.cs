using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class TileMetrics
    {
        public static void GetHexMesh(float radius, GridMetrics.HexOrientation orientation, ref Mesh mesh)
        {
            mesh = new Mesh();

            List<int> triangles = new List<int> { 0, 2, 1, 0, 5, 2, 2, 5, 3, 3, 5, 4 };
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            for (int i = 0; i < 6; i++)
                vertices.Add(Corner(Vector3.zero, radius, i, orientation));

            uvs.Add(new Vector2(0.5f, 1f));
            uvs.Add(new Vector2(1, 0.75f));
            uvs.Add(new Vector2(1, 0.25f));
            uvs.Add(new Vector2(0.5f, 0));
            uvs.Add(new Vector2(0, 0.25f));
            uvs.Add(new Vector2(0, 0.75f));

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            mesh.name = "Hexagonal Plane";

            mesh.RecalculateNormals();

            //AssetDatabase.CreateAsset(mesh, "Assets/mesh.prefab");
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }

        public static Vector3 Corner(Vector3 origin, float radius, int corner, GridMetrics.HexOrientation orientation)
        {
            float angle = 60 * corner;
            if (orientation == GridMetrics.HexOrientation.Pointy)
                angle += 30;
            angle *= Mathf.PI / 180;
            return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
        }

        [System.Serializable]
        public struct HexCoordinate
        {
            public int FirstCoord;
            public int SecondCoord;

            public HexCoordinate(int firstCoord, int secondCoord)
            {
                FirstCoord = firstCoord;
                SecondCoord = secondCoord;
            }

            public static HexCoordinate operator +(HexCoordinate first, HexCoordinate second)
            {
                return new HexCoordinate(first.FirstCoord + second.FirstCoord, first.SecondCoord + second.SecondCoord);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                HexCoordinate o = (HexCoordinate)obj;
                return ((FirstCoord == o.FirstCoord) && (SecondCoord == o.SecondCoord));
            }

            public override string ToString()
            {
                return string.Format("[" + FirstCoord + "," + SecondCoord + "]");
            }

            public override int GetHashCode()
            {
                var hashCode = -1997189103;
                hashCode = hashCode * -1521134295 + base.GetHashCode();
                hashCode = hashCode * -1521134295 + FirstCoord.GetHashCode();
                hashCode = hashCode * -1521134295 + SecondCoord.GetHashCode();
                return hashCode;
            }
        }

    }
}
