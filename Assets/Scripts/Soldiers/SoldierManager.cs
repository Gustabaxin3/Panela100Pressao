using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SoldierManager : MonoBehaviour {
    [SerializeField] private ISoldierState _currentSoldier;

    [Header("Captain")]
    public Captain captain;
    [SerializeField] private LayerMask _captainLayerMask;
    [SerializeField] private bool _isCaptainUnlocked = true;

    [Header("Sublieutenant")]
    public Sublieutenant sublieutenant;
    [SerializeField] private LayerMask _sublieutenantLayerMask;
    [SerializeField] private bool _isSublieutenantUnlocked = false;

    [Header("Sargeant")]
    public Sargeant sargeant;
    [SerializeField] private LayerMask _sargeantLayerMask;
    [SerializeField] private bool _isSargeantUnlocked = false;

    [Header("Cadet")]
    public Cadet cadet;
    [SerializeField] private LayerMask _cadetLayerMask;
    [SerializeField] private bool _isCadetUnlocked = false;

    [SerializeField] private Animator _animator;

    [field: SerializeField] public Transform _originalParent { get; private set; }

    private void Start() {
        MakeAllSoldiersImmobile();
        ChangeState(_currentSoldier);
        SoldierUnlockEvents.OnSoldierUnlocked += UnlockSoldier;
    }

    private void OnDestroy() {
        SoldierUnlockEvents.OnSoldierUnlocked -= UnlockSoldier;
    }

    private void Update() {
        _currentSoldier?.OnUpdate();
        ChooseSoldier();
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

    private void ChooseSoldier() {
        switch (true) {
            case bool _ when Input.GetKeyDown(KeyCode.Alpha1): ChangeCharacter(_isCaptainUnlocked, captain); break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha2): ChangeCharacter(_isSublieutenantUnlocked, sublieutenant); break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha3): ChangeCharacter(_isSargeantUnlocked, sargeant); break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha4): ChangeCharacter(_isCadetUnlocked, cadet); break;
        }
    }
    private void ChangeCharacter(bool soldierUnlocked, ISoldierState soldierState) {
        if (soldierUnlocked && _currentSoldier != soldierState) {
            StartCoroutine(StartAnimation(soldierState));
        }
    }
    private IEnumerator StartAnimation(ISoldierState soldierState) {
        _currentSoldier.GetComponent<SoldierMovement>().SetMovementEnabled(false);

        _animator.SetBool("Start", true);
        yield return new WaitForSeconds(1.5f);

        _animator.SetBool("Start", false);

        ChangeState(soldierState);
        yield return null;

        _animator.SetBool("End", true);
        yield return new WaitForSeconds(1f);

        soldierState.GetComponent<SoldierMovement>().SetMovementEnabled(true);
        _animator.SetBool("End", false);
    }
    private void UnlockSoldier(ISoldierState soldierState) {
        switch (soldierState) {
            case Captain: _isCaptainUnlocked = true; break;
            case Sublieutenant: _isSublieutenantUnlocked = true; break;
            case Sargeant: _isSargeantUnlocked = true; break;
            case Cadet: _isCadetUnlocked = true; break;
        }

        Debug.Log($"{name} desbloqueado!");
    }
    private void MakeAllSoldiersImmobile() {
        captain.GetComponent<SoldierMovement>().SetMovementEnabled(false);
        sublieutenant.GetComponent<SoldierMovement>().SetMovementEnabled(false);
        sargeant.GetComponent<SoldierMovement>().SetMovementEnabled(false);
        cadet.GetComponent<SoldierMovement>().SetMovementEnabled(false);
    }
}
