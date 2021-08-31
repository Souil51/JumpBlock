using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameObject PauseMenu;
    private GameObject EndMenu;
    private GameObject DeadMenu;

    private Tutoriel m_currentTuto = Tutoriel.Aucun;

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu = transform.Find("PauseMenu").gameObject;
        EndMenu = transform.Find("EndMenu").gameObject;
        DeadMenu = transform.Find("DiedMenu").gameObject;

        Button goPauseBack = PauseMenu.transform.Find("BackButton").gameObject.GetComponent<Button>();
        Button goEndBack = EndMenu.transform.Find("BackButton").gameObject.GetComponent<Button>();
        Button goDeadBack = DeadMenu.transform.Find("BackButton").gameObject.GetComponent<Button>();

        goPauseBack.onClick.AddListener(() => BackToMainMenu());
        goEndBack.onClick.AddListener(() => BackToMainMenu());
        goDeadBack.onClick.AddListener(() => BackToMainMenu());

        Transform tLayoutPause = PauseMenu.transform.Find("pnlLayout");
        Transform tLayoutDied = DeadMenu.transform.Find("pnlLayout");
        Transform tLayoutEnd = EndMenu.transform.Find("pnlLayout");

        Transform tButtonPause = tLayoutPause.Find("btnLayout");
        Transform tButtonDied = tLayoutDied.Find("btnLayout");
        Transform tButtonEnd = tLayoutEnd.Find("btnLayout");

        Button btnPause = tButtonPause.gameObject.GetComponent<Button>();
        Button btnDied = tButtonDied.gameObject.GetComponent<Button>();
        Button btnEnd = tButtonEnd.gameObject.GetComponent<Button>();

        btnPause.onClick.AddListener(() => ChangeLayout_Pause());
        btnDied.onClick.AddListener(() => ChangeLayout_Died());
        btnEnd.onClick.AddListener(() => ChangeLayout_End());

        GameObject goInput = GameObject.FindGameObjectWithTag("InputController");
        InputController inputCtrl = goInput.GetComponent<InputController>();

        UpdateTextLayout(PauseMenu.transform, inputCtrl.GetHorizontalAxis() == "Horizontal");
        UpdateTextLayout(EndMenu.transform, inputCtrl.GetHorizontalAxis() == "Horizontal");
        UpdateTextLayout(DeadMenu.transform, inputCtrl.GetHorizontalAxis() == "Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPauseMenu()
    {
        PauseMenu.SetActive(true);
        HideTuto();
    }

    public void HidePauseMenu()
    {
        PauseMenu.SetActive(false);
        ShowTuto();
    }

    public void ShowEndMenu()
    {
        EndMenu.SetActive(true);
        HideTuto();
    }

    public void HideEndMenu()
    {
        EndMenu.SetActive(false);
        ShowTuto();
    }

    public void ShowDeadMenu()
    {
        DeadMenu.SetActive(true);
        HideTuto();
    }

    public void HideDeadMenu()
    {
        DeadMenu.SetActive(false);
        ShowTuto();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }


    #region Tuto

    public enum Tutoriel { Aucun, Block, Checkpoint, Gravity, Jump, Teleport, Water, Win, Kill, Key }

    public void DrawTuto(Tutoriel tuto)
    {
        if (tuto == Tutoriel.Aucun) return;

        string szTuto = "Tuto/" + tuto.ToString() + "_tuto";

        GameObject goInput = GameObject.FindGameObjectWithTag("InputController");
        InputController inputCtrl = goInput.GetComponent<InputController>();

        if((tuto == Tutoriel.Win || tuto == Tutoriel.Jump) && inputCtrl.GetHorizontalAxis() != "Horizontal")
        {
            szTuto += "_QWERTY";
        }

        GameObject pnl = (GameObject)Instantiate(Resources.Load(szTuto), transform, false);
        pnl.transform.localPosition = pnl.transform.localPosition + new Vector3(0,-20,0);
        pnl.transform.position = pnl.transform.position + new Vector3(0, -30, 0);

        Transform t = this.transform.Find("Tutorial");
        t.gameObject.SetActive(true);

        pnl.transform.SetParent(t);

        pnl.SetActive(true);

        this.m_currentTuto = tuto;
    }

    public void ClearTuto()
    {
        Transform t = this.transform.Find("Tutorial");

        if (t.childCount > 0)
        {
            GameObject go = t.GetChild(0).gameObject;
            Destroy(go);
        }
    }

    public void HideTuto()
    {
        Transform t = this.transform.Find("Tutorial");
        t.gameObject.SetActive(false);
    }

    public void ShowTuto()
    {
        Transform t = this.transform.Find("Tutorial");
        t.gameObject.SetActive(true);
    }

    #endregion

    #region Clavier

    public void ChangeLayout_Pause()
    {
        Transform tPause = transform.Find("PauseMenu");
        ChangeLayout(tPause);
    }

    public void ChangeLayout_Died()
    {
        Transform tPause = transform.Find("DiedMenu");
        ChangeLayout(tPause);
    }

    public void ChangeLayout_End()
    {
        Transform tPause = transform.Find("EndMenu");
        ChangeLayout(tPause);
    }

    public void ChangeLayout(Transform tPanelLayout)
    {
        GameObject goInput = GameObject.FindGameObjectWithTag("InputController");
        InputController inputCtrl = goInput.GetComponent<InputController>();

        GameObject goPlayer = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerCtrl = goPlayer.GetComponent<PlayerController>();

        if (inputCtrl.GetHorizontalAxis() == "Horizontal")
        {
            inputCtrl.SetQWERTY();

            UpdateTextLayout(tPanelLayout, false);
        }
        else
        {
            inputCtrl.SetAZERTY();

            UpdateTextLayout(tPanelLayout, true);
        }

        playerCtrl.ChangeKeyboardLayout(inputCtrl.GetHorizontalAxis(), inputCtrl.GetVerticalAxis());

        if(m_currentTuto == Tutoriel.Win || m_currentTuto == Tutoriel.Jump)
        {
            ClearTuto();
            DrawTuto(m_currentTuto);
            HideTuto();
        }
    }

    public void UpdateTextLayout(Transform tPanelLayout, bool bAZERTY)
    {
        Transform tLayout = tPanelLayout.Find("pnlLayout");
        Transform tText = tLayout.Find("txtLayout");
        Transform tButton = tLayout.Find("btnLayout");
        Transform tButtonText = tButton.Find("Text");

        Text txtKeyboard = (Text)tText.gameObject.GetComponent<Text>();

        if (bAZERTY)
        {
            txtKeyboard.text = "Current keyboard layout : AZERTY";
        }
        else
        {
            txtKeyboard.text = "Current keyboard layout : QWERTY";
        }

        Text btnKeyboard = (Text)tButtonText.gameObject.GetComponent<Text>();

        if (bAZERTY)
        {
            btnKeyboard.text = "Change to QWERTY";
        }
        else
        {
            btnKeyboard.text = "Change to AZERTY";
        }
    }

    #endregion
}
