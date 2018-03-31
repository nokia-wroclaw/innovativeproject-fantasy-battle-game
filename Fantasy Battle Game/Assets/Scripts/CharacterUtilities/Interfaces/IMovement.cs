using Map;

namespace CharacterUtilities.Interfaces
{
    public interface IMovement
    {
        void SetTargetPosition(Tile tile);
        void Move();
    }
}