
using System.Collections;
using Map;
using UnityEngine;

namespace Champions.ChampionsUtilities.Interfaces
{
    public interface IMovement
    {
        Tile DestinationTile
        {
            get;
            set;
        }

        void GoToDestination();
        //IEnumerator Move();
        //IEnumerator LookAt(Vector3 point);
    }
}