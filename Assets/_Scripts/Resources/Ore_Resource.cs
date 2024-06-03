using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ore_Resource : Resource, IClickable
{
    [SerializeField] float oreHP = 100f;
    [SerializeField] float gatherer_GatherDamage = 25f;
    [SerializeField] float labelHeight = 5f;
    AI_Controller aI_Controller;    // ref to Ai controller gameobject
    AI_Controller lastGatherer;        // call the last gatherer
    GameObject gathererAssigned;

    public override ResourceType GetResourceType() => ResourceType.Ore;

    private void Update()
    {
        lastGatherer = aI_Controller;

        if (oreHP <= 0 && lastGatherer != null)
        {
            DestroyHpBar();
            Destroy(gameObject);
            aI_Controller.StopNavmesh(false);
            aI_Controller.CanMove = true;
            
            aI_Controller.CurrentState = AI_Controller.State.Patrol;

            HUD_Manager.Instance.OreCount += Random.Range(2, 5);
            gathererAssigned.GetComponent<AI_Controller>().DeletePath();
        }
    }

    public void Interact()
    {
        if (gathererAssigned != null)
        {
            gathererAssigned.GetComponent<AI_Controller>().SetActivityLabel("Mining");
            InvokeRepeating(nameof(Collect), .5f, 1f);
        }
    }

    public override void Collect()
    {
        if (oreHP > 0 && aI_Controller != null)
        {
            aI_Controller.CanMove = false;
            Damage(gatherer_GatherDamage);
        }
    }

    public override void Damage(float damage)
    {
        oreHP -= damage;
        if (spawn_HpImage != null)
            spawn_HpImage.fillAmount = oreHP / 100f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gathererAssigned == other.gameObject && gathererAssigned.GetComponent<AI_Controller>().resourceToGather == gameObject)
        {
            Debug.Log($"Gatherer assigned to tree named: {gameObject.name}\nAssigned gatherer is: {gathererAssigned.name} and {other.gameObject}");

            if (gathererAssigned.TryGetComponent<AI_Controller>(out aI_Controller))
            {
                SpawnHpBar(labelHeight);
                aI_Controller.StopNavmesh(true);
            }
            else Debug.Log("Im called from On trigger enter on Tree Script! Not gathering state!"); ;
        }
    }

    public override void SetGathererAssigned(GameObject gameObject) => gathererAssigned = gameObject;
}