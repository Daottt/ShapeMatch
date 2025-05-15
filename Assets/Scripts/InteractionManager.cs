using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactioLayer;
    private bool canInteract;

    private Shape currentShape;
    private void OnEnable()
    {
        EventBus.Instance.GameStarted += () => { canInteract = true; };
        EventBus.Instance.GameEnded += () => { canInteract = false; };
    }
    void Update()
    {
        Interact();
        if (Input.GetMouseButtonUp(0))
        {
            Select();
        }
    }

    void Interact()
    {
        if (!canInteract) return;
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapCircle(mousePosition, 0.1f, interactioLayer);
        if (hit)
        {
            if(hit.GetComponent<Shape>() is Shape shape)
            {
                if (shape == currentShape) return;
                if (currentShape != null) currentShape.Deselect();
                shape.Select();
                currentShape = shape;
            }
        }
        else
        {
            currentShape = null;
        }
    }

    void Select()
    {
        if (currentShape == null) return;
        EventBus.Instance.ShapeSelected.Invoke(currentShape);
        currentShape = null;
    }
}
