using System;
using System.Collections;
using System.Collections.Generic;
using Map;
using Champions.ChampionsUtilities.Interfaces;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BattleManagement;
using UnityEngine.Analytics;

namespace Champions.CharacterUtilities.Movements
{
    public class HexMovement : MonoBehaviour, IMovement
    {
        public float RotationSpeed = 5;
        public float Speed = 10;
        public bool Moving = false;

        protected Tile currentTile { set; get; }

        private float orientation_;
        private Tile destinationTile_;
        private List<Tile> route_ = new List<Tile>();
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        private Map.Map map_ = Map.Map.Instance;
        private bool move_;
         

        public float Orientation
        {
            get { return orientation_; }
            set
            {
                orientation_ = value;
                transform.localRotation = Quaternion.Euler(0f, value, 0f);
            }
        }

        public Tile DestinationTile
        {
            get { return destinationTile_; }
            set
            {
                destinationTile_ = value;
                CalculateRoute(1);
            }
        }

        public void GoToDestination()
        {

            if (route_==null || route_.Count==0)
            {
                return;
            }
            Moving = true;
            StartCoroutine(goToDestination());
        }

        IEnumerator goToDestination()
        {
            yield return Move();
            TurnManagement.Instance.NextTurn();
        }


        public IEnumerator Move()
        {
            //yield return new WaitUntil(() => !Moving);
            Vector3 pointA, pointB, pointC = route_[0].Position;
            yield return LookAtIEnumerator(route_[1].Position);
            float t = Time.deltaTime * Speed;
            Tile currentTravelLocation;
            for (int i = 1; i < route_.Count; i++)
            {
                
                currentTravelLocation = route_[i];
                pointA = pointC;
                pointB = route_[i - 1].Position;
                pointC = (pointB + currentTravelLocation.Position) * 0.5f;
               
                for (; t < 1f; t += Time.deltaTime * Speed)
                {
                    transform.localPosition = Bezier.GetPoint(pointA, pointB, pointC, t);
                    Vector3 d = Bezier.GetDerivative(pointA, pointB, pointC, t);
                    d.y = 0f;
                    transform.localRotation = Quaternion.LookRotation(d);
                    //Debug.Log(Time.deltaTime);
                    yield return null;
                }
                t -= 1f;
            }
            currentTravelLocation = null;

            pointA = pointC;
            pointB = currentTile.Position;
            pointC = pointB;
            
            for (; t < 1f; t += Time.deltaTime * Speed)
            {
                transform.localPosition = Bezier.GetPoint(pointA, pointB, pointC, t);
                Vector3 d = Bezier.GetDerivative(pointA, pointB, pointC, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Moving = false;
        }

        public void LookAt(Vector3 point)
        {
            StartCoroutine(LookAtIEnumerator(point));
        }

        public IEnumerator LookAtIEnumerator(Vector3 point)
        {
            point.y = transform.localPosition.y;
            Quaternion fromRotation = transform.localRotation;
            Quaternion toRotation =
                Quaternion.LookRotation(point - transform.localPosition);
            float angle = Quaternion.Angle(fromRotation, toRotation);

            if (angle > 0f)
            {
                float speed = RotationSpeed / angle;
                for (
                    float t = Time.deltaTime * speed;
                    t < 1f;
                    t += Time.deltaTime * speed
                )
                {
                    transform.localRotation =
                        Quaternion.Slerp(fromRotation, toRotation, t);
                    yield return null;
                }
            }

            transform.LookAt(point);
            orientation_ = transform.localRotation.eulerAngles.y;
        }

        protected void CalculateRoute(int kindOfAlgorithm)
        {
            
            route_.Clear();
                                
            openList.Clear();
            closedList.Clear();
                        
            Tile tempCurrentTile = currentTile;

            switch (kindOfAlgorithm)
            {
                case 1:
                    tempCurrentTile.Cost = 0;
                    tempCurrentTile.MovementCost = 0;
                    tempCurrentTile.DistanceToTarget = Vector3.Distance(tempCurrentTile.Position, DestinationTile.Position);
                        
                    openList.Add(tempCurrentTile);
                    bool searching = true;
            
                    
                    //NIESKONCZONA PETLA - TRZEBA ZAMIENIAC TILE ZAMIAST DODAWAC NOWE! (CHYBA)
                    while (openList.Any() && searching)
                    {
                        
                        Tile bestTile = openList[0];
                        
                        foreach (var elem in openList)
                        {
                            if (elem.Cost < bestTile.Cost) bestTile = elem;
                        }

                        foreach (var elem in openList)
                        {
                            Debug.Log("open list: " + elem.Position);
                        }
                        Debug.Log("best tile: " + bestTile.Position);
                        Debug.Log("tile: " + openList.Find(x=>x.Position == bestTile.Position));
                        Debug.Log("indeks: " + openList.IndexOf(openList.Find(x=>x.Position == bestTile.Position)));
                        openList.RemoveAt(openList.IndexOf(openList.Find(x=>x.Position == bestTile.Position)));

                        List<Tile> neighbours = map_.AvailableNeighbours(bestTile);
            
                        foreach (var elem in neighbours)
                        {
                            elem.Parent = bestTile;

                            if (elem.Position == DestinationTile.Position)
                            {
                                searching = false;
                                break;
                            }
            
                            elem.MovementCost = bestTile.Drag / 2 + elem.Drag / 2;
                            elem.DistanceToTarget = Vector3.Distance(elem.Position, DestinationTile.Position);
            
                            elem.Cost = elem.MovementCost + elem.DistanceToTarget;

                            if (openList.Find(x => x.Position == elem.Position) != null)
                            {
                                if (openList.Find(x=>x.Position == elem.Position).Cost < elem.Cost) break;
                            }
                            
                            if (closedList.Find(x => x.Position == elem.Position) != null)
                            {
                                if (closedList.Find(x=>x.Position == elem.Position).Cost < elem.Cost) break;
                            }
                            else openList.Add(elem);
                        }
                        
                        Debug.Log("Searching: " + searching);
                        Debug.Log("any: " + openList.Any());
                        
                        if (!searching) break;
                        
                        closedList.Add(bestTile);
                    }


                    foreach (var elem in closedList)
                    {
                        Debug.Log("closedlist item: " + elem.Position);
                    }
                    Tile destination = DestinationTile;
                    Debug.Log("destination: " + destination.Position);
                    Debug.Log("current: " + currentTile.Position);
                    
                    while (destination.Position != currentTile.Position)
                    {
                        destination = map_.GetTile(destination.Coordinate);
                        route_.Add(destination);
                        destination = destination.Parent;
                        
                        Debug.Log("parent: " + destination);
                    }
                    
                    route_.Add(currentTile);
                    route_.Reverse();

                    break;
                    
                    
                case 2:
                    route_.Add(tempCurrentTile);
                    while (tempCurrentTile != DestinationTile)
                    {
                        var list = SortList(tempCurrentTile, DestinationTile);

                        var position = 0;
                        while (route_.Contains(list[position]))
                        {
                            position++;
                        }
                        route_.Add(list[position]);
                        tempCurrentTile = list[position];
                    }

                    break;                
            }


            foreach (var elem in route_)
            {
                Debug.Log(elem);
            }
            Debug.Log("Route calculated");
        }

        private List<Tile> SortList(Tile center, Tile endPoint)
        {

            var list = map_.AvailableNeighbours(center);
            var i = 1;
            while (i < list.Count)
            {
                var j = i;
                while (j > 0 && Vector3.Distance(list[j - 1].Position, endPoint.Position) > Vector3.Distance(list[j].Position, endPoint.Position))
                {
                    var temp = list[j];
                    list[j] = list[j - 1];
                    list[j - 1] = temp;

                    j--;
                }
                i++;
            }
            return list;
        }

    }
}
