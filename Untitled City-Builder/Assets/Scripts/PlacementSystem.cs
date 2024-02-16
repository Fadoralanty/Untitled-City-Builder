using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private float cellIndicatorElevation=0.01f;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO objectsDatabaseSo;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private PlacementPreviewSystem previewSystem;
    private int selectedObjectID=-1;
    private GridData floorData;
    private GridData buildingData;
    private List<GameObject> placedGameObjects = new List<GameObject>();
    private Vector3Int _lastDetectedPosition = Vector3Int.zero;
    private void Start()
    {
        StopPlacementMode();
        floorData = new GridData();
        buildingData = new GridData();
    }

    public void StartPlacement(int ID)
    {
        StopPlacementMode();
        selectedObjectID = objectsDatabaseSo.ObjectDataList.FindIndex(data => data.ID == ID);
        if (selectedObjectID<0)
        {
            Debug.LogError($"No Id Found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        previewSystem.ShowPlacementPreview(objectsDatabaseSo.ObjectDataList[selectedObjectID].Prefab,
            objectsDatabaseSo.ObjectDataList[selectedObjectID].Size);
        
        inputManager.OnClicked += PlaceBuilding;
        inputManager.OnExit += StopPlacementMode;
    }

    private void StopPlacementMode()
    {
        selectedObjectID = -1;
        
        gridVisualization.SetActive(false);
        previewSystem.HidePreview();
        inputManager.OnClicked -= PlaceBuilding;
        inputManager.OnExit -= StopPlacementMode;
        _lastDetectedPosition= Vector3Int.zero;
    }

    private void PlaceBuilding()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        bool CanPlace = CheckPlacementValidity(gridPosition, selectedObjectID);
        if (!CanPlace) {return; }
        
        GameObject newBuilding = Instantiate(objectsDatabaseSo.ObjectDataList[selectedObjectID].Prefab);
        newBuilding.transform.position = grid.CellToWorld(gridPosition) + new Vector3(0, cellIndicatorElevation, 0);
        placedGameObjects.Add(newBuilding);
        GridData selectedData = objectsDatabaseSo.ObjectDataList[selectedObjectID].ID == 0 ? floorData : buildingData;
        selectedData.AddObjectAt(gridPosition,objectsDatabaseSo.ObjectDataList[selectedObjectID].Size,
            objectsDatabaseSo.ObjectDataList[selectedObjectID].ID,
            placedGameObjects.Count-1);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        

    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedID)
    {
        GridData selectedData = objectsDatabaseSo.ObjectDataList[selectedID].ID == 0 ? floorData : buildingData;
        return selectedData.CanPlaceObjectAt(gridPosition, objectsDatabaseSo.ObjectDataList[selectedID].Size);
    }

    private void Update()
    {
        if (selectedObjectID < 0) { return; }
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (_lastDetectedPosition == gridPosition) { return; }
        
        bool CanPlace = CheckPlacementValidity(gridPosition, selectedObjectID);
        
        mouseIndicator.transform.position = mousePosition;
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), CanPlace);
        _lastDetectedPosition = gridPosition;
    }
}
