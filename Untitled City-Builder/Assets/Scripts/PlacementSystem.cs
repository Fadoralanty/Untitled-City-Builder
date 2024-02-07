using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private float cellIndicatorElevation=0.01f;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO objectsDatabaseSo;
    [SerializeField] private GameObject gridVisualization;
    private int selectedObjectID=-1;

    private void Start()
    {
        StopPlacementMode();
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
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceBuilding;
        inputManager.OnExit += StopPlacementMode;
    }

    private void StopPlacementMode()
    {
        selectedObjectID = -1;
        
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceBuilding;
        inputManager.OnExit -= StopPlacementMode;
    }

    private void PlaceBuilding()
    {
        if (inputManager.IsPointerOverUI()) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        GameObject newBuilding = Instantiate(objectsDatabaseSo.ObjectDataList[selectedObjectID].Prefab);
        newBuilding.transform.position = grid.CellToWorld(gridPosition) + new Vector3(0, cellIndicatorElevation, 0);
    }

    private void Update()
    {
        if (selectedObjectID < 0) { return; }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition) + new Vector3(0, cellIndicatorElevation, 0);
    }
}
