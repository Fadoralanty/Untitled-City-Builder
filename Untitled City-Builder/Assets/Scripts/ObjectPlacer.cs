using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _placedGameObjects = new List<GameObject>();


    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newBuilding = Instantiate(prefab);
        newBuilding.transform.position = position;
        _placedGameObjects.Add(newBuilding); 
        return _placedGameObjects.Count - 1;
    }
}
