using UnityEngine;

public class Fan : MonoBehaviour {
    [SerializeField] private Transform _rotationCenter;

    [Range(0f, 1000f)]
    [SerializeField] private float _rotationSpeed = 300f;

    private Vector3 _rotationAxis = Vector3.up;

    private void Update() => transform.RotateAround(_rotationCenter.position, _rotationAxis.normalized, _rotationSpeed * Time.deltaTime);
    
}
