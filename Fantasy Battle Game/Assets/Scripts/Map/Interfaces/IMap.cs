﻿using System.Collections.Generic;

namespace Assets.Scripts.Map.Interfaces
{
    public interface IMap
    {
        /// <summary>
        /// Gets tiles in range from selected tile
        /// </summary>
        /// <param name="center">center tile</param>
        /// <param name="range">range</param>
        /// <returns>dictionary with tiles in range. Key- coordinate, value- tile</returns>
        Dictionary<TileMetrics.HexCoordinate, Tile> TilesInRange(Tile center, double range);

        /// <summary>
        /// Gets list of available tiles next to selected tile
        /// </summary>
        /// <param name="center">current tile</param>
        /// <returns>list of available tiles</returns>
        List<Tile> AvailableNeighbours(Tile center);
        
        /// <summary>
        /// Gets or sets all tiles in map.
        /// </summary>
        /// <returns>dictionary with all tiles</returns>
        Dictionary<TileMetrics.HexCoordinate, Tile> Tiles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the tiles in range.
        /// </summary>
        /// <returns>dictionary with all tiles in range</returns>
        Dictionary<TileMetrics.HexCoordinate, Tile> TilesInRangeDictionary { get; }

        /// <summary>
        /// Method return random positions
        /// </summary>
        /// <param name="amountofCreaturesFirstPlayer"></param>
        /// <param name="amountofCreaturesSecondPlayer"></param>
        /// <returns>Key-HexCoordinate</para>
        ///  Value-0 if creature was owned by first player,  1 if creature was owned by first player
        ///  </returns>
        List<KeyValuePair<TileMetrics.HexCoordinate, int>> RandomPositions(int amountofCreaturesFirstPlayer,
            int amountofCreaturesSecondPlayer);

    }
}