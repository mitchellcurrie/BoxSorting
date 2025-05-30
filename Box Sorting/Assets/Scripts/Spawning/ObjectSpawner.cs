using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawning
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _spawnPool = new();
        [SerializeField] private int _maximumNumberOfObjects = 20;
        [SerializeField] private Vector2 _spawnRateMinMaxSeconds;
        [SerializeField] private int _randomMaxForce = 50;
    
        private const string SPAWNED_OBJECTS_PARENT_NAME = "Spawned Objects Parent";
        
        private readonly List<GameObject> _objectPool = new();
        private Transform[] _spawnPoints = Array.Empty<Transform>();
        private Transform _spawnedObjectsParent;
        private float _spawnTimer;
        private float _spawnRate;

        private void Start()
        {
            _spawnPoints = GetComponentsInChildren<Transform>();
            _spawnedObjectsParent = new GameObject(SPAWNED_OBJECTS_PARENT_NAME).transform;
            _spawnedObjectsParent.SetParent(transform);
            
            InstantiateObjectPool();
            SetRandomSpawnRate();
        }

        private void InstantiateObjectPool()
        {
            var numberOfEachObject = _maximumNumberOfObjects / _spawnPool.Count;

            // Instantiate all objects and set them to inactive until they are required
            foreach (var prefab in _spawnPool)
            {
                for (var i = 0; i < numberOfEachObject; i++)
                {
                    var newObject = Instantiate(prefab, transform.position, Quaternion.identity, _spawnedObjectsParent);
                    newObject.SetActive(false);
                    _objectPool.Add(newObject);
                }
            }
        }

        private void Update()
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _spawnRate)
            {
                SpawnObject();
                _spawnTimer = 0;
            }
        }

        private void SpawnObject()
        {
            var objectToSpawn = GetRandomObjectFromPool();

            if (!objectToSpawn)
            {
                Debug.Log("No valid object available in the pool");
                return;
            }

            var randomSpawnPoint = GetRandomSpawnPoint();
            
            objectToSpawn.transform.SetPositionAndRotation(randomSpawnPoint.position, randomSpawnPoint.rotation);
            objectToSpawn.transform.SetParent(_spawnedObjectsParent);
            objectToSpawn.SetActive(true);
        
            if (objectToSpawn.TryGetComponent<Rigidbody2D>(out var rigidBody))
            {
                Vector2 forceDirection = randomSpawnPoint.right;
                var force = Random.Range(0, _randomMaxForce);
                rigidBody.AddForce(forceDirection * force);
            }

            SetRandomSpawnRate();
        }

        public void ResetObjects()
        {
            // Used when the reset button on the HUD is pressed
            foreach (var obj in _objectPool)
            {
                obj.SetActive(false);
                obj.transform.SetParent(_spawnedObjectsParent);
            }
        }

        private GameObject GetRandomObjectFromPool()
        {
            var randomIndices = Enumerable.Range(0, _objectPool.Count).OrderBy(_ => Random.value);
            return randomIndices.Select(index => _objectPool[index])
                                .FirstOrDefault(obj => obj && !obj.activeInHierarchy);
        }

        private Transform GetRandomSpawnPoint()
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        }

        private void SetRandomSpawnRate()
        {
            _spawnRate = Random.Range(_spawnRateMinMaxSeconds.x, _spawnRateMinMaxSeconds.y);
        }
    }
}
