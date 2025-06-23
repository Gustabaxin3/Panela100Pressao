using AUDIO;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SoldierManager : MonoBehaviour {
    [SerializeField] private ISoldierState _currentSoldier;

    [Header("Captain")]
    public Captain captain;
    [SerializeField] private LayerMask _captainLayerMask;
    [field: SerializeField] public bool IsCaptainUnlocked { get; private set; } = true;
    [field: SerializeField] public bool IsCaptainActive { get; private set; } = true;

    [Header("Sublieutenant")]
    public Sublieutenant sublieutenant;
    [SerializeField] private LayerMask _sublieutenantLayerMask;
    [field: SerializeField] public bool IsSublieutenantUnlocked { get; private set; } = false;
    [field: SerializeField] public bool IsSublieutenantActive { get; private set; } = true;

    [Header("Sargeant")]
    public Sargeant sargeant;
    [SerializeField] private LayerMask _sargeantLayerMask;
    [field: SerializeField] public bool IsSargeantUnlocked { get; private set; } = false;
    [field: SerializeField] public bool IsSargeantActive { get; private set; } = true;

    [Header("Cadet")]
    public Cadet cadet;
    [SerializeField] private LayerMask _cadetLayerMask;
    [field: SerializeField] public bool IsCadetUnlocked { get; private set; } = false;
    [field: SerializeField] public bool IsCadetActive { get; private set; } = true;

    [SerializeField] private Animator _animator;
    private float _startAnimationDuration;
    private float _endAnimationDuration;

    private bool _isTransitioning = false;
    public bool IsTransitioning => _isTransitioning;


    [field: SerializeField] public Transform _originalParent { get; private set; }

    public static event Action<SoldierType> OnSoldierChanged;

    private void Start() {
        MakeAllSoldiersImmobile();
        ChangeState(_currentSoldier);
        _animator.SetBool("End", true);
        StartCoroutine(DisableAfterTime(_animator.GetCurrentAnimatorStateInfo(0).length, "End"));
    }

    private void OnEnable() {
        SoldierUnlockEvents.OnSoldierUnlocked += UnlockSoldier;
    }

    private void OnDisable() {
        SoldierUnlockEvents.OnSoldierUnlocked -= UnlockSoldier;
    }

    private void Update() {
        _currentSoldier?.OnUpdate();
    }
    private void FixedUpdate() {
        _currentSoldier?.FixedUpdate();
    }
    private void LateUpdate() {
        _currentSoldier?.LateUpdate();
    }
    private void ChangeState(ISoldierState newState) {
        _currentSoldier?.OnExit();
        _currentSoldier = newState;
        _currentSoldier.OnEnter(this);
    }

    public void SelectCaptain() => ChangeCharacter(IsCaptainUnlocked, captain);
    public void SelectSublieutenant() => ChangeCharacter(IsSublieutenantUnlocked, sublieutenant);
    public void SelectSargeant() => ChangeCharacter(IsSargeantUnlocked, sargeant);
    public void SelectCadet() => ChangeCharacter(IsCadetUnlocked, cadet);

    public void ChangeCharacter(bool soldierUnlocked, ISoldierState soldierState) {
        if (soldierUnlocked && _currentSoldier != soldierState && !_isTransitioning) {
            StartCoroutine(StartAnimation(soldierState));
        }
    }

    private IEnumerator StartAnimation(ISoldierState soldierState) {
        _isTransitioning = true;

        AudioManager.Instance.PlaySoundEffect("Audio/UI/TrocaPersonagem", volume: 0.5f, position: transform.position, spatialBlend: 0);

        _currentSoldier.GetComponent<SoldierMovement>().SetMovementEnabled(false);

        _startAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
        _endAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).length;

        _animator.SetBool("Start", true);
        yield return new WaitForSeconds(_startAnimationDuration);

        _animator.SetBool("Start", false);

        ChangeState(soldierState);
        OnSoldierChanged?.Invoke(GetCurrentSoldierType());

        yield return null;

        _animator.SetBool("End", true);
        yield return new WaitForSeconds(_endAnimationDuration);

        soldierState.GetComponent<SoldierMovement>().SetMovementEnabled(true);
        _animator.SetBool("End", false);
        _isTransitioning = false;
    }
    private void UnlockSoldier(ISoldierState soldierState) {
        switch (soldierState) {
            case Captain:
                IsCaptainUnlocked = true;
                IsCaptainActive = true;
                break;
            case Sublieutenant:
                IsSublieutenantUnlocked = true;
                IsSublieutenantActive = true;
                break;
            case Sargeant:
                IsSargeantUnlocked = true;
                IsSargeantActive = true;
                break;
            case Cadet:
                IsCadetUnlocked = true;
                IsCadetActive = true;
                break;
        }
    }

    public void PlayStartTransition() {
        _animator.SetBool("Start", true);
        StartCoroutine(DisableAfterTime(_animator.GetCurrentAnimatorStateInfo(0).length, "Start"));
    }

    public void PlayEndTransition() {
        _animator.SetBool("End", true);
        StartCoroutine(DisableAfterTime(_animator.GetCurrentAnimatorStateInfo(0).length, "End"));
    }

    private IEnumerator DisableAfterTime(float time, string param) {
        yield return new WaitForSeconds(time);
        _animator.SetBool(param, false);
    }
    private void MakeAllSoldiersImmobile() {
        captain.GetComponent<SoldierMovement>().SetMovementEnabled(false);
        sublieutenant.GetComponent<SoldierMovement>().SetMovementEnabled(false);
        sargeant.GetComponent<SoldierMovement>().SetMovementEnabled(false);
        cadet.GetComponent<SoldierMovement>().SetMovementEnabled(false);
    }
    public SoldierType GetCurrentSoldierType() {
        if (_currentSoldier == captain) return SoldierType.Captain;
        if (_currentSoldier == sublieutenant) return SoldierType.Sublieutenant;
        if (_currentSoldier == sargeant) return SoldierType.Sargeant;
        if (_currentSoldier == cadet) return SoldierType.Cadet;
        return SoldierType.Captain;
    }
    public bool IsSoldierSelectable(SoldierType type) {
        return IsSoldierUnlocked(type) && IsSoldierActive(type);
    }

    public bool IsSoldierUnlocked(SoldierType type) {
        return type switch {
            SoldierType.Captain => IsCaptainUnlocked,
            SoldierType.Sublieutenant => IsSublieutenantUnlocked,
            SoldierType.Sargeant => IsSargeantUnlocked,
            SoldierType.Cadet => IsCadetUnlocked,
            _ => false
        };
    }
    public void SetSoldierActive(SoldierType type, bool active) {
        switch (type) {
            case SoldierType.Captain: IsCaptainActive = active; break;
            case SoldierType.Sublieutenant: IsSublieutenantActive = active; break;
            case SoldierType.Sargeant: IsSargeantActive = active; break;
            case SoldierType.Cadet: IsCadetActive = active; break;
        }
    }
    public bool IsSoldierActive(SoldierType type) {
        return type switch {
            SoldierType.Captain => IsCaptainActive,
            SoldierType.Sublieutenant => IsSublieutenantActive,
            SoldierType.Sargeant => IsSargeantActive,
            SoldierType.Cadet => IsCadetActive,
            _ => false
        };
    }
    public void PlayStartTransitionWithoutDisable() {
        _animator.SetBool("Start", true);
    }
}
