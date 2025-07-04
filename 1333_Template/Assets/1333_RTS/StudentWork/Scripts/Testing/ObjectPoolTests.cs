using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolTests : MonoBehaviour
{
    IObjectPool<ArcingProjectile> _myPool;
    [SerializeField] private ArcingProjectile ProjectilePrefab;
    
    public void Start()
    {
        _myPool = new ObjectPool<ArcingProjectile>(OnCreatePool, OnGetFromPool, OnReturnToPool, OnDestroyPoolObject, collectionCheck, defaultPoolCapacity, maxPoolSize);
    }

    

    public ArcingProjectile OnCreatePool()
    {
        // Instantiate with generic overload so no GetComponent is needed.
        var proj =
            Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);

        return proj;
        
    }

    private void OnGetFromPool(ArcingProjectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private void OnReturnToPool(ArcingProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(ArcingProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    private bool collectionCheck;
    
    private int defaultPoolCapacity = 10;

    private int maxPoolSize = 100;
    
}
