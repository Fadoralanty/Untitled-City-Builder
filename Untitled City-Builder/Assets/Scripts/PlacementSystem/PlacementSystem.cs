using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    public ObjectsDatabaseSO ObjectsDatabaseSo => objectsDatabaseSo;
    [SerializeField] private ObjectsDatabaseSO objectsDatabaseSo;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PlacementPreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer objectPlacer;
    private GridData floorData;
    private GridData buildingData;
    private Vector3Int _lastDetectedPosition = Vector3Int.zero;
    private IBuildingState _currentBuildingState;
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
        _currentBuildingState = new PlacementState(ID,grid,previewSystem, objectsDatabaseSo, floorData,buildingData, objectPlacer);
        inputManager.OnClicked += PlaceBuilding;
        inputManager.OnExit += StopPlacementMode;
    }
    
    private void StopPlacementMode()
    {
        if (_currentBuildingState == null) { return; }
        
        gridVisualization.SetActive(false);
        _currentBuildingState.EndState();
        inputManager.OnClicked -= PlaceBuilding;
        inputManager.OnExit -= StopPlacementMode;
        inputManager.OnClicked -= RemoveBuilding;

        _lastDetectedPosition= Vector3Int.zero;
        _currentBuildingState = null;
        
    }

    public void StartRemoving()
    {
        StopPlacementMode();
        gridVisualization.SetActive(true);
        _currentBuildingState = new RemovingState(grid, previewSystem, floorData, buildingData, objectPlacer);
        inputManager.OnClicked += RemoveBuilding;
        inputManager.OnExit += StopPlacementMode;
    }

    private void ExecuteBuildingState()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        _currentBuildingState.OnAction(gridPosition);
    }
    private void PlaceBuilding()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        ExecuteBuildingState();
    }    
    private void RemoveBuilding()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        ExecuteBuildingState();
    }

    // private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedID)
    // {
    //     GridData selectedData = objectsDatabaseSo.ObjectDataList[selectedID].ID == 0 ? floorData : buildingData;
    //     return selectedData.CanPlaceObjectAt(gridPosition, objectsDatabaseSo.ObjectDataList[selectedID].Size);
    // }

    private void Update()
    {
        if (_currentBuildingState == null) { return; }
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (_lastDetectedPosition == gridPosition) { return; }
        
        _currentBuildingState.UpdateState(gridPosition);
        _lastDetectedPosition = gridPosition;
    }
}
