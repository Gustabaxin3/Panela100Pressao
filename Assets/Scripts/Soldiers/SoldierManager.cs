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
    [field: SerializeField] public bool IsCaptainUnlocked {get; private set;} = true;

    [Header("Sublieutenant")]
    public Sublieutenant sublieutenant;
    [SerializeField] private LayerMask _sublieutenantLayerMask;
    [field: SerializeField] public bool IsSublieutenantUnlocked {get; private set;} = false;

    [Header("Sargeant")]
    public Sargeant sargeant;
    [SerializeField] private LayerMask _sargeantLayerMask;
    [field: SerializeField] public bool IsSargeantUnlocked {get; private set;} = false;

    [Header("Cadet")]
    public Cadet cadet;
    [SerializeField] private LayerMask _cadetLayerMask;
    [field: SerializeField] public bool IsCadetUnlocked { get; private set; } = false;

    [SerializeField] private Animator _animator;
    private float _startAnimationDuration;
    private float _endAnimationDuration;

    private bool _isTransitioning = false;

    [field: SerializeField] public Transform _originalParent { get; private set; }

    public static event Action<SoldierType> OnSoldierChanged;

    private void Start() {
        MakeAllSoldiersImmobile();
        ChangeState(_currentSoldier);
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

        AudioManager.Instance.PlaySoundEffect("Audio/Troca de Personagem/TrocaPersonagem", volume: 0.5f, position: transform.position, spatialBlend: 0);

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
            case Captain: IsCaptainUnlocked = true; break;
            case Sublieutenant: IsSublieutenantUnlocked = true; break;
            case Sargeant: IsSargeantUnlocked = true; break;
            case Cadet: IsCadetUnlocked = true; break;
        }
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
}
