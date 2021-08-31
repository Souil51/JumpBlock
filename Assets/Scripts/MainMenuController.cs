using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject LevelSelectionMenu;
    public GameObject OptionPanel;

    private int nCurrentPage = 0;

    // Start is called before the first frame update
    public void Start()
    {
        GameObject goInput = GameObject.FindGameObjectWithTag("InputController");
        InputController inputCtrl = goInput.GetComponent<InputController>();

        if(inputCtrl.GetHorizontalAxis() == "Horizontal")
        {
            UpdateTextLayoutAZERTY();
        }
        else
        {
            UpdateTextLayoutQWERTY();
        }

        MainMenuPanel.SetActive(true);
        LevelSelectionMenu.SetActive(false);
        OptionPanel.SetActive(false);

        GameSettings gs = FileManager.GetGameSettings();

        if (gs.SoundEnabled)
        {
            SoundManager.SetMainMusicVolume(gs.SoundVolume);
            SoundManager.StartMainMusic();
        }
        else
        {
            SoundManager.StopMainMusic();
        }
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    #region Main Menu

    public void MainMenu_SelectLevel()
    {
        MainMenuPanel.SetActive(false);
        LevelSelectionMenu.SetActive(true);

        LoadLevels();
    }

    public void MainMenu_Options()
    {
        MainMenuPanel.SetActive(false);
        OptionPanel.SetActive(true);

        LoadGameSettings();
    }

    public void MainMenu_Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Level Selection

    public void LevelSelection_Back()
    {
        LevelSelectionMenu.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public static void LoadLevel(int nScene)
    {
        SceneManager.LoadScene(nScene);
    }

    public void GoToNextPage()
    {
        nCurrentPage++;
        LoadLevels();
    }

    public void GoToPreviousPage()
    {
        nCurrentPage--;
        LoadLevels();
    }

    public void LoadLevels()
    {
        ClearPanelLevelSelection();

        List<string> listeScenes = new List<string>();
        LevelManager.InitLevelManager();

        foreach(Level lvl in LevelManager.listeLevels)
        {
            listeScenes.Add(lvl.SceneName);
        }

        Transform tLevelSelectionMenu = this.gameObject.transform.Find("LevelSelectionMenu");
        Transform tPanel = tLevelSelectionMenu.transform.Find("Panel");

        RectTransform rectTrans = tPanel.GetComponent<RectTransform>();
        float canvasWidth = rectTrans.rect.width - 150;//-150 pour la place des buttons "<" et ">"
        float canvasHeight = rectTrans.rect.height;

        GameObject goButtonTest = (GameObject)Resources.Load("btnLevel");

        RectTransform rtButton = goButtonTest.GetComponent<RectTransform>();

        int nbColonnesLevels = 4;// (int)(canvasWidth / (rtButton.rect.width + 50));
        int nbLignesLevels = 2;// (int)((canvasHeight - 150) / (rtButton.rect.height + 50));

        float colWidth = canvasWidth / nbColonnesLevels;
        float linHeight = (canvasHeight - 200) / nbLignesLevels;

        int nColIndex = 0;
        int nLinIndex = 0;

        string[] arrayLevels = listeScenes.ToArray();
        Level[] arrayLevelObj = LevelManager.listeLevels.ToArray();

        int nOffsetDebut = nCurrentPage * (nbColonnesLevels * nbLignesLevels);

        int nLimites = 0;
        bool bLastPage = false;

        if (nOffsetDebut + (nbColonnesLevels * nbLignesLevels) > arrayLevelObj.Length)
        {
            nLimites = arrayLevelObj.Length;
            bLastPage = true;
        }
        else
            nLimites = nOffsetDebut + (nbColonnesLevels * nbLignesLevels);

        Transform tButtonPrevious = tPanel.Find("btnPreviousPage");
        Transform tButtonNext = tPanel.Find("btnNextPage");

        if (nCurrentPage == 0)
            tButtonPrevious.gameObject.SetActive(false);
        else
            tButtonPrevious.gameObject.SetActive(true);

        if (bLastPage)
            tButtonNext.gameObject.SetActive(false);
        else
            tButtonNext.gameObject.SetActive(true);

        GameSave gSave = FileManager.GetGameSave();

        for (int i = nOffsetDebut; i < nLimites; i++)
        {
            Level currentLevel = arrayLevelObj[i];

            string[] szSplit = currentLevel.SceneName.Split(new char[] { '_' });

            int nLevelIndex = int.Parse(szSplit[1]);

            GameObject goButton = (GameObject)Instantiate(Resources.Load("btnLevel"));

            Button btnObject = goButton.GetComponent<Button>();

            if (nLevelIndex <= gSave.LastLevelCleared + 1)
                btnObject.interactable = true;

            bool bLevelHasBlockCapacity = currentLevel.CapacityBlock;

            for (int j = 0; j < goButton.transform.childCount; j++)
            {
                GameObject goCurrentChild = goButton.transform.GetChild(j).gameObject;

                if (bLevelHasBlockCapacity && goCurrentChild.tag == "CapacityBlockImage")
                {
                    Transform transform = goCurrentChild.transform.GetChild(0);
                    Text textChild = (Text)transform.gameObject.GetComponent<Text>();
                    textChild.text = currentLevel.CapacityBlockCount.ToString();

                    goCurrentChild.SetActive(true);
                }

                if(currentLevel.CapacityGravity && goCurrentChild.tag == "CapacityGravityImage")
                {
                    goCurrentChild.SetActive(true);
                }
            }

            goButton.transform.SetParent(tPanel);

            Transform tText = goButton.transform.Find("Text");
            Text btnText = tText.GetComponent<Text>();
            btnText.text = "Level " + nLevelIndex;

            float nPos_X = nColIndex * colWidth;
            float nPos_Y = nLinIndex * linHeight;

            float nOffset_X = (canvasWidth / 2) - (colWidth / 2);
            float nOffset_Y = ((canvasHeight - 100) / 2) - (linHeight / 2); ;

            Vector3 vPosButton = new Vector3(nPos_X - nOffset_X, -nPos_Y + nOffset_Y, 0);
            goButton.transform.localPosition = vPosButton;

            nColIndex = (nColIndex + 1) % nbColonnesLevels;

            if (nColIndex == 0)
                nLinIndex = (nLinIndex + 1) % nbLignesLevels;

            goButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(nLevelIndex));
        }

    }

    private void ClearPanelLevelSelection()
    {
        Transform tLevelSelectionMenu = this.gameObject.transform.Find("LevelSelectionMenu");
        Transform tPanel = tLevelSelectionMenu.transform.Find("Panel");

        for (int i = 0; i < tPanel.transform.childCount; i++)
        {
            Transform child = tPanel.transform.GetChild(i);

            if (child.name.Contains("(Clone)"))
                Destroy(child.gameObject);
        }
    }

    #endregion

    #region Options

    public void Options_Back()
    {
        OptionPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void Options_Apply()
    {
        GameSettings gs = new GameSettings();

        Transform tOptionPanel = this.gameObject.transform.Find("OptionPanel");
        Transform tPanel = tOptionPanel.transform.Find("Panel");

        Transform tToggleSoundEnabled = tPanel.Find("ToggleSoundEnabled");
        Toggle tSoundEnabled = tToggleSoundEnabled.GetComponent<Toggle>();
        gs.SoundEnabled = tSoundEnabled.isOn;

        Transform tSliderSoundVolume = tPanel.Find("SliderSoundVolume");
        Slider sSoundVolume = tSliderSoundVolume.GetComponent<Slider>();
        gs.SoundVolume = sSoundVolume.value;

        FileManager.SaveGameSettings(gs);

        if (!gs.SoundEnabled)
            SoundManager.StopMainMusic();
        else
        {
            SoundManager.SetMainMusicVolume(gs.SoundVolume);
            SoundManager.StartMainMusic();
        }
    }

    private GameObject goQuestion = null;

    public void Options_ResetSave()
    {
        ShowQuestionResetSave();

        //FileManager.InitGameSave();
    }

    private void ShowQuestionResetSave()
    {
        GameObject goPanel = (GameObject)Instantiate(Resources.Load("question_reset_save"), transform);
        //goPanel.transform.SetParent(transform);

        GameObject goImage = (GameObject)goPanel.transform.GetChild(0).gameObject;

        //Yes = 0
        Button btnYes = goImage.transform.GetChild(1).gameObject.GetComponent<Button>();
        btnYes.onClick.AddListener(() => QuestionYes());

        //No = 1
        Button btnNo = goImage.transform.GetChild(2).gameObject.GetComponent<Button>();
        btnNo.onClick.AddListener(() => QuestionNo());

        Transform tLevelSelectionMenu = this.gameObject.transform.Find("LevelSelectionMenu");
        Transform tPanel = tLevelSelectionMenu.transform.Find("Panel");
        RectTransform rectTrans = tPanel.GetComponent<RectTransform>();
        float canvasWidth = rectTrans.rect.width;
        float canvasHeight = rectTrans.rect.height;

        float posX = canvasWidth / 2;
        float posY = canvasHeight / 2;

        //goPanel.transform.localPosition = new Vector3(posX, posY, goImage.transform.position.z);
        //goPanel.transform.position = new Vector3(posX, posY, goImage.transform.position.z);

        //goPanel.transform.localPosition = new Vector3(0,0, goPanel.transform.localPosition.z);
        //goPanel.transform.position = new Vector3(0, 0, goPanel.transform.position.z);

        goQuestion = goPanel;
    }

    private void QuestionYes()
    {
        FileManager.InitGameSave();

        Destroy(goQuestion);
        goQuestion = null;
    }

    private void QuestionNo()
    {
        Destroy(goQuestion);
        goQuestion = null;
    }

    #endregion

    public void LoadGameSettings()
    {
        GameSettings gs = FileManager.GetGameSettings();

        Transform tOptionPanel = this.gameObject.transform.Find("OptionPanel");
        Transform tPanel = tOptionPanel.transform.Find("Panel");

        Transform tToggleSoundEnabled = tPanel.Find("ToggleSoundEnabled");
        Toggle tSoundEnabled = tToggleSoundEnabled.GetComponent<Toggle>();
        tSoundEnabled.isOn = gs.SoundEnabled;

        Transform tSliderSoundVolume = tPanel.Find("SliderSoundVolume");
        Slider sSoundVolume = tSliderSoundVolume.GetComponent<Slider>();
        sSoundVolume.value = gs.SoundVolume;
    }

    public void ChangeKeyboardLayout()
    {
        GameObject goInput = GameObject.FindGameObjectWithTag("InputController");
        InputController inputCtrl = goInput.GetComponent<InputController>();

        if (inputCtrl.GetHorizontalAxis() == "Horizontal")
        {
            inputCtrl.SetQWERTY();

            UpdateTextLayoutQWERTY();
        }
        else
        {
            inputCtrl.SetAZERTY();

            UpdateTextLayoutAZERTY();
        }
    }

    public void UpdateTextLayoutAZERTY()
    {
        Transform tMainPanel = transform.Find("MainMenuPanel");
        Transform tLayout = tMainPanel.Find("pnlLayout");
        Transform tText = tLayout.Find("txtLayout");
        Transform tButton = tLayout.Find("btnLayout");
        Transform tButtonText = tButton.Find("Text");

        Text txtKeyboard = (Text)tText.gameObject.GetComponent<Text>();
        txtKeyboard.text = "Current keyboard layout : AZERTY";

        Text btnKeyboard = (Text)tButtonText.gameObject.GetComponent<Text>();
        btnKeyboard.text = "Change to QWERTY";
    }

    public void UpdateTextLayoutQWERTY()
    {
        Transform tMainPanel = transform.Find("MainMenuPanel");
        Transform tLayout = tMainPanel.Find("pnlLayout");
        Transform tText = tLayout.Find("txtLayout");
        Transform tButton = tLayout.Find("btnLayout");
        Transform tButtonText = tButton.Find("Text");

        Text txtKeyboard = (Text)tText.gameObject.GetComponent<Text>();
        txtKeyboard.text = "Current keyboard layout : QWERTY";

        Text btnKeyboard = (Text)tButtonText.gameObject.GetComponent<Text>();
        btnKeyboard.text = "Change to AZERTY";
    }
}
