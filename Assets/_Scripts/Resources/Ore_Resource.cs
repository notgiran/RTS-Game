using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Ore_Resource : Resource, IClickable
{
    [SerializeField] float oreHP = 100f;
    [SerializeField] float gatherer_GatherDamage = 25f;
    [SerializeField] float labelHeight = 5f;
    AI_Controller aI_Controller;    // ref to Ai controller gameobject
    AI_Controller lastGatherer;        // call the last gatherer

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
            aI_Controller.isGatherable = false;
            aI_Controller.CurrentState = AI_Controller.State.Patrol;
        }
    }

    public void Interact()
    {
        aI_Controller.SetActivityLabel("Mining");
        InvokeRepeating(nameof(Collect), .5f, 1f);
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
        other.gameObject.TryGetComponent<AI_Controller>(out aI_Controller);
        if (aI_Controller != null && aI_Controller.isGatherable)
        {
            SpawnHpBar(labelHeight);
            aI_Controller.StopNavmesh(true);
        }
    }
}