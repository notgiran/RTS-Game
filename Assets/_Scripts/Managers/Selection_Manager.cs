using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection_Manager : MonoBehaviour
{
    public GameObject currentSelectedCharacter;
    
    public static Selection_Manager Instance;
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


    public void SelectCharacter(GameObject character)
    {
        Debug.Log($"Selected Character: {character.name}");
        currentSelectedCharacter = character;
    }

    public void DeselectCharacter()
    {
        currentSelectedCharacter.GetComponent<AI_Controller>().DeselectCharacter();
        currentSelectedCharacter = null;
    }
}
