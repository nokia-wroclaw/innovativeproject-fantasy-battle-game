using System.Collections.Generic;
using Assets.Scripts.Map;
using Champions.ChampionsUtilities.Interfaces;

namespace Champions.ChampionsUtilities.Algorithms
{
    public class PathFinding:IFindRoute
    {
        private List<Tile> solution_;
        
        public List<Tile> CalculateRoute(Tile startPoint, Tile endPoint)
        {
            return null;
        }
    }
}