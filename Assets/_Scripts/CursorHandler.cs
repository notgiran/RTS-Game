using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Drag : MonoBehaviour
{
    [Header("Cursor Configs")]
    [SerializeField] Texture2D cursorArrow;
    [SerializeField] Texture2D cursorPickaxe;
    [SerializeField] Texture2D cursorAxe;

    public static Camera_Drag Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start() => OnCursorHoverExit();

    // handles changing cursors
    public void OnCursorHoverExit() => Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    
    public void OnHover(ResourceType type)
    {
        Debug.Log($"Im called from Camera Drag! Resource type is: {type}");
        switch (type)
        {
            case ResourceType.Tree:
                Cursor.SetCursor(cursorAxe, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case ResourceType.Ore:
                Cursor.SetCursor(cursorPickaxe, Vector2.zero, CursorMode.ForceSoftware);
                break;
        }
    }
}
