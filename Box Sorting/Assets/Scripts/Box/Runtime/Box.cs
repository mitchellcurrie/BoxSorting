using System;
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

    public void OnDropped()
    {
        // Re-enable effects of gravity
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
