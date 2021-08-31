using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Image capacity_block_image;
    public Text capacity_block_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCapacityBlock()
    {
        capacity_block_image.gameObject.SetActive(true);
        capacity_block_text.gameObject.SetActive(true);
    }

    public void HideCapacityBlock()
    {
        capacity_block_image.gameObject.SetActive(false);
        capacity_block_text.gameObject.SetActive(false);
    }
}
