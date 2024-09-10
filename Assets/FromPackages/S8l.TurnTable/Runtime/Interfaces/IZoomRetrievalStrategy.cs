namespace S8l.TurnTable.Runtime.Interfaces
{
    public interface IZoomRetrievalStrategy
    {
        

        void Init(ITurnTable parent);
        
        float OnUpdate();
        
    }
}