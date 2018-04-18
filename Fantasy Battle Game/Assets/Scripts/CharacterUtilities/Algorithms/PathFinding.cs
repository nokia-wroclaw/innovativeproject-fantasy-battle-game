using System.Collections.Generic;
using Assets.Scripts.Map;
using CharacterUtilities.Interfaces;

namespace CharacterUtilities.Algorithms
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