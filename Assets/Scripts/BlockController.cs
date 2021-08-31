using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private float m_fOffsetDetection_Y = 0.05f;
    private float m_fOffsetDetection_X = 0.1f;
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != groundLayer.value / 32) return;

        if (isGrounded())
        {
            Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Static;

            this.gameObject.layer = 8;
        }
    }

    public bool isGrounded()
    {
        Vector2 positionRight = transform.position;
        positionRight.x += GetComponent<SpriteRenderer>().bounds.size.x / 2;
        positionRight.x -= m_fOffsetDetection_X;

        Vector2 positionLeft = transform.position;
        positionLeft.x -= GetComponent<SpriteRenderer>().bounds.size.x / 2;
        positionLeft.x += m_fOffsetDetection_X;

        Vector2 direction = Vector2.down;
        float distance = GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hitRight = Physics2D.Raycast(positionRight, direction, distance, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(positionLeft, direction, distance, groundLayer);

        bool bRes = false;

        if (hitRight.collider != null || hitLeft.collider != null)
        {
            bRes = true;
        }

        return bRes;
    }
}
