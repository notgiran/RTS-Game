using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD_Manager : MonoBehaviour
{
    public static HUD_Manager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    
    [SerializeField] GameObject label_controlsInfo;
    [SerializeField] GameObject popUp_controlsInfo;

    [Header("Resources Labels")]
    [SerializeField] TMP_Text label_woodResource;
    [SerializeField] TMP_Text label_oreResource;
    public int WoodCount { get; set; } = 0;
    public int OreCount { get; set; } = 0;

    [Header("Keybinds")]
    [SerializeField] KeyCode key_controlsInfo = KeyCode.L;

    private void Update()
    {
        // update resources values
        label_woodResource.SetText($"Wood: {WoodCount}");
        label_oreResource.SetText($"Metal Ore: {OreCount}");

        if (Input.GetKey(key_controlsInfo))
        {
            label_controlsInfo.SetActive(false);
            popUp_controlsInfo.SetActive(true);
        }
        else
        {
            label_controlsInfo.SetActive(true);
            popUp_controlsInfo.SetActive(false);
        }
    }
}