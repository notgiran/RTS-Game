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
    [SerializeField] GameObject activityLabel;

    [Header("Keybinds Config")]
    [SerializeField] KeyCode patrolKey = KeyCode.T;
    [SerializeField] KeyCode waypointKey = KeyCode.Q;

    [Header("Selection Configs")]
    [SerializeField] GameObject prefab_selectionCircle;
    GameObject spawned_selectionCircle;

    // Line Renderer Configs
    LineRenderer lineRenderer;

    // public declarations
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public State CurrentState { get; set; }
    [HideInInspector] public bool CanMove { get; set; } = true;

    //hover
    Transform hover_object;
    RaycastHit raycastHit;  // used in hover
    RaycastHit mouseHit;    // used in movement

    // crowd or ghost selection
    bool isSelected;

    // references
    Resource resource;
    Resource resource_lastHover;
    [HideInInspector] public GameObject resourceToGather;

    public enum State
    {
        Patrol,
        Gathering
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = .15f;
        lineRenderer.endWidth = .15f;
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        // draws the path using line renderer
        if (navMeshAgent.hasPath)
            DrawPath();

        Ai_State();
        KeyBindings();
        CheckMouseHover();
        SelectionCircleFollow();

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

    void CheckMouseHover()
    {
        if (mouseHit.collider == null || Input.GetKey(waypointKey)) return;

        if ((mouseHit.collider.CompareTag("Ore") || mouseHit.collider.CompareTag("Tree")) && CurrentState != State.Gathering)
        {
            Debug.Log("A resource is clicked!");
            if (navMeshAgent.isActiveAndEnabled)
                navMeshAgent.SetDestination(mouseHit.collider.transform.position);

            mouseHit.collider.GetComponent<Resource>().SetGathererAssigned(gameObject);
            resourceToGather = mouseHit.collider.gameObject;
        }
    }

    public void StopNavmesh(bool condition) => navMeshAgent.enabled = !condition;                   // navmesh toggle
    public void SetActivityLabel(string activityText) => activityLabelText.SetText(activityText);   // sets the activity text | label prefab

    void KeyBindings()
    {
        // Mouse Raycast | Click to move ai
        if (Input.GetMouseButtonDown(1) && CanMove && navMeshAgent.isActiveAndEnabled && isSelected)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out mouseHit) && !Input.GetKey(waypointKey))
            {
                movePatrol = false;
                navMeshAgent.SetDestination(mouseHit.point);
                DeselectCharacter();
            }
        }

        if (Input.GetKeyDown(patrolKey) && isSelected)
        {
            movePatrol = !movePatrol;
            DeselectCharacter();
        }

        // waypoint cursor change
        if (Input.GetKey(waypointKey) && isSelected)
            CursorHandler.Instance.WaypointCursor();
        else
            CursorHandler.Instance.DefaultCursor();

        // waypoint keybinds
        if (Input.GetKey(waypointKey) && Input.GetMouseButtonDown(0) && isSelected)
            SpawnWaypoint();

        if (Input.GetKey(waypointKey) && Input.GetMouseButtonDown(1) && isSelected)
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
                Gathering();
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

        if (movePatrol && navMeshAgent.isActiveAndEnabled && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            WaypointMove();
    }

    void Gathering()
    {
        Vector3 playerPos = transform.position + new Vector3(-1, 0, -1.5f);
        playerPos.y = labelHeight;
        activityLabel.transform.position = playerPos;
        activityLabel.SetActive(true);
    }
    #endregion

    #region Waypoint
    void SpawnWaypoint()
    {
        if (max_waypointSpawn != 0)
        {
            max_waypointSpawn--;
            Debug.Log("Spawn Waypoint!");
            spawned_waypoint = Instantiate(prefab_waypoint, raycastHit.point, Quaternion.identity);
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
            navMeshAgent.SetDestination(waypointCollection[^1].transform.position);
            currentWaypointIndex = 0;
        }
    }
    #endregion

    #region Draw Path
    void DrawPath()
    {
        lineRenderer.positionCount = navMeshAgent.path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);
        if (navMeshAgent.path.corners.Length < 2) return;
        for (int i = 0; i < navMeshAgent.path.corners.Length; i++)
        {
            Vector3 pointPos = new(navMeshAgent.path.corners[i].x, navMeshAgent.path.corners[i].y + 1.5f, navMeshAgent.path.corners[i].z);
            lineRenderer.SetPosition(i, pointPos);
        }
    }

    public void DeletePath()
    {
        lineRenderer.positionCount = 0;
    }
    #endregion

    #region Selection
    public void DeselectCharacter()
    {
        isSelected = false;
        Destroy(spawned_selectionCircle);
    }

    void SelectionCircleFollow()
    {
        if (spawned_selectionCircle != null)
            spawned_selectionCircle.transform.position = transform.position;
    }

    private void OnMouseDown()
    {
        if (Selection_Manager.Instance.currentSelectedCharacter != this && Selection_Manager.Instance.currentSelectedCharacter != null)
            Selection_Manager.Instance.DeselectCharacter();
        isSelected = true;
        Selection_Manager.Instance.SelectCharacter(gameObject);
        spawned_selectionCircle = Instantiate(prefab_selectionCircle, transform.position, Quaternion.identity);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Ore") || other.gameObject.CompareTag("Tree")) && other.gameObject == resourceToGather)
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