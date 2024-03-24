using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    private Grid _grid;
    private PlacementPreviewSystem _previewSystem;
    private GridData _floorData;
    private GridData _buildingsData;
    private ObjectPlacer _objectPlacer;
    private float cellIndicatorElevation=0.01f;

    public RemovingState(Grid grid, PlacementPreviewSystem previewSystem, GridData floorData, GridData buildingsData, ObjectPlacer objectPlacer)
    {
        _grid = grid;
        _previewSystem = previewSystem;
        _floorData = floorData;
        _buildingsData = buildingsData;
        _objectPlacer = objectPlacer;
        
        _previewSystem.ShowRemovePreview();
    }

    public void EndState()
    {
        _previewSystem.HidePreview();
    }

    public int OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        int objID;
        if (_buildingsData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == false)
        {
            selectedData = _buildingsData;
        }else if (_floorData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == false)
        {
            selectedData = _floorData;
        }

        if (selectedData==null)
        {
            //feedback that there is nothing to remove
            return -1;
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            objID = selectedData.GetIDOfPlacedObject(gridPosition);
            if (gameObjectIndex==-1) {return -1; }

            selectedData.RemoveObjectAt(gridPosition);
            _objectPlacer.RemoveObject(gameObjectIndex);
        }

        Vector3 cellPosition = _grid.CellToWorld(gridPosition);
        _previewSystem.UpdatePosition(cellPosition,CheckIfSelectionIsValid(gridPosition));
        return objID;
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(_buildingsData.CanPlaceObjectAt(gridPosition, Vector2Int.one) && _floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), validity);
    }
}
