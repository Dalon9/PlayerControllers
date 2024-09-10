using S8l.Guardian.Runtime.Interfaces;
using UnityEngine;

namespace S8l.Guardian.Runtime
{
    public class GuardianArea : MonoBehaviour
    {
        public Transform Player;
        private IGuardianShapeProvider _shapeProvider;
        private IGuardianEffectProvider[] _effectProviders;

        private void Awake()
        {
            _shapeProvider = GetComponent<IGuardianShapeProvider>();
            _effectProviders = GetComponents<IGuardianEffectProvider>();

            if (_shapeProvider != null && _effectProviders != null) return;

            Debug.LogError("No Play Area Shader or Effect Provider found on the Guardian. Disabling guardian");
            enabled = false;
        }

        private void Update()
        {
            // Sample the position
            var pos = _shapeProvider.SamplePosition(Player.position);
            // Play the effects    
            foreach (var effect in _effectProviders) effect.PlayEffect(pos);
        }
    }
}