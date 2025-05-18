using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _spawnPool = new();
    [SerializeField] private int _maximumNumberOfObjects = 20;
    [SerializeField] private float _spawnRateSeconds = 5f;
    [SerializeField] private int _randomMaxForce = 50;
    [SerializeField] private int _randomForceMaxAngleDegrees = 45;
    
    private List<GameObject> _objectPool = new();
    private Transform[] _spawnTransforms = Array.Empty<Transform>();
    private float _spawnTimer;

    private void Start()
    {
        _spawnTransforms = GetComponentsInChildren<Transform>();
        InstantiateObjectPool();
    }

    private void InstantiateObjectPool()
    {
        var numberOfEachObject = _maximumNumberOfObjects / _spawnPool.Count;

        foreach (var prefab in _spawnPool)
        {
            for (var i = 0; i < numberOfEachObject; i++)
            {
                var newObject = Instantiate(prefab, transform.position, Quaternion.identity, transform);
                _objectPool.Add(newObject);
                newObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnRateSeconds)
        {
            SpawnObject();
            _spawnTimer = 0;
        }
    }

    public void SpawnObject()
    {
        var obj = GetObjectFromPool();

        if (!obj)
        {
            Debug.LogError($"No valid object available in the pool");
            return;
        }

        var randomSpawnLocation = GetRandomSpawnTransform();
        obj.transform.SetPositionAndRotation(randomSpawnLocation.position, randomSpawnLocation.rotation);
        obj.transform.SetParent(transform);
        obj.SetActive(true);
        
        if (obj.TryGetComponent<Rigidbody2D>(out var rigidBody))
        {
            Vector2 forceDirection = Quaternion.Euler(0, 0, 
                Random.Range(-_randomForceMaxAngleDegrees, _randomForceMaxAngleDegrees)) * Vector2.down;
            var force = Random.Range(0, _randomMaxForce);
            rigidBody.AddForce(forceDirection * force);
            Debug.Log($"Add Force: {force}");
        }
    }

    private GameObject GetObjectFromPool()
    {
        var randomIndices = Enumerable.Range(0, _objectPool.Count).OrderBy(_ => Random.value);
        return randomIndices.Select(index => _objectPool[index])
                            .FirstOrDefault(obj => obj && !obj.activeInHierarchy);
    }

    private Transform GetRandomSpawnTransform()
    {
        return _spawnTransforms[Random.Range(0, _spawnTransforms.Length)];
    }
}
