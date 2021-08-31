using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBlockController : MonoBehaviour
{
    public Animator LockBlockAnimator;
    public ParticleSystem UnlockParticle;
    private BoxCollider2D bc2d;

    // Start is called before the first frame update
    void Start()
    {
        bc2d = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnlockBlock()
    {
        bc2d.isTrigger = true;
        AnimationManager.PlayAnimation(LockBlockAnimator, "lock_disappear");
    }

    public void DisappearFinished()
    {
        ParticlesManager.InstantiateParticule(UnlockParticle, transform.position);
        Destroy(gameObject);
    }
}
