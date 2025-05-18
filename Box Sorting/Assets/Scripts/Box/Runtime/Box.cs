using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Box : MonoBehaviour
{
    [field: SerializeField] public BoxColour Colour { get; private set; }
    [SerializeField] private int _rotationMaxForce = 20;
    [SerializeField] private int _timeUntilFade = 15;
    [SerializeField] private float _fadeDuration = 2;
    
    private Rigidbody2D _rigidbody; 
    private SpriteRenderer _renderer;
    private Color _colour;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _colour = _renderer.color;
    }

    public void OnPickedUp()
    {
        // Stop effects of gravity and any existing velocity
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
    }

    public void OnDropped(Vector2 dropForce)
    {
        // Re-enable effects of gravity
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        
        // Set to ignore raycasts so the NPC character doesn't try and pick it up again
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        _rigidbody.AddTorque(Random.Range(-_rotationMaxForce, _rotationMaxForce));
        _rigidbody.AddForce(dropForce);
    }

    public void DelayedDespawn()
    {
        StartCoroutine(DelayedDespawnRoutine());
    }

    private IEnumerator DelayedDespawnRoutine()
    {
        yield return new WaitForSeconds(_timeUntilFade);
      
        var timer = 0f;

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
        _renderer.color = _colour;
    }
}
