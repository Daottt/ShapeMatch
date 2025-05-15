using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseShapeData : ScriptableObject
{
    public GameObject[] PossibleShapePrefabs;
    public Color ShapeColor;
    public Color OutlineColor;
    public Sprite Image;
    public Color ImageColor;
    public Color SelectedOutline;
    public virtual void OnAddToActionBar() { }

    public virtual void OnRemoveFromActionBar() { }

    public virtual void OnGameTurn() { }
}
