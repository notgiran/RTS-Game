using UnityEngine;
using UnityEngine.UI;

public enum ResourceType { Ore, Tree } // Enum to define resource types

public abstract class Resource : MonoBehaviour
{
    [SerializeField] GameObject prefab_HpBar;
    protected string resourceName; // Name of the resource
    protected GameObject spawn_HpBar;
    protected Image spawn_HpImage;
    protected GameObject GathererAssigned { get; set; }

    public abstract ResourceType GetResourceType(); // method to define resource type
    public abstract void Collect();
    public abstract void Damage(float damage);


    protected virtual void SpawnHpBar(float hpBar_height = 5f)
    {
        Vector3 spawnPos = transform.position + new Vector3(-1, 0, -1.5f);
        spawnPos.y = hpBar_height;
        spawn_HpBar = Instantiate(prefab_HpBar, spawnPos, Quaternion.identity);
        spawn_HpImage = spawn_HpBar.transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    public virtual void SetGathererAssigned(GameObject gameObject) => GathererAssigned = gameObject;

    protected virtual void DestroyHpBar() => Destroy(spawn_HpBar);

    public virtual void OnHover() => CursorHandler.Instance.OnHover(GetResourceType());

    public virtual void OnHoverExit() => CursorHandler.Instance.DefaultCursor();
}