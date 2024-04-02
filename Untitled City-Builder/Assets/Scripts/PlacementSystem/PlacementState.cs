using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int _selectedObjectIndex = -1;
    private int _ID;
    private Grid _grid;
    private PlacementPreviewSystem _previewSystem;
    private ObjectsDatabaseSO _databaseSo;
    private GridData _floorData;
    private GridData _buildingsData;
    private ObjectPlacer _objectPlacer;
    private float cellIndicatorElevation=0.01f;

    public PlacementState(int id, Grid grid, PlacementPreviewSystem previewSystem, ObjectsDatabaseSO databaseSo, GridData floorData, GridData buildingsData, ObjectPlacer objectPlacer)
    {
        _ID = id;
        _grid = grid;
        _previewSystem = previewSystem;
        _databaseSo = databaseSo;
        _floorData = floorData;
        _buildingsData = buildingsData;
        _objectPlacer = objectPlacer;
        
        _selectedObjectIndex = _databaseSo.ObjectDataList.FindIndex(data => data.ID == _ID);
        if (_selectedObjectIndex > -1)
        {
            previewSystem.ShowPlacementPreview(_databaseSo.ObjectDataList[_selectedObjectIndex].Prefab,
                _databaseSo.ObjectDataList[_selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"no object with ID{_ID}");
        }
    }

    public void EndState()
    {
        _previewSystem.HidePreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool CanPlace = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        if (!CanPlace) {return;}

        if (!GameManager.Singleton.CanBuy(_selectedObjectIndex)) {return;}

        if (!IsNextToRoad(gridPosition, _databaseSo.ObjectDataList[_selectedObjectIndex].Size, 2)
            && _selectedObjectIndex != 2) { return; }

        //GridData selectedData = _databaseSo.ObjectDataList[_selectedObjectIndex].ID == 0 ? _floorData : _buildingsData;
        GridData selectedData = _buildingsData;
        
        //Fuse buildigns
        if (CanFuseBuildings(gridPosition,_databaseSo.ObjectDataList[_selectedObjectIndex]))
        {
            Fuse(gridPosition, _databaseSo.ObjectDataList[_selectedObjectIndex]);
            return;
        }
        
        //Place Building
        int index = _objectPlacer.PlaceObject(_databaseSo.ObjectDataList[_selectedObjectIndex].Prefab,
            _grid.CellToWorld(gridPosition) + new Vector3(0, cellIndicatorElevation, 0),
            _selectedObjectIndex, PlacingType.Buy);
        
        selectedData.AddObjectAt(gridPosition,_databaseSo.ObjectDataList[_selectedObjectIndex].Size,
            _databaseSo.ObjectDataList[_selectedObjectIndex].ID,
            index,
            _databaseSo.ObjectDataList[_selectedObjectIndex].FusionData);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
    }

    private void Fuse(Vector3Int gridPosition, ObjectData building)
    {
        //Get compatible neighbour buildings
        List<PlacementData> neighbours = _buildingsData.GetNeighbours(gridPosition, building.Size);
        int gameObjectIndex;
        int objId;
        foreach (var neighbour in neighbours)
        {
            if (building.FusionData.CompatibleID != neighbour.ID)
            {
                neighbours.Remove(neighbour);
            }
        }
        //remove the required amount of buildings
        for (int i = 0; i < building.FusionData.RequiredAmount; i++)
        {
            gameObjectIndex = _buildingsData.GetRepresentationIndex(neighbours[i].occupiedPositions[0]);
            objId = _buildingsData.GetIDOfPlacedObject(neighbours[i].occupiedPositions[0]);
            _buildingsData.RemoveObjectAt(neighbours[i].occupiedPositions[0]);
            _objectPlacer.RemoveObject(gameObjectIndex, objId, PlacingType.Fuse);
        }
        //TODO replace with a while loop for checking validity
        
        bool CanPlace = CheckPlacementValidity(gridPosition, _databaseSo.ObjectDataList[building.FusionData.ResultId].ID);
        if (!CanPlace)
        {
            gridPosition += Vector3Int.back;
            CanPlace = CheckPlacementValidity(gridPosition, _databaseSo.ObjectDataList[building.FusionData.ResultId].ID);
            if (!CanPlace)
            {
                gridPosition += Vector3Int.left;

            }
        }
        _objectPlacer.onObjectPlaced.Invoke(building.ID, PlacingType.BuyFuse);
        //create Fusion building Result
        int index = _objectPlacer.PlaceObject(building.FusionData.FusionResult,
            _grid.CellToWorld(gridPosition)+ new Vector3(0, cellIndicatorElevation, 0),
            building.FusionData.ResultId,
            PlacingType.Fuse);
        
        _buildingsData.AddObjectAt(gridPosition,_databaseSo.ObjectDataList[building.FusionData.ResultId].Size,
            _databaseSo.ObjectDataList[building.FusionData.ResultId].ID,
            index,
            _databaseSo.ObjectDataList[building.FusionData.ResultId].FusionData);
        
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);

    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedID)
    {
        //GridData selectedData = _databaseSo.ObjectDataList[selectedID].ID == 0 ? _floorData : _buildingsData;
        GridData selectedData = _buildingsData;
        return selectedData.CanPlaceObjectAt(gridPosition, _databaseSo.ObjectDataList[selectedID].Size);
    }

    public bool IsNextToRoad(Vector3Int gridPosition, Vector2Int objectSize, int roadId)
    {
        List<PlacementData> neighbours = new List<PlacementData>();
        neighbours = _buildingsData.GetNeighbours(gridPosition, objectSize);
        foreach (var neighbour in neighbours)
        {
            if (neighbour.ID == roadId)//Road Id is 2
            {
                return true;
            }
        }
        return false;
    }
    public bool CanFuseBuildings(Vector3Int gridPosition, ObjectData building)
    {
        GridData selectedData = _buildingsData;
        List<PlacementData> neighbours = selectedData.GetNeighbours(gridPosition, building.Size);
        if (neighbours.Count<=0) { return false;}

        int CurrentCompatibleNeighbours = 0;
        foreach (var neighbour in neighbours)
        {
            if (building.FusionData.CompatibleID == neighbour.ID)
            {
                CurrentCompatibleNeighbours += 1;
            }
        }

        return CurrentCompatibleNeighbours >= building.FusionData.RequiredAmount;
    }
    public void UpdateState(Vector3Int gridPosition)
    {
        bool CanPlace = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), CanPlace);
    }
}
