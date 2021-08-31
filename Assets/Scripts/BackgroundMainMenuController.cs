using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMainMenuController : MonoBehaviour
{
    private float fDistanceTreshhold;

    // Start is called before the first frame update
    void Start()
    {
        float fPosXTest = Camera.main.pixelWidth;
        Vector3 vLast = Camera.main.ScreenToWorldPoint(new Vector3(fPosXTest, 0, 0));

        fDistanceTreshhold = vLast.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.transform.position.x - transform.position.x > 3 * (fDistanceTreshhold + 1))
            Destroy(this.gameObject);
    }
}
