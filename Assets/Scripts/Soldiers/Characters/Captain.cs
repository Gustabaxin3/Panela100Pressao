using AUDIO;
using System;
using UnityEngine;
public class Captain : ISoldierState
{
    [SerializeField] private bool _isActive = false;

    [SerializeField] private float _jumpStrength = 2;
    [SerializeField] private event System.Action Jumped;

    [SerializeField] private GroundCheck _groundCheck;

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
    }
    public override void LateUpdate() {
        if (Input.GetButtonDown("Jump") && (!_groundCheck || _groundCheck.isGrounded) && _isActive) {
            //PlayPlayerJumpSound();
            SoundManager.PlaySound(SoundType.SOLD_PULO);
            _rigidBody.AddForce(_jumpStrength * 100 * Vector3.up);
            Jumped?.Invoke();
        }
    }
    private void PlayPlayerJumpSound() {
        string[] jumpSounds = {
            "Audio/Pulo/SoldadoPulo01",
            "Audio/Pulo/SoldadoPulo02",
            "Audio/Pulo/SoldadoPulo03",
            "Audio/Pulo/SoldadoPulo04"
        };

        int index = UnityEngine.Random.Range(0, jumpSounds.Length);

        AudioManager.Instance.PlaySoundEffect(jumpSounds[index]);
    }
    public override void OnExit()
    {
        base.OnExit();

        _isActive = false;
    }
}