using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    private static bool bInit = false;

    public void Awake()
    {
        if (!bInit)
        {
            DontDestroyOnLoad(transform.gameObject);
            Screen.SetResolution(1280, 720, false);

            bInit = true;
        }
        else
        {
            Destroy(transform.gameObject);
        }
    }
}
