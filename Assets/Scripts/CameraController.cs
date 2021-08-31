using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    public Vector2 velocitySmooth;
    public float smoothTimeX;
    public float smoothTimeY;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocitySmooth.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocitySmooth.y, smoothTimeY);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
}
