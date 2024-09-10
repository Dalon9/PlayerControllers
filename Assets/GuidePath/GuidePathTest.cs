using S8l.GuidePath.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidePathTest : MonoBehaviour
{
    public GuidePath GP;
    public Transform Player, Target;


    void Update()
    {
        GP.StartShowing(Player, Target);
    }
}