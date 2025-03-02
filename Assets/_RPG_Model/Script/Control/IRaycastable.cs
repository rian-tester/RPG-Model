
namespace RPG.Control
{
    // Only for target combat and items that can be pickup
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController callingController);
    }
}
