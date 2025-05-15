using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public struct ShapeType
{
    public Sprite sprite;
    public Color color;

    public ShapeType(Sprite sprite, Color color)
    {
        this.sprite = sprite;
        this.color = color;
    }
}

public class Shape : MonoBehaviour
{
    public Rigidbody2D rigidBody2D;

    [SerializeField] private SpriteRenderer BaseSprite;
    [SerializeField] private SpriteRenderer ImageSprite;
    [SerializeField] private SpriteRenderer OutlineSprite;
    [SerializeField] private SpriteMask Mask;
    
    public ShapeType shapeType;
    [HideInInspector] public BaseShapeData shapeData;

    private void OnEnable()
    {
        EventBus.Instance.ShapeSelected += OnGameTurn;
    }
    private void OnDisable()
    {
        EventBus.Instance.ShapeSelected -= OnGameTurn;
    }
    public void Init(BaseShapeData shapeData)
    {
        BaseSprite.color = shapeData.ShapeColor;
        ImageSprite.sprite = shapeData.Image;
        ImageSprite.color = shapeData.ImageColor;
        OutlineSprite.color = shapeData.OutlineColor;
        this.shapeData = shapeData;
        shapeType = new ShapeType(BaseSprite.sprite, shapeData.ImageColor);
    }

    public void Press()
    {
        Destroy(rigidBody2D);
        Destroy(GetComponent<Collider2D>());
        Deselect();
    }
    public void Select()
    {
        OutlineSprite.color = shapeData.SelectedOutline;
    }

    public void Deselect()
    {
        OutlineSprite.color = shapeData.OutlineColor;
    }

    void OnGameTurn(Shape shape)
    {
        shapeData.OnGameTurn();
    }

    public void Hide()
    {
        BaseSprite.enabled = false;
        ImageSprite.enabled = false;
        OutlineSprite.enabled = false;
        Destroy(Mask);
    }
}
