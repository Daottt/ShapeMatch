using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public const int spawnAmount = 69;

    [SerializeField] private BaseShapeData[] ShapesData;
    [SerializeField] private int SpawnOffsetY = 2;
    [SerializeField] private float SpawnOffsetForce;
    [SerializeField] private float SpawnFallingForce;

    [SerializeField] private Transform spawnPoint;
    private void OnEnable()
    {
#pragma warning disable 162
        if (spawnAmount % ActionBar.AmountToRemove != 0) 
            Debug.LogError("Число фигур не кратно " + ActionBar.AmountToRemove);
#pragma warning restore  162
        //EventBus.Instance.GameStarted += 
    }
    private void OnDisable()
    {
    }
    public void Spawn()
    {
        EventBus.Instance.GameStarted.Invoke();
        var shapes = new (BaseShapeData, GameObject)[spawnAmount];
        BaseShapeData[] shapesData = new BaseShapeData[ShapesData.Length];
        ShapesData.CopyTo(shapesData, 0);
        ShuffleArray(shapesData);
        for (int i = 0, k = 0; i < shapes.Length; i += 3, k++)
        {
            if (k >= shapesData.Length) k = 0;
            var shapePrefab = shapesData[k].PossibleShapePrefabs[Random.Range(0, shapesData[k].PossibleShapePrefabs.Length)];
            for (int j = i; j < i + ActionBar.AmountToRemove; j++)
            {
                shapes[j].Item1 = shapesData[k];
                shapes[j].Item2 = shapePrefab;
            }
        }
       ShuffleArray(shapes);

        int yOffset = 0;
        foreach (var item in shapes)
        {
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y + yOffset, 0);

            Shape shape = Instantiate(item.Item2, spawnPosition, spawnPoint.rotation, spawnPoint).GetComponent<Shape>();
            shape.Init(item.Item1);

            shape.rigidBody2D.AddForce(new Vector2(Random.Range(-SpawnOffsetForce, SpawnOffsetForce), SpawnFallingForce));
            yOffset += SpawnOffsetY;
        }
    }

    public void Reshuffle()
    {
        int yOffset = 0;
        for (int i = 0; i < spawnPoint.childCount; i++)
        {
            var item = spawnPoint.GetChild(i).GetComponent<Rigidbody2D>();
            Vector3 newPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y + yOffset, 0);
            item.transform.position = newPosition;

            item.velocity = Vector2.zero;
            item.AddForce(new Vector2(Random.Range(-SpawnOffsetForce, SpawnOffsetForce), SpawnFallingForce));
            yOffset += SpawnOffsetY;
        }
    }

    public static void PrintDictionary(Dictionary<ShapeType, int> dict)
    {
        foreach (var item in dict)
        {
            Debug.Log("Key: " + item.Key.sprite + " Value " + item.Value);
        }
    }

    public void Restart()
    {
        for (int i = 0; i < spawnPoint.childCount; i++)
        {
            Destroy(spawnPoint.GetChild(i).gameObject);
        }
        Spawn();
    }

    public void ShuffleArray<T>(T[] array) 
    {
        for (int i = 0; i < array.Length; i++)
        {
            int j = Random.Range(0, array.Length);
            var temp = array[j];
            array[j] = array[i];
            array[i] = temp;
        }
    }
}
