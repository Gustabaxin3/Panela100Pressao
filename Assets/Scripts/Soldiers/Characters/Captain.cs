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

    [Header("Script")]
    [SerializeField] private SoldierMovement soldierMovement;

    [Header("Double Jump Settings")]
    [SerializeField] private int _maxJumps = 2;

    public bool _isJumping;
    private int _jumpCount = 0;

    protected override void Awake() {
        base.Awake();
        soldierType = SoldierType.Captain;
    }
    protected override void Start() {
        base.Start();
        _groundCheck = GetComponentInChildren<GroundCheck>();
        soldierMovement = GetComponent<SoldierMovement>();
    }

    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Captain state entered.");
        _isActive = true;
        _jumpCount = 0;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (Input.GetButtonUp("Jump") && _rigidBody.linearVelocity.y > 0f) {
            soldierMovement.CheckJump(true);
            _rigidBody.linearVelocity = new Vector3(_rigidBody.linearVelocity.x, _rigidBody.linearVelocity.y * 0.5f, _rigidBody.linearVelocity.z);

            string[] soundsPulo =
            {
                "Audio/Pulo/SoldadoPulo01",
                "Audio/Pulo/SoldadoPulo02",
                "Audio/Pulo/SoldadoPulo03",
                "Audio/Pulo/SoldadoPulo04"
            };

            int numSorteado = UnityEngine.Random.Range(0, soundsPulo.Length);
            AudioManager.Instance.PlaySoundEffect(soundsPulo[numSorteado], position: transform.position, spatialBlend: 0);
        }
    }

    public override void LateUpdate() {
        base.LateUpdate();

        if (_groundCheck && _groundCheck.isGrounded)
        {
            _jumpCount = 0;
            _isJumping = false;
            soldierMovement.CheckJump(false);
        }

        if (Input.GetButtonDown("Jump") && _isActive) {
            
           
            if ((_groundCheck && _groundCheck.isGrounded) || _jumpCount < _maxJumps) {
                
                float velY = _rigidBody.linearVelocity.y;
                float compensacao = velY < 0 ? -velY : 0f;
                _rigidBody.linearVelocity = new Vector3(_rigidBody.linearVelocity.x, 0f, _rigidBody.linearVelocity.z);
                Vector3 jumpForce = Vector3.up * (_jumpStrength + compensacao);
                _rigidBody.AddForce(jumpForce, ForceMode.Impulse);


                _isJumping = true;
                _jumpCount++;
                Jumped?.Invoke();
                soldierMovement.CheckJump(true);
                string[] soundsPulo =
                {
                    "Audio/Pulo/SoldadoPulo01",
                    "Audio/Pulo/SoldadoPulo02",
                    "Audio/Pulo/SoldadoPulo03",
                    "Audio/Pulo/SoldadoPulo04"
                };
                int numSorteado = UnityEngine.Random.Range(0, soundsPulo.Length);
                AudioManager.Instance.PlaySoundEffect(soundsPulo[numSorteado], position: transform.position, spatialBlend: 0);
            }
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
