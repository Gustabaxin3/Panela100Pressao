using AUDIO;
using System;
using UnityEngine;

public class Captain : ISoldierState {
    [SerializeField] private bool _isActive = false;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpStrength = 30f;
    [SerializeField] private float _customGravity = -15f;
    [SerializeField] private event System.Action Jumped;

    [SerializeField] private GroundCheck _groundCheck;

    private bool _isJumping;

    protected override void Start() {
        base.Start();
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }

    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Captain state entered.");
        _isActive = true;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (Input.GetButtonUp("Jump") && _rigidBody.linearVelocity.y > 0f) {
            _rigidBody.linearVelocity = new Vector3(_rigidBody.linearVelocity.x, _rigidBody.linearVelocity.y * 0.5f, _rigidBody.linearVelocity.z);
            
            string[] sounds =
            {
                "Audio/Pulo/SoldadoPulo01",
                "Audio/Pulo/SoldadoPulo02",
                "Audio/Pulo/SoldadoPulo03",
                "Audio/Pulo/SoldadoPulo04"
            };

            int numSorteado = UnityEngine.Random.Range(0, sounds.Length);
            AudioManager.Instance.PlaySoundEffect(sounds[numSorteado]);

            /*
            AudioManager.Instance.PlaySoundEffect(
                sounds[numSorteado],
                position: _transform.position,
                spatialBlend: 1

                );
            */
        }
    }

    public override void LateUpdate() {
        base.LateUpdate();
        if (Input.GetButtonDown("Jump") && (!_groundCheck || _groundCheck.isGrounded) && _isActive) {
            _rigidBody.AddForce(Vector3.up * _jumpStrength, ForceMode.Impulse);
            _isJumping = true;
            Jumped?.Invoke();
        }
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
        if (!_groundCheck || !_groundCheck.isGrounded) {
            _rigidBody.AddForce(Vector3.up * _customGravity, ForceMode.Acceleration);
        }
    }

    public override void OnExit() {
        base.OnExit();
        _isActive = false;
        _isJumping = false;
    }
}
