using System.Collections.Generic;
using UnityEngine;

namespace Map.Interfaces
{
    public interface ITile
    {
        /// <summary>
        /// Gets or sets the distance from start. Used 
        /// </summary>
        /// <returns>The distance from start</returns>
        double DistanceFromStart { get; set; }

        /// <summary>
        /// Gets or sets position of tile in space
        /// </summary>
        /// <returns>position</returns>
        Vector3 Position { get; }

        /// <summary>
        /// Gets or sets drag- price of move.
        /// </summary>
        /// <returns>The drag, price of move</returns>
        double Drag { get; set; }

        /// <summary>
        /// Gets or sets availability to stand at tile. 
        /// </summary>
        /// <returns>true if champion can stand at tile; otherwise, false</returns>
        bool Available { get; set; }

        /// <summary>
        /// Gets or sets the hex coordinate
        /// </summary>
        /// <value>hex coordinate</value>
        TileMetrics.HexCoordinate Coordinate { get; set; }
        /// <summary>
        /// Deletes childs game objects i.e. label with distance from start and projector
        /// </summary>
        void DeleteChildsGO();
        /// <summary>
        /// Gets the neighbours list- 6 or less tiles next to current tile
        /// </summary>
        /// <returns>Tiles next to current tile</returns>
        List<Tile> GetNeighbours();
    }
}