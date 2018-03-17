using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Map
{
    public class Tile : MonoBehaviour {
        public GameObject TileGameObject;
        public GameObject ShGameObject;
        public CubeIndex Index;
        public bool Available = true;
        public double Drag = 1;
        public Vector3 Position = new Vector3(0, 0, 0);
        public double DistanceFromStart { get; set; }


        public static Vector3 Corner(Vector3 origin, float radius, int corner, HexOrientation orientation){
            float angle = 60 * corner;
            if(orientation == HexOrientation.Pointy)
                angle += 30;
            angle *= Mathf.PI / 180;
            return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
        }

        public void DeleteChildGO()
        {
            if(ShGameObject!=null)
                Destroy(ShGameObject);
            if(LabelGameObject!=null)
                Destroy(LabelGameObject);
        }

        public static void GetHexMesh(float radius, HexOrientation orientation, ref Mesh mesh) {
            mesh = new Mesh();

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            for (int i = 0; i < 6; i++)
                verts.Add(Corner(Vector3.zero, radius, i, orientation));

            tris.Add(0);
            tris.Add(2);
            tris.Add(1);
		
            tris.Add(0);
            tris.Add(5);
            tris.Add(2);
		
            tris.Add(2);
            tris.Add(5);
            tris.Add(3);
		
            tris.Add(3);
            tris.Add(5);
            tris.Add(4);

            //UVs are wrong, I need to find an equation for calucalting them
            uvs.Add(new Vector2(0.5f, 1f));
            uvs.Add(new Vector2(1, 0.75f));
            uvs.Add(new Vector2(1, 0.25f));
            uvs.Add(new Vector2(0.5f, 0));
            uvs.Add(new Vector2(0, 0.25f));
            uvs.Add(new Vector2(0, 0.75f));

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();

            mesh.name = "Hexagonal Plane";

            mesh.RecalculateNormals();
        }

        void OnMouseDown()
        {
            var grid = Assets.Map.Grid.Instance;
            var tileList = grid.TilesInRange(this, 12);

            foreach (var tile in tileList)
            {
                // Add Projector to tile
                GameObject go = new GameObject("Shadow");
                go.transform.parent = tile.TileGameObject.transform;
                go.transform.position = tile.Position + new Vector3(0, grid.HexRadius * 2, 0);
                go.transform.Rotate(new Vector3(90, 0, 0));
                tile.ShGameObject = go;
                go.AddComponent<Projector>();
                var x = go.GetComponent<Projector>();
                x.material = grid.ShadowMaterial;

                // Add label to tile
                GameObject labelGameObject = new GameObject("Label");
                labelGameObject.transform.parent = tile.TileGameObject.transform;
                labelGameObject.transform.position = tile.Position;
                labelGameObject.transform.Rotate(new Vector3(90, 0, 0));
                tile.LabelGameObject = labelGameObject;
                var text = labelGameObject.AddComponent<TextMesh>();
                text.text = Convert.ToString(Math.Round(tile.DistanceFromStart, 1));
                text.fontSize = (int) (grid.HexRadius*9);
                text.anchor = TextAnchor.MiddleCenter;
                text.color = Color.white;
            }
        }

        public GameObject LabelGameObject { get; set; }


        #region Coordinate Conversion Functions
        public static OffsetIndex CubeToEvenFlat(CubeIndex c) {
            OffsetIndex o;
            o.row = c.q;
            o.col = c.s + (c.q + (c.q&1)) / 2;
            return o;
        }

        public static CubeIndex EvenFlatToCube(OffsetIndex o){
            CubeIndex c;
            c.q = o.col;
            c.s = o.row - (o.col + (o.col&1)) / 2;
            c.r = -c.q - c.s;
            return c;
        }

        public static OffsetIndex CubeToOddFlat(CubeIndex c) {
            OffsetIndex o;
            o.col = c.q;
            o.row = c.s + (c.q - (c.q&1)) / 2;
            return o;
        }
	
        public static CubeIndex OddFlatToCube(OffsetIndex o){
            CubeIndex c;
            c.q = o.col;
            c.s = o.row - (o.col - (o.col&1)) / 2;
            c.r = -c.q - c.s;
            return c;
        }

        public static OffsetIndex CubeToEvenPointy(CubeIndex c) {
            OffsetIndex o;
            o.row = c.s;
            o.col = c.q + (c.s + (c.s&1)) / 2;
            return o;
        }
	
        public static CubeIndex EvenPointyToCube(OffsetIndex o){
            CubeIndex c;
            c.q = o.col - (o.row + (o.row&1)) / 2;
            c.s = o.row;
            c.r = -c.q - c.s;
            return c;
        }

        public static OffsetIndex CubeToOddPointy(CubeIndex c) {
            OffsetIndex o;
            o.row = c.s;
            o.col = c.q + (c.s - (c.s&1)) / 2;
            return o;
        }
	
        public static CubeIndex OddPointyToCube(OffsetIndex o){
            CubeIndex c;
            c.q = o.col - (o.row - (o.row&1)) / 2;
            c.s = o.row;
            c.r = -c.q - c.s;
            return c;
        }

        public static Tile operator+ (Tile one, Tile two){
            Tile ret = new Tile();
            ret.Index = one.Index + two.Index;
            return ret;
        }

        public void LineColour(Color colour) {
            LineRenderer lines = GetComponent<LineRenderer>();
            if(lines)
                lines.SetColors(colour, colour);
        }

        public void LineColour(Color start, Color end){
            LineRenderer lines = GetComponent<LineRenderer>();
            if(lines)
                lines.SetColors(start, end);
        }

        public void LineWidth(float width){
            LineRenderer lines = GetComponent<LineRenderer>();
            if(lines)
                lines.SetWidth(width, width);
        }

        public void LineWidth(float start, float end){
            LineRenderer lines = GetComponent<LineRenderer>();
            if(lines)
                lines.SetWidth(start, end);
        }
        #endregion

        #region A* Herustic Variables
        public int MoveCost { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get { return GCost + HCost; } }
        public Tile Parent { get; set; }
        #endregion
    }

    [System.Serializable]
    public struct OffsetIndex {
        public int row;
        public int col;

        public OffsetIndex(int row, int col){
            this.row = row; this.col = col;
        }
    }

    [System.Serializable]
    public struct CubeIndex {
        public int q;
        public int r;
        public int s;

        public CubeIndex(int q, int r, int s){
            this.q = q; this.r = r; this.s = s;
        }

        public CubeIndex(int q, int s) {
            this.q = q; this.s = s; this.r = -q-s;
        }

        public static CubeIndex operator+ (CubeIndex one, CubeIndex two){
            return new CubeIndex(one.q + two.q, one.r + two.r, one.s + two.s);
        }

        public override bool Equals (object obj) {
            if(obj == null)
                return false;
            CubeIndex o = (CubeIndex)obj;
            if((System.Object)o == null)
                return false;
            return((q == o.q) && (r == o.r) && (s == o.s));
        }

        public override int GetHashCode () {
            return(q.GetHashCode() ^ (r.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5))/2) + (q.GetHashCode() << 6) + (q.GetHashCode() >> 2)));
        }
    

        public override string ToString () {
            return string.Format("[" + q + "," + r + "," + s + "]");
        }


    }
}