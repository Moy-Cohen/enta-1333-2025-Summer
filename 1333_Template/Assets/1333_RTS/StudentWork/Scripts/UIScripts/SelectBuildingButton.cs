using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Image _buttonImage;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private Button _button;

    private BuildingPlacer _buildingPlacer;


    private BuildingData _buildingDataForButton;

    public void SetUp(BuildingData buildingData)
    {
        _buildingDataForButton = buildingData;
        _buttonText.text = buildingData.BuildingName;
        _buttonImage.sprite = buildingData.BuildingIcon;

        _buildingPlacer = GameObject.FindAnyObjectByType<BuildingPlacer>();
    }

    public void OnClickPlaceBuilding()
    {
        if (_buildingPlacer != null && _buildingDataForButton != null)
        {
            _buildingPlacer.SetPrefabToPlace(_buildingDataForButton);
        }
    }
}
