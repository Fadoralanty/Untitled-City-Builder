using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> PlacedObjects=new ();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionsToOccupy, ID, placedObjectIndex);
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
    public PlacementData[] GetNeighbours(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> occupiedPositions = CalculateOuterPositions(gridPosition, objectSize);
        foreach (var position in occupiedPositions)
        {
            //if position has a neighbour of compatible type
            if (PlacedObjects.ContainsKey(position) /* && hasCompatibleType*/)
            {
                //(if applicable when the current amopunt of compatible neighbours > required to combine) delete previous buildings
                //(if applicable) place new building
                //produce more money
            }
        }
        PlacementData[] neighbours = new PlacementData[] { };
        
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
        foreach (var pos in PlacedObjects[gridPosition].occupiedPositions)
        {
            PlacedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = id;
        PlacedObjectIndex = placedObjectIndex;
    }

    
}
