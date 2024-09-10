namespace S8l.RotZoomCam.Runtime.Interfaces
{
    public interface IRotStrategy
    {

        void Init(IRotZoomCam parent);
        
        void OnUpdate();
    }
}