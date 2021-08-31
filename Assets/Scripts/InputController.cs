using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private string HorizontalName = "Horizontal";
    private string VerticalName = "Vertical";

    private static bool bInit = false;

    public void Awake()
    {
        if (!bInit)
        {
            DontDestroyOnLoad(transform.gameObject);

            bInit = true;
        }
        else
        {
            Destroy(transform.gameObject);
        }
    }

    public void SetQWERTY()
    {
        HorizontalName = "Horizontal_Bis";
        VerticalName = "Vertical_Bis";
    }

    public void SetAZERTY()
    {
        HorizontalName = "Horizontal";
        VerticalName = "Vertical";
    }

    public string GetHorizontalAxis()
    {
        return HorizontalName;
    }

    public string GetVerticalAxis()
    {
        return VerticalName;
    }

}
