using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourceCounter;
    // Update is called once per frame
    void Update()
    {
        resourceCounter.text = $"Resources: {ResourceManager.Instance.ResourceAmmount}";
    }
}
