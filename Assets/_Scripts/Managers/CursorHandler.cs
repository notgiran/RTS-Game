using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    [Header("Cursor Configs")]
    [SerializeField] Texture2D cursor_Arrow;
    [SerializeField] Texture2D cursor_Pickaxe;
    [SerializeField] Texture2D cursor_Axe;
    [SerializeField] Texture2D cursor_Waypoint;

    public static CursorHandler Instance;

    void Start() => Cursor.SetCursor(cursor_Arrow, Vector2.zero, CursorMode.ForceSoftware);
    void SetCursor(Texture2D cursor) => Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware); 

    // handles changing cursors
    public void DefaultCursor() => SetCursor(cursor_Arrow);
    
    public void WaypointCursor() => SetCursor(cursor_Waypoint);

    public void OnHover(ResourceType type)
    {
        Debug.Log($"Im called from Camera Drag! Resource type is: {type}");
        switch (type)
        {
            case ResourceType.Tree:
                SetCursor(cursor_Axe);
                break;
            case ResourceType.Ore:
                SetCursor(cursor_Pickaxe);
                break;
        }
    }
}
