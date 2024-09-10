namespace S8l.RotZoomCam.Runtime.Interfaces
{
    public interface IZoomStrategy
    {
        

        void Init(IRotZoomCam parent);
        
        void OnUpdate();
        
    }
}