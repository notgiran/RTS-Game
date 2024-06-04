using UnityEngine;

public class Tree_Resource : Resource, IClickable
{
    [SerializeField] float treeHP = 100f;
    [SerializeField] float gatherer_GatherDamage = 25f;
    [SerializeField] float labelHeight = 5f;
    AI_Controller aI_Controller;        // ref to Ai controller gameobject
    AI_Controller lastGatherer;         // call the last gatherer
    GameObject gathererAssigned;

    public override ResourceType GetResourceType() => ResourceType.Tree;

    private void Update()
    {
        lastGatherer = aI_Controller;

        if (treeHP <= 0 && lastGatherer != null)
        {
            DestroyHpBar();
            Destroy(gameObject);
            lastGatherer.StopNavmesh(false);
            lastGatherer.CanMove = true;
            // lastGatherer.isGatherable = false;
            lastGatherer.CurrentState = AI_Controller.State.Idle;

            HUD_Manager.Instance.WoodCount += Random.Range(3, 8);
            gathererAssigned.GetComponent<AI_Controller>().DeletePath();
        }
    }

    public void Interact()
    {
        if (gathererAssigned != null)
        {
            gathererAssigned.GetComponent<AI_Controller>().SetActivityLabel("Chopping");
            InvokeRepeating(nameof(Collect), 1f, 1f);
        }
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

    // TODO: Create a reference for assigned charactere for a specific tree.
    // parang ipapasa ang game object sa condition na i ccheck ang assigned object na mag cchop sa tree.
    // GOODLUCK!!!
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
    public override void SetGathererAssigned(GameObject gatherer) => gathererAssigned = gatherer;
}