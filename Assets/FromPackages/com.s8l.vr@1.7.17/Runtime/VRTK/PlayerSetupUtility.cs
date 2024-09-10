using UnityEngine;
#if PLAYER_SETUP_PRESENT
using S8l.PlayerSetup.Runtime;
using VRTK;
using System;
#endif

public class PlayerSetupUtility : MonoBehaviour
{
#if PLAYER_SETUP_PRESENT
    private void Awake()
    {
        VRTK_SDKManager.instance.LoadedSetupChanged += LoadedSetupChanged;
    }

    private void OnDestroy()
    {
        VRTK_SDKManager.instance.LoadedSetupChanged -= LoadedSetupChanged;
    }

    private void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
    {
        if (sender != null && sender.loadedSetup != null)
        {
            var type = sender.loadedSetup.systemSDK.GetType();
            if (type == typeof(SDK_SimSystem))
            {
                Info.instance.playerSetupType = PlayerSetupType.VRSimulator;
            }
            else if (type == typeof(SDK_OculusSystem))
            {
                Info.instance.playerSetupType = PlayerSetupType.VROculus;
            }
        }
    }
#endif
}