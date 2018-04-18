using System.Collections.Generic;
using Assets.Scripts.Map;

namespace CharacterUtilities.Interfaces
{
    public interface IFindRoute
    {
        List<Tile> CalculateRoute(Tile startPoint, Tile endPoint);
    }
}