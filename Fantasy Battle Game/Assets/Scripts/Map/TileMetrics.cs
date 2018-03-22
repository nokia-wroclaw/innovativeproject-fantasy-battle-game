using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class TileMetrics
    {
        public static void GetHexMesh(float radius, GridGenerator.HexOrientation orientation, ref Mesh mesh)
        {
            mesh = new Mesh();

            List<int> triangles = generateIndexesOfTrianglesInHex();
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

        public static Vector3 Corner(Vector3 origin, float radius, int corner, GridGenerator.HexOrientation orientation)
        {
            float angle = 60 * corner;
            if (orientation == GridMetrics.HexOrientation.Pointy)
                angle += 30;
            angle *= Mathf.PI / 180;
            return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
        }

        private static List<int> generateIndexesOfTrianglesInHex()
        {
            List<int> triangles = new List<int>();

            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(1);

            triangles.Add(0);
            triangles.Add(5);
            triangles.Add(2);

            triangles.Add(2);
            triangles.Add(5);
            triangles.Add(3);

            triangles.Add(3);
            triangles.Add(5);
            triangles.Add(4);

            return triangles;
        }

        [System.Serializable]
        public struct Index
        {
            public readonly int Q;
            public readonly int R;

            public Index(int Q, int R)
            {
                this.Q = Q;
                this.R = R;
            }

            public static Index operator +(Index one, Index two)
            {
                return new Index(one.Q + two.Q, one.R + two.R);
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                Index o = (Index)obj;
                if ((System.Object)o == null)
                    return false;
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
