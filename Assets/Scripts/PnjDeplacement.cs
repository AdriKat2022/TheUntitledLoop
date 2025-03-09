using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public struct inputHistory
{
    public float time;
    public float value;

    public inputHistory(float time, float value)
    {
        this.time = time;
        this.value = value;
    }

    public string ToString()
    {
        return "time : " + time + ",value : " + value;
    }
}

public class PnjDeplacement : MonoBehaviour
{

    public Vector3 initialPosition;
    public bool reproduce = true;
    public List<inputHistory> history = new List<inputHistory>();
    private int currentInputId = 0;
    private Coroutine co;

    private float _currentHorizontalInput;
    [SerializeField] private float _acceleration = 50f;
    [SerializeField] private float _deceleration = 50f;
    private Rigidbody2D _rigidbody;

    [HideInInspector]
    public int SpeedMultiplier = 100; // Modified by something external
    [SerializeField] private float _topSpeed = 2f;

    private IEnumerator ReproduceInput()
    {
        while (reproduce && currentInputId < history.Count)
        {
            _currentHorizontalInput = history[currentInputId].value;
            yield return new WaitForSeconds(history[currentInputId].time);
            currentInputId++;
        }
        StartCoroutine(GoBackToSpawn());
    }

    private IEnumerator GoBackToSpawn()
    {
        float distance;
        while((distance = Vector3.Distance(transform.position, initialPosition)) > 0.1)
        {
            _currentHorizontalInput = Mathf.Clamp01(distance) * Mathf.Sign((initialPosition - transform.position ).x);
            yield return new WaitForFixedUpdate();
        }
        _currentHorizontalInput = 0;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ManageMovement();
    }

    private void ManageMovement()
    {
        if (_currentHorizontalInput != 0)
        {
            // Normalize the input if needed
            float targetVelocity = (SpeedMultiplier / 100f) * _topSpeed * _currentHorizontalInput;

            float acceleration = (targetVelocity - _rigidbody.velocity.x) * _acceleration;
            _rigidbody.AddForce(acceleration * Vector2.right, ForceMode2D.Force);
        }
        else
        {
            Vector2 deceleration = -_rigidbody.velocity * _deceleration;
            _rigidbody.AddForce(deceleration, ForceMode2D.Force);
            
        }
    }

    public void Launch() 
    { 
        currentInputId = 0;
        if(co != null) StopCoroutine(co);
        co = StartCoroutine(ReproduceInput());
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
