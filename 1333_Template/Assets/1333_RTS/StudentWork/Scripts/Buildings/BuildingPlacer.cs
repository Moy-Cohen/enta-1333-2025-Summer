using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    private GameObject _buildingOverlay;
    private bool _isPlacing = false;
    private float _currentRotation = 0f;
    private Vector2Int _currentFootprint = Vector2Int.one;
    private BuildingData _buildingData;
    private Color _lastOverlayColor = Color.clear;

    private Material[] _originalMaterial;


    public void HandleOverlayAndColor()
    {
        Vector3 mousePos = _gridManager.ClampWorldToGrid(GetMouseWorldPointOnGround());
        
        GridNode centerNode = _gridManager.GetNodeFromWorldPosition(mousePos);
        if (centerNode == null) return;

        
        Vector3 basePos = centerNode.WorldPosition;

        bool canPlace = IsValidPlacement(basePos, _buildingData.footprintSize);
        SetOverlayColor(canPlace ? Color.green : Color.red, 0.5f);

        _buildingOverlay.transform.position = centerNode.WorldPosition;

    }

    private void HandlePlacementInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mousePos = GetMouseWorldPointOnGround();
        GridNode centerNode = _gridManager.GetNodeFromWorldPosition(mousePos);
        if (centerNode == null) return;

        
        Vector3 basePos = centerNode.WorldPosition;

        if (!IsValidPlacement(basePos, _currentFootprint))
        {
            Debug.Log("[Placer] Cannot place — invalid or occupied node.");
            return;
        }

        GameObject placed = Instantiate(_buildingOverlay);
        placed.transform.position = centerNode.WorldPosition;
        placed.transform.rotation = _buildingOverlay.transform.rotation;

        FinalizePlacement(placed);

        for (int dx = 0; dx < _currentFootprint.x; dx++)
        {
            for (int dy = 0; dy < _currentFootprint.y; dy++)
            {
                Vector3 checkPos = basePos + new Vector3(dx, 0, dy);
                GridNode occNode = _gridManager.GetNodeFromWorldPosition(checkPos);
                if (occNode != null)
                {
                    occNode.Occupied = true;
                    Debug.Log($"[Placer] Marked node at {occNode.WorldPosition} as occupied");
                }
            }
        }

        Debug.Log("[Placer] Final building spawned and finalized at: " + placed.transform.position);

        Destroy(_buildingOverlay);
        _buildingOverlay = null;
        _isPlacing = false;


    }

    private void FinalizePlacement(GameObject building)
    {

        foreach (var col in building.GetComponentsInChildren<Collider>())
            col.enabled = true;

        foreach (var script in building.GetComponents<MonoBehaviour>())
            script.enabled = true;

        foreach (var rend in building.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in rend.materials)
            {
                // ✅ Fully opaque white
                mat.color = new Color(1f, 1f, 1f, 1f);

                // ✅ Reset shader blending to opaque
                mat.SetFloat("_Mode", 0); // Opaque
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = -1;
            }
        }

        Debug.Log("[Placer] Final object color and render queue reset.");


    }

    private void HandleCancelInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("[Placer] Placement cancelled (right click)");
            CancelPlacement();
        }
    }

    private void CancelPlacement()
    {
        if (_buildingOverlay != null)
            Destroy(_buildingOverlay);

        _buildingOverlay = null;
        _isPlacing = false;
    }


    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentRotation += 90f;
            _currentRotation %= 360f;

            Vector3 originalEuler = _buildingOverlay.transform.rotation.eulerAngles;
            _buildingOverlay.transform.rotation = Quaternion.Euler(originalEuler.x, _currentRotation, originalEuler.z);

            Debug.Log("[Placer] Rotated ghost to Y=" + _currentRotation + " degrees");
        }
    }

    private void Update()
    {
        if (!_isPlacing || _gridManager == null || _buildingOverlay == null) return;

        HandleOverlayAndColor();
        HandleRotationInput();
        HandleCancelInput();
        HandlePlacementInput();
    }


    public void SetPrefabToPlace(BuildingData data)
    {
        Debug.Log("[Placer] UI button clicked — preparing ghost preview");

        if (_buildingOverlay != null)
            Destroy(_buildingOverlay);

        _buildingOverlay = data.BuildingPrefab;
        _currentFootprint = data.footprintSize;

        _buildingOverlay = Instantiate(data.BuildingPrefab);

        foreach (Renderer renderer in _buildingOverlay.GetComponentsInChildren<Renderer>())
        {
            renderer.materials = (Material[])renderer.materials.Clone();
        }

        _originalMaterial = GetMaterials(_buildingOverlay);
        SetOverlay(_buildingOverlay);
        _isPlacing = true;
        _currentRotation = 0f;

    }


    private bool IsValidPlacement(Vector3 basePos, Vector2Int size)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dy = 0; dy < size.y; dy++)
            {
                Vector3 checkPos = basePos + new Vector3(dx, 0, dy);
                GridNode checkNode = _gridManager.GetNodeFromWorldPosition(checkPos);
                if (checkNode == null || !checkNode.walkable || checkNode.Occupied)
                    return false;
            }
        }
        return true;
    }

    

    private void SetOverlay(GameObject building)
    {
        foreach (var col in building.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (var script in building.GetComponents<MonoBehaviour>())
            if (script != this) script.enabled = false;

        SetOverlayColor(Color.green, 0.5f); // ghost look
    }

    private void SetOverlayColor(Color baseColor, float alpha)
    {
        if (_originalMaterial == null) return;

        Color desired = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

        if (_lastOverlayColor == desired) return;

        _lastOverlayColor = desired;

        foreach (var mat in _originalMaterial)
        {
            if (mat == null) continue;

            mat.color = desired;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        Debug.Log("[Placer] Ghost material updated to transparent");
    }

    private Material[] GetMaterials(GameObject obj)
    {
        List<Material> mats = new();
        foreach (var rend in obj.GetComponentsInChildren<Renderer>())
            mats.AddRange(rend.materials);

        return mats.ToArray();
    }

    private Vector3 GetMouseWorldPointOnGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }
}
