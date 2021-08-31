using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Capacity { BLOCK = 0}

public class GameController : MonoBehaviour
{
    private Vector3 m_lastCheckpointPosition = new Vector3(0, 0, -1);

    public static bool IsPaused
    {
        get { return Time.timeScale == 0;  }
    }

    private static bool m_bIsFinished = false;
    public static bool IsFinished
    {
        get { return m_bIsFinished; }
    }

    private static bool m_bIsDead = false;
    public static bool IsDead
    {
        get { return m_bIsDead; }
    }

    public GameObject MenuCanvas;

    private IEnumerator coroutine;
    
    public string SceneName
    {
        get
        {
            return SceneManager.GetActiveScene().name;
        }
    }

    private Level m_lvlCurrentLevel;

    public float m_yDeath;
    public float m_xDeath;

    /*Particules*/
    public ParticleSystem checkpointParticles;
    public ParticleSystem victoryParticles;

    private int capacity_block_left;

    private bool bNoCheckPoint;
    private bool bNoNextLevelAll;
    private bool bNoNextLevelPauseDeath;

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.InitLevelManager();
        m_lvlCurrentLevel = LevelManager.GetLevel(SceneManager.GetActiveScene().name);

        MenuController mController = MenuCanvas.GetComponent<MenuController>();
        ResumeGame(false);
        mController.HideEndMenu();
        mController.HidePauseMenu();
        mController.HideDeadMenu();
        m_bIsFinished = false;
        m_bIsDead = false;

        capacity_block_left = m_lvlCurrentLevel.CapacityBlockCount;

        Transform tHUD = MenuCanvas.transform.Find("HUD");

        Transform tPanelBlock = tHUD.Find("pnlBlock");

        if (m_lvlCurrentLevel.CapacityBlock == true)
        {
            tPanelBlock.gameObject.SetActive(true);
            Transform tText = tPanelBlock.Find("capacity_block_text");

            Text tNbBlock = tText.gameObject.GetComponent<Text>();
            tNbBlock.text = m_lvlCurrentLevel.CapacityBlockCount.ToString();
        }

        if (m_lvlCurrentLevel.CapacityGravity)
        {
            
            Transform tPanelGravity = tHUD.Find("pnlGravity");
            tPanelGravity.gameObject.SetActive(true);
        } 

        GameObject[] blcTeleport = GameObject.FindGameObjectsWithTag("TeleportBlock");

        if(blcTeleport.Length > 0)
        {
            Transform tPanelTeleport = tHUD.Find("pnlTeleport");
            tPanelTeleport.gameObject.SetActive(true);
        }

        if (m_lvlCurrentLevel.Tutoriel != MenuController.Tutoriel.Aucun)
        {
            MenuController script = MenuCanvas.GetComponent<MenuController>();
            script.DrawTuto(m_lvlCurrentLevel.Tutoriel);
        }
        
        GameObject[] chkPoints = GameObject.FindGameObjectsWithTag("CheckpointBlock");

        Transform tPauseMenu = MenuCanvas.transform.Find("PauseMenu");
        Transform tDiedMenu = MenuCanvas.transform.Find("DiedMenu");
        Transform tEndMenu = MenuCanvas.transform.Find("EndMenu");

        Transform tUI_TextLevelEnd = tEndMenu.Find("txtLevel");
        Transform tUI_TextLevelPause = tPauseMenu.Find("txtLevel");
        Transform tUI_TextLevelDeath = tDiedMenu.Find("txtLevel");

        tUI_TextLevelEnd.gameObject.GetComponent<Text>().text = "Level " + (m_lvlCurrentLevel.Index + 1);
        tUI_TextLevelPause.gameObject.GetComponent<Text>().text = "Level " + (m_lvlCurrentLevel.Index + 1);
        tUI_TextLevelDeath.gameObject.GetComponent<Text>().text = "Level " + (m_lvlCurrentLevel.Index + 1);

        GameSave gs = FileManager.GetGameSave();

        if (chkPoints.Length == 0)
        {
            Transform tUI_CheckpointPause = tPauseMenu.Find("Text_Checkpoint");
            Transform tUI_CheckpointDeath = tDiedMenu.Find("Text_Checkpoint");

            tUI_CheckpointPause.gameObject.SetActive(false);
            tUI_CheckpointDeath.gameObject.SetActive(false);

            bNoCheckPoint = true;
        }

        Transform tUI_NextLevelEnd = tEndMenu.Find("Text_NextLevel");
        Transform tUI_NextLevelPause = tPauseMenu.Find("Text_NextLevel");
        Transform tUI_NextLevelDeath = tDiedMenu.Find("Text_NextLevel");

        if (m_lvlCurrentLevel.Index > gs.LastLevelCleared - 1)
        {
            tUI_NextLevelPause.gameObject.SetActive(false);
            tUI_NextLevelDeath.gameObject.SetActive(false);

            bNoNextLevelPauseDeath = true;
        }

