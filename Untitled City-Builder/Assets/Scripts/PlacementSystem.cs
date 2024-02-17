using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO objectsDatabaseSo;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PlacementPreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;
    private GridData floorData;
    private GridData buildingData;
    private Vector3Int _lastDetectedPosition = Vector3Int.zero;
    private IBuildingState _buildingState;
    
    private void Start()
    {
        StopPlacementMode();
        floorData = new GridData();
        buildingData = new GridData();
    }

    public void StartPlacement(int ID)
    {
        StopPlacementMode();
        gridVisualization.SetActive(true);
        _buildingState = new PlacementState(ID,grid,previewSystem, objectsDatabaseSo, floorData,buildingData, objectPlacer);
        
        inputManager.OnClicked += PlaceBuilding;
        inputManager.OnExit += StopPlacementMode;
    }

    private void StopPlacementMode()
    {
        if (_buildingState == null) { return; }
        
        gridVisualization.SetActive(false);
        _buildingState.EndState();
        inputManager.OnClicked -= PlaceBuilding;
        inputManager.OnExit -= StopPlacementMode;
        _lastDetectedPosition= Vector3Int.zero;
        _buildingState = null;
        
    }

    private void PlaceBuilding()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        _buildingState.OnAction(gridPosition);

    }

    // private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedID)
    // {
    //     GridData selectedData = objectsDatabaseSo.ObjectDataList[selectedID].ID == 0 ? floorData : buildingData;
    //     return selectedData.CanPlaceObjectAt(gridPosition, objectsDatabaseSo.ObjectDataList[selectedID].Size);
    // }

    private void Update()
    {
        if (_buildingState == null) { return; }
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (_lastDetectedPosition == gridPosition) { return; }
        
        _buildingState.UpdateState(gridPosition);
        _lastDetectedPosition = gridPosition;
    }
}
