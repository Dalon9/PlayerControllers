using UnityEngine;

namespace S8l.TurnTable.Runtime.Interfaces
{
    public interface IRotApplicationStrategy
    {
        void Init(ITurnTable parent);
        
        void ApplyRotation(Quaternion delta);
    }
}