        if (m_lvlCurrentLevel.Index == LevelManager.listeLevels.Count - 1)
        {
            tUI_NextLevelPause.gameObject.SetActive(false);
            tUI_NextLevelDeath.gameObject.SetActive(false);
            tUI_NextLevelEnd.gameObject.SetActive(false);

            bNoNextLevelAll = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !m_bIsDead)
        {
            MenuController mController = MenuCanvas.GetComponent<MenuController>();
            if (!IsPaused)
            {
                PauseGame();
                mController.ShowPauseMenu();
            }
            else
            {
                ResumeGame();
                mController.HidePauseMenu();
            }
        }

        if (m_bIsFinished)
        {
            if (Input.GetKeyDown(KeyCode.R))//Retry
            {
                RestartLevel();
            }

            if (Input.GetKeyDown(KeyCode.N) && !bNoNextLevelAll)//Next level
            {
                LoadNextLevel();
            }
        }

        if (IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.R))//Retry
            {
                RestartLevel();
            }

            if (Input.GetKeyDown(KeyCode.N) && !bNoNextLevelPauseDeath)//Next level
            {
                LoadNextLevel();
            }

            if (Input.GetKeyDown(KeyCode.C) && !bNoCheckPoint)//Next level
            {
                ResumeGame();
                MenuController mController = MenuCanvas.GetComponent<MenuController>();
                mController.HidePauseMenu();

                LoadLastCheckpoint();
            }
        }

        if (m_bIsDead)
        {
            if (Input.GetKeyDown(KeyCode.R))//Retry
            {
                RestartLevel();
            }

            if (Input.GetKeyDown(KeyCode.N) && !bNoNextLevelPauseDeath)//Next level
            {
                LoadNextLevel();
            }

            if (Input.GetKeyDown(KeyCode.C) && !bNoCheckPoint)//Next level
            {
                PlayerRespawn();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }

    void ResumeGame(bool test = true)
    {
        Time.timeScale = 1;

        /*if (test)
        {
            coroutine = WaitAndTest();
            StartCoroutine(coroutine);
        }*/
    }

    public void PlayerDied()
    {
        PauseGame();
        m_bIsDead = true;
        MenuController mController = MenuCanvas.GetComponent<MenuController>();
        mController.ShowDeadMenu();
    }

    public void PlayerRespawn()
    {
        ResumeGame();
        m_bIsDead = false;
        MenuController mController = MenuCanvas.GetComponent<MenuController>();
        mController.HideDeadMenu();

        LoadLastCheckpoint();
    }

    public void FinishLevel()
    {
        GameObject goVictory = GameObject.FindGameObjectWithTag("VictoryBlock");

        Vector3 vPosition = goVictory.transform.position;
        vPosition.y += 1.5f;

        ParticlesManager.InstantiateParticule(victoryParticles, vPosition);

        m_bIsFinished = true;

        MenuController mController = MenuCanvas.GetComponent<MenuController>();
        PauseGame();
        mController.ShowEndMenu();

        GameSave gSave = FileManager.GetGameSave();
        gSave.LastLevelCleared++;
        FileManager.SaveGameSave(gSave);
    }

    private IEnumerator WaitAndTest()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);

            FinishLevel();
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneName);
    }

    void LoadNextLevel()
    {
        Level lvlNext = LevelManager.GetNextLevel(this.m_lvlCurrentLevel);

        string szScneneName = lvlNext.SceneName;
        SceneManager.LoadScene(szScneneName);
    }

    public void UpdateLastCheckpoint(GameObject go)
    {
        if (m_lastCheckpointPosition.z < 0 || go.transform.position.x > m_lastCheckpointPosition.x)
        {
            m_lastCheckpointPosition = go.transform.position;
            m_lastCheckpointPosition.y += 1.02f;

            Vector3 vPositionParticules = go.transform.position;
            vPositionParticules.y += 1.7f;

            ParticlesManager.InstantiateParticule(checkpointParticles, vPositionParticules);
        }
    }

    public void LoadLastCheckpoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (m_lastCheckpointPosition.z > 0)
        {
            player.transform.position = new Vector3(m_lastCheckpointPosition.x, m_lastCheckpointPosition.y, player.transform.position.z);
        }
        else
        {
            RestartLevel();
        }
    }

    public void CapacityBlockUse()
    {
        capacity_block_left--;

        if(this.m_lvlCurrentLevel.CapacityBlock)
            UpdateHUD(Capacity.BLOCK);
    }

    public int GetCapacityBlockLeft()
    {
        return capacity_block_left;
    }

    public void UpdateHUD(Capacity cap)
    {
        if(cap == Capacity.BLOCK)
        {
            Transform tHUD = MenuCanvas.transform.Find("HUD");
            Transform tPanelBlock = tHUD.Find("pnlBlock");
            Transform tText = tPanelBlock.Find("capacity_block_text");

            Text txtBlock = tText.gameObject.GetComponent<Text>();
            txtBlock.text = capacity_block_left.ToString();
        }
    }

    public void PlayerGetKey()
    {
        Transform tHUD = MenuCanvas.transform.Find("HUD");
        Transform tKey = tHUD.Find("key_image");
        tKey.gameObject.SetActive(true);
    }

    public void PlayerLoseKey()
    {
        Transform tHUD = MenuCanvas.transform.Find("HUD");
        Transform tKey = tHUD.Find("key_image");
        tKey.gameObject.SetActive(false);
    }

    public bool IsGravityAllowed()
    {
        return m_lvlCurrentLevel.CapacityGravity;
    }

}
