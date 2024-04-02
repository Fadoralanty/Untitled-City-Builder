using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> PlacedObjects=new ();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex, FusionData fusionData)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, ID, placedObjectIndex, fusionData);
        foreach (var position in positionsToOccupy)
        {
            if (PlacedObjects.ContainsKey(position))
            {
                throw new Exception($"Dictionary already contains this cell position {position}");
            }

            PlacedObjects[position] = data;
        }


    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnValues = new List<Vector3Int>();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y ; y++)
            {
                returnValues.Add(gridPosition +new Vector3Int(x,0,y));
            }
        }

        return returnValues;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionsToOccupy)
        {
            if (PlacedObjects.ContainsKey(pos)) 
            { return false; }
        }
        return true;
    }
    
    public List<PlacementData> GetNeighbours(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> outerPositions = CalculateOuterPositions(gridPosition, objectSize);
        List<PlacementData>  neighbours = new List<PlacementData>();
        for (var i = 0; i < outerPositions.Count; i++)
        {
            var position = outerPositions[i];
            //if position has a neighbour of compatible type
            if (PlacedObjects.ContainsKey(position) /* && hasCompatibleType*/)
            {
                neighbours.Add(PlacedObjects[position]);
            }
        }

        return neighbours;
    }
    
    private List<Vector3Int> CalculateOuterPositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnValues = new List<Vector3Int>();
        for (int x = -1; x < objectSize.x + 1; x++)
        {
            for (int y = -1; y < objectSize.y + 1 ; y++)
            {
                returnValues.Add(gridPosition +new Vector3Int(x,0,y));
            }
        }
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y ; y++)
            {
                returnValues.Remove(gridPosition +new Vector3Int(x,0,y));
            }
        }
        return returnValues;

    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (!PlacedObjects.ContainsKey(gridPosition)) { return -1; }
        return PlacedObjects[gridPosition].PlacedObjectIndex;
    }

    public int GetIDOfPlacedObject(Vector3Int gridPosition)
    {
        if (!PlacedObjects.ContainsKey(gridPosition)) { return -1; }

        return PlacedObjects[gridPosition].ID;
    }
    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (!PlacedObjects.ContainsKey(gridPosition)) { return; }
        foreach (var pos in PlacedObjects[gridPosition].occupiedPositions)
        {
            PlacedObjects.Remove(pos);
        }
    }
}
[System.Serializable]
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public FusionData FusionData { get; private set; }
    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex, FusionData fusionData)
    {
        this.occupiedPositions = occupiedPositions; 
        ID = id;
        PlacedObjectIndex = placedObjectIndex;
        FusionData = fusionData;
    }

    
}
