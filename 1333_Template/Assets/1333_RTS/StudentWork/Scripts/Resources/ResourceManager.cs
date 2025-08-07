using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Resource Settings")]
    [SerializeField] private int resourceAmount = 0;
    [SerializeField] private float regenInterval = 0.2f;
    [SerializeField] private int regenAmount = 1;
    [SerializeField] private bool playSFXOnRegen = true;

    public int ResourceAmmount => resourceAmount;

    private float timer;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= regenInterval)
        {
            AddResource(regenAmount);
            timer = 0;
        }
    }

    public void AddResource(int amount)
    {
        resourceAmount += amount;

        if (playSFXOnRegen)
        {
            AudioManager.Instance.PlaySFX("ResourceGenerated");
        }
    }

    public bool HasEnough(int cost) => resourceAmount >= cost;

    public bool SpendResource(int cost)
    {
        if (!HasEnough(cost)) return false;

        resourceAmount -= cost;
        return true;
    }

    public int GetResourceAmount() =>  resourceAmount; 

    public void SetResources(int value) { resourceAmount = value; }
}
