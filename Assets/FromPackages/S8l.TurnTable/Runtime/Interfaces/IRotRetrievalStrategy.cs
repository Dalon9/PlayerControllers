using UnityEngine;

namespace S8l.TurnTable.Runtime.Interfaces
{
    public interface IRotRetrievalStrategy
    {

        void Init(ITurnTable parent);
        
        Quaternion OnUpdate();
    }
}