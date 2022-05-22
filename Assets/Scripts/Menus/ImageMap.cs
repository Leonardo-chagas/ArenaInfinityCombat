using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageMap : MonoBehaviour
{
    [SerializeField] TMP_Dropdown selectScene;
    [SerializeField] Image imageMap;
    [SerializeField] Sprite arenaImage;
    [SerializeField] Sprite installationImage;
    [SerializeField] Sprite constructionImage;
    [SerializeField] Sprite plazaImage;
    [SerializeField] Sprite platformsImage;

    void Start()
    {
        selectScene.onValueChanged.AddListener(delegate { OnSelectSceneChange(); });
    }

    public void OnSelectSceneChange()
    {
        // string[] array = new string[5] {"arena", "construction", "installation", "platforms", "plaza"};

        if (selectScene.value == 0) 
        {
            imageMap.sprite = arenaImage;
        }
        if (selectScene.value == 1) 
        {
            imageMap.sprite = constructionImage;
        }
        if (selectScene.value == 2) 
        {
            imageMap.sprite = installationImage;
        }
        if (selectScene.value == 3) 
        {
            imageMap.sprite = platformsImage;
        }
        if (selectScene.value == 4) 
        {
            imageMap.sprite = plazaImage;
        }
        Debug.Log("Oi Mapa");
    }

}
