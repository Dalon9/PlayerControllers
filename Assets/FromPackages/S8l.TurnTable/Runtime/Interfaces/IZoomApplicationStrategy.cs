namespace S8l.TurnTable.Runtime.Interfaces
{
    public interface IZoomApplicationStrategy
    {
        void Init(ITurnTable parent);

        void ApplyZoom(float delta);
    }
}