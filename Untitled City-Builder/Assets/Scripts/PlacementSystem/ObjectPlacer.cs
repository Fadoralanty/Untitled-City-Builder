using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _placedGameObjects = new List<GameObject>();
    public Action<int, PlacingType> onObjectPlaced;
    public Action<int, PlacingType> onObjectRemoved;

    public int PlaceObject(GameObject prefab, Vector3 position, int id, PlacingType placingType)
    {
        GameObject newBuilding = Instantiate(prefab);
        newBuilding.transform.position = position;
        _placedGameObjects.Add(newBuilding);
        onObjectPlaced?.Invoke(id, placingType);
        return _placedGameObjects.Count - 1;
    }

    public void RemoveObject(int gameObjectIndex, int id, PlacingType placingType)
    {
        if (_placedGameObjects.Count <= gameObjectIndex) { return ;}
        if(_placedGameObjects[gameObjectIndex] == null) { return; }

        Destroy(_placedGameObjects[gameObjectIndex]);
        _placedGameObjects[gameObjectIndex] = null;
        onObjectRemoved?.Invoke(id, placingType);
    }
}
