using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ResourceType { Ore, Tree } // Enum to define resource types

public abstract class Resource : MonoBehaviour
{
    protected string resourceName; // Name of the resource
    protected GameObject spawn_HpBar;
    protected Image spawn_HpImage;
    [SerializeField] GameObject prefab_HpBar;

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

    protected virtual void DestroyHpBar() => Destroy(spawn_HpBar);

    public virtual void OnHover()
    {
        //cursor
        Camera_Drag.Instance.OnHover(GetResourceType());
    }

    public virtual void OnHoverExit()
    {
        Camera_Drag.Instance.OnCursorHoverExit();
    }
}