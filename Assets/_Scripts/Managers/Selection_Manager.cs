using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Selection_Manager : MonoBehaviour
{
    public static Selection_Manager Instance;
    public GameObject currentSelectedCharacter;

    [Header ("Selection Box Configs")]
    [SerializeField] RectTransform selectBox;
    [SerializeField] List<AI_Controller> allSelectableObject, currentlySelectedObjects;
    bool isMouseDown, isDragging;
    Vector3 mouseStartPos;

    private void Start()
    {
        AI_Controller[] temp = FindObjectsOfType<AI_Controller>();

        foreach (var selectableObject in temp)
            allSelectableObject.Add(selectableObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.Q))
        {
            isMouseDown = true;
            mouseStartPos = Input.mousePosition;
            if (currentlySelectedObjects != null)
            {
                foreach (AI_Controller characterUnit in currentlySelectedObjects)
                {
                    characterUnit.DeselectCharacter();
                }
                currentlySelectedObjects.Clear();
            }
        }
        if (isMouseDown)
        {
            if (Vector3.Distance(Input.mousePosition, mouseStartPos) > 1 && !isDragging)
            {
                isDragging = true;
                selectBox.gameObject.SetActive(true);
            }
            if (isDragging)
            {
                float boxWidth = Input.mousePosition.x - mouseStartPos.x;
                float boxHeight = Input.mousePosition.y - mouseStartPos.y;

                selectBox.sizeDelta = new Vector2(Mathf.Abs(boxWidth), Mathf.Abs(boxHeight));
                selectBox.anchoredPosition = (mouseStartPos + Input.mousePosition) / 2;

                SelectUnits();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            isDragging = false;
            selectBox.gameObject.SetActive(false);
        }
    }

    void SelectUnits()
    {
        foreach (AI_Controller characterUnit in allSelectableObject)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(characterUnit.transform.position);
            float left = selectBox.anchoredPosition.x - (selectBox.sizeDelta.x / 2);
            float right = selectBox.anchoredPosition.x + (selectBox.sizeDelta.x / 2);
            float top = selectBox.anchoredPosition.y + (selectBox.sizeDelta.y / 2);
            float bottom = selectBox.anchoredPosition.y - (selectBox.sizeDelta.y / 2);

            if (screenPos.x > left && screenPos.x < right && screenPos.y > bottom && screenPos.y < top)
            {
                if (!currentlySelectedObjects.Contains(characterUnit))
                {
                    currentlySelectedObjects.Add(characterUnit);
                    characterUnit.SelectCharacter();
                }
            }
            else
            {
                if (currentlySelectedObjects != null)
                {
                    if (currentlySelectedObjects.Contains(characterUnit))
                    {
                        currentlySelectedObjects.Remove(characterUnit);
                        characterUnit.DeselectCharacter();
                    }
                }
            }
        }
    }


    #region Public Methods

    public void SelectCharacter(GameObject character)
    {
        Debug.Log($"Selected Character: {character.name}");
        currentSelectedCharacter = character;
    }

    public void DeselectCharacter()
    {
        currentSelectedCharacter.GetComponent<AI_Controller>().DeselectCharacter();
        currentSelectedCharacter = null;
    }

    // TO ADD: spawn unit game mechanic
    public void AddSelectableUnit(AI_Controller addObject) => allSelectableObject.Add(addObject);
    #endregion
}
