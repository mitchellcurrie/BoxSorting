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

        public void OnPickedUp()
        {
            // Stop the effects of gravity and any existing velocity
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            gameObject.layer = LayerMask.NameToLayer(IGNORE_RAYCAST_LAYER);
        }

        public void OnReleased(Vector2 releaseForce)
        {
            // Re-enable effects of gravity
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        
            // Set to ignore raycasts so the NPC character doesn't try and pick it up again // TODO: Elaborate here after changes
            gameObject.layer = LayerMask.NameToLayer(IGNORE_RAYCAST_LAYER);
            _rigidbody.AddTorque(Random.Range(-_rotationMaxForce, _rotationMaxForce));
            _rigidbody.AddForce(releaseForce);
            
            StartCoroutine(DelayedSetLayerToDefault());
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Target"))
            {
                gameObject.layer = LayerMask.NameToLayer(IGNORE_RAYCAST_LAYER);
                _inTargetZone = true;
                
                StartCoroutine(DelayedDespawnRoutine());
            }
        
            if (other.gameObject.CompareTag("SpawnArea"))
            {
                _rigidbody.AddTorque(Random.Range(-_rotationMaxForce, _rotationMaxForce));
            }
        }
    
        private IEnumerator DelayedSetLayerToDefault()
        {
            yield return new WaitForSeconds(_ignoreTimeAfterDropped);
            
            if (!_inTargetZone)
            {
                gameObject.layer = LayerMask.NameToLayer(DEFAULT_LAYER);
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
    
        private void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer(DEFAULT_LAYER);
            _renderer.color = _colour;
            _inTargetZone = false;
        }
    }
}
