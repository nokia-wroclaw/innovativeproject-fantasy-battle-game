using System.Collections.Generic;
using Map;

namespace Champions.ChampionsUtilities.Interfaces
{
    public interface IFindRoute
    {
        List<Tile> CalculateRoute(Tile startPoint, Tile endPoint);
    }
}