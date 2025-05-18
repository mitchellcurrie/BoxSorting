using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Box : MonoBehaviour
{
    [field: SerializeField] public BoxColour Colour { get; private set; }
    
    private Rigidbody2D _rigidbody; 

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
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
        
        _rigidbody.AddForce(dropForce);
    }
}
