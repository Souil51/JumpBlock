using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoFileController : MonoBehaviour
{
    public static bool NoFileEnabled =  true;

    private GameSave m_GameSave;
    private GameSettings m_GameSettings;

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

    public void Start()
    {
        m_GameSave = new GameSave();
        m_GameSave.LastLevelCleared = 0;

        m_GameSettings = new GameSettings();
        m_GameSettings.SoundEnabled = true;
        m_GameSettings.SoundVolume = 0.25f;
    }

    public GameSave GetGameSave()
    {
        return m_GameSave;
    }

    public void SaveGameSave(GameSave gs)
    {
        m_GameSave = gs;
    }

    public GameSettings GetGameSettings()
    {
        return m_GameSettings;
    }

    public void SaveGameSettings(GameSettings gs)
    {
        m_GameSettings = gs;
    }
}
