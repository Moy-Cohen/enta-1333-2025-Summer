using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCarousel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private UnitDatabase unitDatabase;

    [Header("UI Cards")]
    [SerializeField] private UnitCardUI prevCard;
    [SerializeField] private UnitCardUI currentCard;
    [SerializeField] private UnitCardUI nextCard;

    public UnitType ActiveUnit => units[currentIndex];

    List<UnitType> units;
    int currentIndex;

    private void Awake()
    {
        if (unitDatabase == null)
        {
            unitDatabase = FindObjectOfType<UnitDatabase>();
            if (unitDatabase == null)
            {
                Debug.LogError("UnitCarousel: No UnitDatabase found in the scene!", this);
                enabled = false;
                return;
            }
        }

        if (unitDatabase.PlayerUnits !=  null)
        {
            units = new List<UnitType>(unitDatabase.PlayerUnits);
        }

        if (units.Count == 0)
        {
            Debug.LogWarning("UnitCarousel: playerUnits list is empty.");
            enabled = false;
            return;
        }

        currentIndex = 0;
        Refresh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Shift(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Shift(+1);
        }
    }

    private void Shift(int dir)
    {
        currentIndex = (currentIndex + dir + units.Count) % units.Count;
        Refresh();
    }

    private void Refresh()
    {
        if (units.Count == 0) return;

        int prev = (currentIndex - 1 + units.Count) % units.Count;
        int next = (currentIndex + 1) % units.Count;

        prevCard.Bind(units[prev]);
        currentCard.Bind(units[currentIndex]);
        nextCard.Bind(units[next]);
    }

}
