using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public GameObject m_goFond;

    private float fSens;

    private float fVitesse = 0.15f;

    private float m_nCalcSize = 13;//Nombre de bloc de la structure
    private int nbStruct = 4;

    private GameObject lastGoInstante;

    float nOffSetCam;

    // Start is called before the first frame update
    void Start()
    {
        fSens = 1;

        int nPrefab = Random.Range(0, nbStruct);
        GameObject goGrid = (GameObject)Instantiate(Resources.Load("MainMenu/goBackGround_" + nPrefab));
        goGrid.transform.localPosition = new Vector3(0.5f, 0, 0);

        lastGoInstante = goGrid;

        nPrefab = Random.Range(0, nbStruct);
        GameObject goGrid2 = (GameObject)Instantiate(Resources.Load("MainMenu/goBackGround_" + nPrefab));
        goGrid2.transform.localPosition = new Vector3(0.5f - (m_nCalcSize + 1), 0, 0);

        nPrefab = Random.Range(0, nbStruct);
        GameObject goGrid3 = (GameObject)Instantiate(Resources.Load("MainMenu/goBackGround_" + nPrefab));
        goGrid3.transform.localPosition = new Vector3(0.5f - 2 * (m_nCalcSize + 1), 0, 0);

        float fPosXTest = Camera.main.pixelWidth;
        Vector3 vLast = Camera.main.ScreenToWorldPoint(new Vector3(fPosXTest, 0, 0));

        nOffSetCam = vLast.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float fVelocity = 0f;

        float posX = Mathf.SmoothDamp(transform.position.x, transform.position.x + (fVitesse * fSens), ref fVelocity, 0.05f);

        float fSomme = lastGoInstante.transform.position.x + m_nCalcSize + 1 + 0.5f;
        float fSomme_2 = Camera.main.transform.position.x + nOffSetCam + 1.5f;
        float fSomme3 = fSomme - fSomme_2;

        if (fSomme3 < 0)
        {
            int nPrefab = Random.Range(0, nbStruct);

            GameObject goGrid = (GameObject)Instantiate(Resources.Load("MainMenu/goBackGround_" + nPrefab));

            float fPosition = lastGoInstante.transform.position.x + m_nCalcSize + 1;

            goGrid.transform.localPosition = new Vector3(fPosition, goGrid.transform.localPosition.y, goGrid.transform.localPosition.z);

            lastGoInstante = goGrid;
        }

        transform.position = new Vector3(posX, transform.position.y, transform.position.z);

        if (m_goFond != null)
            m_goFond.transform.position = new Vector3(transform.position.x, transform.position.y, m_goFond.transform.position.z);
    }
}
