using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public interface IClickable
{
    void Interact();
}

public class AI_Controller : MonoBehaviour
{
    [Header("Waypoint Configs")]
    [SerializeField] GameObject prefab_waypoint;
    [SerializeField] List<GameObject> waypointCollection;
    [SerializeField] int max_waypointSpawn = 4;
    GameObject spawned_waypoint;
    int currentWaypointIndex;

    [Header("Behavior Label Config")]
    [SerializeField] float labelHeight;
    [SerializeField] bool movePatrol;
    [SerializeField] TMP_Text activityLabelText;
    public GameObject activityLabel;

    [Header("Keybinds Config")]
    [SerializeField] KeyCode patrolKey = KeyCode.T;
    [SerializeField] KeyCode waypointKey = KeyCode.Q;

    // public declarations
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public State CurrentState { get; set; }
    [HideInInspector] public bool CanMove { get; set; } = true;
    [HideInInspector] public bool isGatherable;

    //hover
    Transform hover_object;
    RaycastHit raycastHit;  // used in hover
    RaycastHit mouseHit;    // used in movement

    // script references
    Resource resource;
    Resource resource_lastHover;

    public enum State
    {
        Patrol,
        Gathering
    }

    private void Start() => navMeshAgent = GetComponent<NavMeshAgent>();

    private void Update()
    {
        Ai_State();

        // Mouse Raycast | Click to move ai
        if (Input.GetMouseButtonDown(0) && CanMove && navMeshAgent.isActiveAndEnabled)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out mouseHit) && !Input.GetKey(waypointKey))
            {
                movePatrol = false;
                navMeshAgent.SetDestination(mouseHit.point);
            }
        }

        KeyBindings();
        CheckMouseClick();

        // hover object handler
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            hover_object = raycastHit.transform;
            if (hover_object.gameObject.TryGetComponent<Resource>(out resource))
            {
                resource.OnHover();
                resource_lastHover = resource;
            }
            else
            {
                if (resource_lastHover != null)
                    resource_lastHover.OnHoverExit();
                resource_lastHover = null;
            }
        }
    }

    void CheckMouseClick()
    {
        if (!mouseHit.collider || Input.GetKey(waypointKey)) return;

        if ((mouseHit.collider.CompareTag("Ore") || mouseHit.collider.CompareTag("Tree")) && CurrentState != State.Gathering)
        {
            Debug.Log("A resource is clicked!");
            if (navMeshAgent.isActiveAndEnabled)
                navMeshAgent.SetDestination(mouseHit.collider.transform.position);
            isGatherable = true;
        }
    }

    public void StopNavmesh(bool condition) => navMeshAgent.enabled = !condition;                   // navmesh toggle
    public void SetActivityLabel(string activityText) => activityLabelText.SetText(activityText);   // sets the activity text | label prefab

    void KeyBindings()
    {
        if (Input.GetKeyDown(patrolKey))
            movePatrol = !movePatrol;

        if (Input.GetKey(waypointKey) && Input.GetMouseButtonDown(0))
            SpawnWaypoint();

        if (Input.GetKey(waypointKey) && Input.GetMouseButtonDown(1))
            UndoPlacement();
    }

    void InteractWithObject(GameObject objectToInteractWith)
    {
        if (objectToInteractWith.TryGetComponent(out IClickable clickableObject))
            clickableObject.Interact();
    }

    #region Behavior
    void Ai_State()
    {
        switch (CurrentState)
        {
            case State.Gathering:
                Mining();
                break;
            case State.Patrol:
                Patroling();
                break;
            default:
                break;
        }
    }

    public void Patroling()
    {
        activityLabel.SetActive(false);
        Debug.Log("AI on Patrol!");

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && movePatrol)
            WaypointMove();
    }

    void Mining()
    {
        activityLabel.SetActive(true);
        Vector3 playerPos = transform.position + new Vector3(-1, 0, -1.5f);
        playerPos.y = labelHeight;
        activityLabel.transform.position = playerPos;
    }
    #endregion

    #region Waypoint
    void SpawnWaypoint()
    {
        if (max_waypointSpawn != 0)
        {
            max_waypointSpawn--;
            Debug.Log("Spawn Waypoint!");
            spawned_waypoint = Instantiate(prefab_waypoint, mouseHit.point, Quaternion.identity);
            waypointCollection.Add(spawned_waypoint);
        }
    }

    void UndoPlacement()
    {
        GameObject currentWaypoint = waypointCollection[^1];
        waypointCollection?.Remove(currentWaypoint);
        Destroy(currentWaypoint);
        max_waypointSpawn++;
    }

    void WaypointMove()
    {
        try
        {
            if (waypointCollection.Count == 0) return;

            float distanceToWaypoint = Vector3.Distance(waypointCollection[currentWaypointIndex].transform.position, transform.position);

            if (distanceToWaypoint <= 3)
                currentWaypointIndex = (currentWaypointIndex + 1) % waypointCollection.Count;

            navMeshAgent.SetDestination(waypointCollection[currentWaypointIndex].transform.position);
        }
        catch (System.Exception)
        {
            navMeshAgent.SetDestination(waypointCollection[0].transform.position);
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Ore") || other.gameObject.CompareTag("Tree")) && isGatherable)
        {
            CurrentState = State.Gathering;
            InteractWithObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CurrentState = State.Patrol;
    }
}
