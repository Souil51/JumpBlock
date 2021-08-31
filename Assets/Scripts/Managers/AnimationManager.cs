using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static void PlayAnimation(Animator animator, string szAnimationName)
    {
        animator.Play(szAnimationName);
    }
}
