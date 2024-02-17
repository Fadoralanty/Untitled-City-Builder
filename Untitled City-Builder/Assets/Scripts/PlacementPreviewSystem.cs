using UnityEngine;

public class PlacementPreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewOffset=0.06f;

    [SerializeField] private GameObject cellIndicator;
    private GameObject _previewObject;

    [SerializeField] private Material previewMaterial;
    private Material _previewMaterialInstance;

    [SerializeField] private Renderer cellIndicatorRenderer;
    private void Start()
    {
        _previewMaterialInstance = new Material(previewMaterial);
        cellIndicator.SetActive(false);
        
    }

    public void ShowPlacementPreview(GameObject prefab, Vector2Int size)
    {
        _previewObject = Instantiate(prefab);
        PreparePreview(_previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
        
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x>0 || size.y>0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = _previewMaterialInstance;
            }

            renderer.materials = materials;
        }
    }

    public void HidePreview()
    {
        cellIndicator.SetActive(false);
        if (_previewObject!=null)
        {
            Destroy(_previewObject);
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (_previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }
        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        _previewMaterialInstance.color = c;
        
    }
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
        
    }

    private void MovePreview(Vector3 position)
    {
        _previewObject.transform.position = new Vector3(position.x, position.y + previewOffset, position.z);
        
    }

    public void ShowRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
        
    }
}
