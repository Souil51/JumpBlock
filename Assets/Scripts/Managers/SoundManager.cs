using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static void MakeSound(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position, 1f);

    }

    public static void StartMainMusic()
    {
        GameObject.FindGameObjectWithTag("MainMusic").GetComponent<MainMusicController>().Play();
    }

    public static void StopMainMusic()
    {
        GameObject.FindGameObjectWithTag("MainMusic").GetComponent<MainMusicController>().Stop();
    }

    public static void SetMainMusicVolume(float fVolume)
    {
        GameObject.FindGameObjectWithTag("MainMusic").GetComponent<MainMusicController>().SetVolume(fVolume);
    }
}
