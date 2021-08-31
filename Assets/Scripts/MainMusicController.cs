using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMusicController : MonoBehaviour
{
    private static AudioSource m_audioSource;

    private static bool bInit = false;

    public void Awake()
    {
        if (!bInit)
        {
            DontDestroyOnLoad(transform.gameObject);
            m_audioSource = GetComponent<AudioSource>();

            bInit = true;
        }
        else
        {
            Destroy(transform.gameObject);
        }
    }

    public void Play()
    {
        if (m_audioSource.isPlaying) return;

        m_audioSource.Play();
    }

    public void Stop()
    {
        m_audioSource.Stop();
    }

    public void SetVolume(float fVolume)
    {
        m_audioSource.volume = fVolume * 0.25f;
    }
}
