using Box.Data;
using System.Collections;
using UnityEngine;

namespace Box.Runtime
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BoxController : MonoBehaviour
    {
        [field: SerializeField] public BoxColour Colour { get; private set; }
        
        [SerializeField] private int _rotationMaxForce = 30;
        [SerializeField] private int _timeUntilFade = 5;
        [SerializeField] private float _fadeDuration = 2;
        [SerializeField] private float _ignoreTimeAfterDropped = 1.5f;
    
        private const string IGNORE_RAYCAST_LAYER = "Ignore Raycast";
        private const string DEFAULT_LAYER = "Default";

        private Transform _spawnerParent;
        private Rigidbody2D _rigidbody; 
        private SpriteRenderer _renderer;
        private Color _colour;
        private bool _inTargetZone;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _colour = _renderer.color;
        }

        private void Start()
        {
            _spawnerParent = transform.parent;
        }
        
        private void OnEnable()
        {
            // Sets values to default when the box is spawned by the object pool
            gameObject.layer = LayerMask.NameToLayer(DEFAULT_LAYER);
            _renderer.color = _colour;
            _inTargetZone = false;
        }
        
        public void SetOriginalParent()
        {
            transform.parent = _spawnerParent;
        }
        
        public void OnPickedUp()
        {
            // Stop the effects of gravity, any existing velocity, and prevent character raycasts colliding
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            gameObject.layer = LayerMask.NameToLayer(IGNORE_RAYCAST_LAYER);
        }

        public void OnReleased(Vector2 releaseForce)
        {
            // Re-enable effects of gravity
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            
            // Add a random torque to rotate the released box and add the dropping / throwing force
            _rigidbody.AddTorque(Random.Range(-_rotationMaxForce, _rotationMaxForce));
            _rigidbody.AddForce(releaseForce);
            
            StartCoroutine(DelayedSetLayerToDefault());
        }
        
        private IEnumerator DelayedSetLayerToDefault()
        {
            yield return new WaitForSeconds(_ignoreTimeAfterDropped);
            
            if (!_inTargetZone)
            {
                // If the box has been released and is not in the target zone, set the layer back to default to allow
                // the NPC to see it
                gameObject.layer = LayerMask.NameToLayer(DEFAULT_LAYER);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Target"))
            {
                gameObject.layer = LayerMask.NameToLayer(IGNORE_RAYCAST_LAYER);
                _inTargetZone = true;
                
                // Box has reached the target. Start the despawn coroutine
                StartCoroutine(DelayedDespawnRoutine());
            }
        
            if (other.gameObject.CompareTag("SpawnArea"))
            {
                // Adds a random rotation to boxes when they are spawned at the top of the screen
                _rigidbody.AddTorque(Random.Range(-_rotationMaxForce, _rotationMaxForce));
            }
        }
        
        private IEnumerator DelayedDespawnRoutine()
        {
            yield return new WaitForSeconds(_timeUntilFade);
      
            var timer = 0f;

            // Fade sprite alpha to 0 over time
            while (timer < _fadeDuration)
            {
                var alpha = Mathf.Lerp(1f, 0f, timer / _fadeDuration);
                _renderer.color = new Color(_colour.r, _colour.g, _colour.b, alpha);
                
                timer += Time.deltaTime;
                yield return null;
            }
        
            _renderer.color = new Color(_colour.r, _colour.g, _colour.b, 0);
            gameObject.SetActive(false);
        }
    }
}
