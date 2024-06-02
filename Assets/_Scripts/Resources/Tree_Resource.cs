using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Resource : Resource, IClickable
{
    [SerializeField] float treeHP = 100f;
    [SerializeField] float gatherer_GatherDamage = 25f;
    [SerializeField] float labelHeight = 5f;
    AI_Controller aI_Controller;        // ref to Ai controller gameobject
    AI_Controller lastGatherer;         // call the last gatherer

    public override ResourceType GetResourceType() => ResourceType.Tree;

    private void Update()
    {
        lastGatherer = aI_Controller;

        if (treeHP <= 0 && lastGatherer != null)
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
        aI_Controller.SetActivityLabel("Chopping");
        InvokeRepeating(nameof(Collect), .5f, 1f);
    }

    public override void Collect()
    {
        if (treeHP > 0 && aI_Controller != null)
        {
            aI_Controller.CanMove = false;
            Damage(gatherer_GatherDamage);
        }
    }

    public override void Damage(float damage)
    {
        treeHP -= damage;
        if (spawn_HpImage != null)
            spawn_HpImage.fillAmount = treeHP / 100f;
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