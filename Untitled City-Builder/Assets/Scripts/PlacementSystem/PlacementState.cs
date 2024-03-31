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

    public int OnAction(Vector3Int gridPosition)
    {
        bool CanPlace = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        if (!CanPlace) {return -1;}

        if (!GameManager.Singleton.CanBuy(_selectedObjectIndex)) {return -1;}

        //GridData selectedData = _databaseSo.ObjectDataList[_selectedObjectIndex].ID == 0 ? _floorData : _buildingsData;
        GridData selectedData = _buildingsData;
        
        //Fuse buildigns
        if (CanFuseBuildings(gridPosition,_databaseSo.ObjectDataList[_selectedObjectIndex]))
        {
            Fuse(gridPosition, _databaseSo.ObjectDataList[_selectedObjectIndex]);
            return _databaseSo.ObjectDataList[_selectedObjectIndex].FusionData.ResultId;
        }
        
        //Place Building
        int index = _objectPlacer.PlaceObject(_databaseSo.ObjectDataList[_selectedObjectIndex].Prefab,
            _grid.CellToWorld(gridPosition) + new Vector3(0, cellIndicatorElevation, 0));
        
        selectedData.AddObjectAt(gridPosition,_databaseSo.ObjectDataList[_selectedObjectIndex].Size,
            _databaseSo.ObjectDataList[_selectedObjectIndex].ID,
            index,
            _databaseSo.ObjectDataList[_selectedObjectIndex].FusionData);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
        return _selectedObjectIndex;
    }

    private void Fuse(Vector3Int gridPosition, ObjectData building)
    {
        //Get compatible neighbour buildings
        List<PlacementData> neighbours = _buildingsData.GetNeighbours(gridPosition, building.Size);
        int gameObjectIndex;
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
            _buildingsData.RemoveObjectAt(neighbours[i].occupiedPositions[0]);
            _objectPlacer.RemoveObject(gameObjectIndex);

        }
        //remove the building that we placed
        // gameObjectIndex = _buildingsData.GetRepresentationIndex(gridPosition);
        // _buildingsData.RemoveObjectAt(gridPosition);
        // _objectPlacer.RemoveObject(gameObjectIndex);
        //create Fusion building Result
        int index = _objectPlacer.PlaceObject(building.FusionData.FusionResult,
            _grid.CellToWorld(gridPosition)+ new Vector3(0, cellIndicatorElevation, 0));
        
        _buildingsData.AddObjectAt(gridPosition,_databaseSo.ObjectDataList[_selectedObjectIndex].Size,
            _databaseSo.ObjectDataList[_selectedObjectIndex].ID,
            index,
            _databaseSo.ObjectDataList[_selectedObjectIndex].FusionData);
        
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);

    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedID)
    {
        //GridData selectedData = _databaseSo.ObjectDataList[selectedID].ID == 0 ? _floorData : _buildingsData;
        GridData selectedData = _buildingsData;
        return selectedData.CanPlaceObjectAt(gridPosition, _databaseSo.ObjectDataList[selectedID].Size);
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
