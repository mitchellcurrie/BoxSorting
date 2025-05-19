using System;
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
    
    private const string SPAWNED_OBJECTS_PARENT_NAME = "Spawned Objects Parent";
    private List<GameObject> _objectPool = new();
    private Transform[] _spawnTransforms = Array.Empty<Transform>();
    private Transform _spawnedObjectsParent;
    private float _spawnTimer;

    private void Start()
    {
        _spawnTransforms = GetComponentsInChildren<Transform>();
        _spawnedObjectsParent = new GameObject(SPAWNED_OBJECTS_PARENT_NAME).transform;
        _spawnedObjectsParent.SetParent(transform);
        InstantiateObjectPool();
    }

    private void InstantiateObjectPool()
    {
        var numberOfEachObject = _maximumNumberOfObjects / _spawnPool.Count;

        foreach (var prefab in _spawnPool)
        {
            for (var i = 0; i < numberOfEachObject; i++)
            {
                var newObject = Instantiate(prefab, transform.position, Quaternion.identity, _spawnedObjectsParent);
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
        obj.transform.SetParent(_spawnedObjectsParent);
        obj.SetActive(true);
        
        if (obj.TryGetComponent<Rigidbody2D>(out var rigidBody))
        {
            Vector2 forceDirection = randomSpawnLocation.right;
            var force = Random.Range(0, _randomMaxForce);
            rigidBody.AddForce(forceDirection * force);
        }
    }

    public void ResetObjects()
    {
        foreach (var obj in _objectPool)
        {
            obj.SetActive(false);
            obj.transform.SetParent(_spawnedObjectsParent);
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
