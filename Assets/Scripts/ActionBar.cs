using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ActionBarData
{
    public int Amount;
    public List<int> SlotsOccupiedIndex = new List<int>();
    public BaseShapeData shapeData;

    public ActionBarData(BaseShapeData shapeData)
    {
        this.shapeData = shapeData;
    }
}

public class ActionBar : MonoBehaviour
{
    public const int AmountToRemove = 3;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private Image[] slots;

    private Dictionary<ShapeType, ActionBarData> Shapes = new Dictionary<ShapeType, ActionBarData>();
    private Dictionary<int, Shape> SlotsOccupied = new Dictionary<int, Shape>();
    private int RemovedCount;

    private void OnEnable()
    {
        EventBus.Instance.ShapeSelected += AddShape;
        for (int i = 0; i < slots.Length; i++)
            SlotsOccupied.Add(i, null);
    }
    private void OnDisable()
    {
        EventBus.Instance.ShapeSelected -= AddShape;
    }

    public void AddShape(Shape shape)
    {
        var shapeType = shape.shapeType;
        if (!Shapes.ContainsKey(shapeType))
        {
            Shapes.Add(shapeType, new ActionBarData(shape.shapeData));
        }
        var emptySlot = 0;
        for (int i = 0; i < slots.Length; i++)
            if (SlotsOccupied[i] == null) emptySlot = i;

        Shapes[shapeType].Amount += 1;
        Shapes[shapeType].SlotsOccupiedIndex.Add(emptySlot);
        SlotsOccupied[emptySlot] = shape;
        PickUpShape(shape, emptySlot);

        shape.shapeData.OnAddToActionBar();
        CheckShapesAmount(shapeType);
    }

    void PickUpShape(Shape shape, int slot)
    {
        shape.transform.SetParent(transform);
        var worldPos = mainCamera.ScreenToWorldPoint(slots[slot].rectTransform.position);
        shape.Press();
        shape.transform.position = new Vector3(worldPos.x, worldPos.y -0.1f, 0);
        shape.transform.rotation = Quaternion.Euler(0,0,0);
        slots[slot].enabled = false;
    }

    void CheckShapesAmount(ShapeType shapeType)
    {
        foreach (var item in Shapes)
        {
            if(item.Value.Amount == AmountToRemove)
            {
                item.Value.Amount = 0;
                foreach (var slot in item.Value.SlotsOccupiedIndex)
                {
                    RemoveShape(slot);
                }
                item.Value.SlotsOccupiedIndex.Clear();
            }
        }
        if (RemovedCount >= Spawner.spawnAmount)
            EndGame(WinPanel);
        var count = 0;
        foreach (var item in SlotsOccupied)
            if (item.Value != null) count++;
        if (count == slots.Length)
        {
            EndGame(LosePanel);
        }
    }

    void RemoveShape(int index)
    {
        var shape = SlotsOccupied[index];
        shape.shapeData.OnRemoveFromActionBar();
        SlotsOccupied[index] = null;
        slots[index].enabled = true;
        RemovedCount++;
        shape.Hide();
    }

    void EndGame(GameObject panel)
    {
        EventBus.Instance.GameEnded.Invoke();
        panel.SetActive(true);
        SlotsOccupied.Clear();
        for (int i = 0; i < slots.Length; i++)
            SlotsOccupied.Add(i, null);
        foreach (var item in slots)
            item.enabled = true;
        Shapes.Clear();
        RemovedCount = 0;
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

    }
}