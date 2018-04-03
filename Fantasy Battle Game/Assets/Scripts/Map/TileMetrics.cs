using System.Collections.Generic;
using UnityEngine;

namespace assets.scripts.map
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
            public int Q;
            public int R;

            public HexCoordinate(int q, int r)
            {
                Q = q;
                R = r;
            }

            public static HexCoordinate operator +(HexCoordinate first, HexCoordinate second)
            {
                return new HexCoordinate(first.Q + second.Q, first.R + second.R);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                HexCoordinate o = (HexCoordinate)obj;
                return ((Q == o.Q) && (R == o.R));
            }

            public override string ToString()
            {
                return string.Format("[" + Q + "," + R + "]");
            }

            public override int GetHashCode()
            {
                var hashCode = -1997189103;
                hashCode = hashCode * -1521134295 + base.GetHashCode();
                hashCode = hashCode * -1521134295 + Q.GetHashCode();
                hashCode = hashCode * -1521134295 + R.GetHashCode();
                return hashCode;
            }
        }

    }
}
