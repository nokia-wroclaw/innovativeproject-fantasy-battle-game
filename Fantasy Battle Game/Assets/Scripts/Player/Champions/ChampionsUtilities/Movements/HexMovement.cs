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
        protected AnimationController animationController_;
        protected ChampionsManager championsManager_;
        
        private float orientation_;
        private Tile destinationTile_;
        private List<Tile> route_ = new List<Tile>();
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        private Map.Map map_ = Map.Map.Instance;

        public void Start()
        {
            animationController_ = AnimationController.Instance;
            championsManager_ = ChampionsManager.Instance;
        }
        
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
                map_.ClearCosts();
            }
        }

        public void GoToDestination()
        {

            if (championsManager_.SelectedChampion != null)
                animationController_.MoveAnimation(championsManager_.SelectedChampion);
            
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
            animationController_.RestAnimation(championsManager_.SelectedChampion);
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

        private double EstimateCost(Tile current, Tile destination)
        {
            var dx = Math.Abs(current.Coordinate.FirstCoord - destination.Coordinate.FirstCoord);
            var dy = Math.Abs(current.Coordinate.SecondCoord - destination.Coordinate.SecondCoord);

            return 1 * (dx + dy) + (Math.Sqrt(2) - 2 * 1) * Math.Min(dx, dy);
        }

        protected void CalculateRoute(int kindOfAlgorithm)
        {
            
            route_.Clear();
                                
            openList.Clear();
            closedList.Clear();

            switch (kindOfAlgorithm)
            {
                case 1:
                    
                    openList.Add(currentTile);
                    currentTile.GScore = 0;
                    currentTile.FScore = EstimateCost(currentTile, DestinationTile);

                    while (openList.Any())
                    {
                        
                        var bestTile = openList[0];
                        
                        foreach (var elem in openList)
                        {
                            if (elem.FScore < bestTile.FScore) bestTile = elem;
                        }

                        if (bestTile == DestinationTile) break;

                        openList.Remove(bestTile);
                        closedList.Add(bestTile);

                        List<Tile> neighbours = map_.AvailableNeighbours(bestTile);
            
                        foreach (var elem in neighbours)
                        {
                            if (closedList.Contains(elem)) continue;
                            if (!openList.Contains(elem))
                            {
                                openList.Add(elem);

                                var gScore = bestTile.GScore + bestTile.Drag / 2 + elem.Drag / 2;
                                
                                if(gScore >= elem.GScore) continue;

                                elem.Parent = bestTile;
                                elem.GScore = gScore;
                                elem.FScore = elem.GScore + EstimateCost(elem, DestinationTile);
                            }
                        }
                    }
                    
                    Tile destination = DestinationTile;
                    Debug.Log("Koszt celu: " + DestinationTile.FScore);
                    Debug.Log("Rodzic celu: " + DestinationTile.Parent);
                    
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
                    route_.Add(currentTile);
                    while (currentTile != DestinationTile)
                    {
                        var list = SortList(currentTile, DestinationTile);

                        var position = 0;
                        while (route_.Contains(list[position]))
                        {
                            position++;
                        }
                        route_.Add(list[position]);
                        currentTile = list[position];
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
