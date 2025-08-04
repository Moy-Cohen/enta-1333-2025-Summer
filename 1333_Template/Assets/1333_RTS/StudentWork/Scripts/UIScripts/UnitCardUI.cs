using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitCardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dmgText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text costText;

    public void Bind(UnitType unit)
    {
        icon.sprite = unit.UnitIcon;
        nameText.text = $"{unit.UnitName}";
        dmgText.text = $"Damage: {unit.Damage}";
        hpText.text = $"HP: {unit.Durability}";
        costText.text = $"Cost: {unit.UnitCost}";

    }
}
