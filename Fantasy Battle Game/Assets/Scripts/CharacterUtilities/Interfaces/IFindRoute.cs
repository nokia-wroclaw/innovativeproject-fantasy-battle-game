using System.Collections.Generic;
using Map;

namespace CharacterUtilities.Interfaces
{
    public interface IFindRoute
    {
        List<Tile> CalculateRoute(Tile startPoint, Tile endPoint);
    }
}