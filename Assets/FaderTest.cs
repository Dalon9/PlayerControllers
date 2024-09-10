using System.Collections;
using System.Collections.Generic;
using S8l.ScreenFader.Runtime;
using UnityEngine;

public class FaderTest : MonoBehaviour
{
    public float Duration;
    public LWRPScreenFade VRScreenFader;
    private bool _faded;

    public void Fade()
    {
        ScreenFader.Instance.JustFadeInAndOut(2f, 2f);
    }

    public void ToogleFade()
    {
        if (_faded)
        {
            ScreenFader.Instance.FadeOut(Duration);
        }
        else
        {
            ScreenFader.Instance.FadeIn(Duration);
        }
        _faded = !_faded;
    }

    public void VRBlink()
    {
        VRScreenFader.AppliedDuration = 2f;
        VRScreenFader.Blink();
    }
}
