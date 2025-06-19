using UnityEngine;
using AUDIO;

public class PushableObject : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private Transform _pusher;
    private Vector3 _offset;

    public bool IsBeingPushed => _pusher != null;

    //variáveis para implementação de audio
    private readonly string pushSoundPath = "Audio/ArrastaObjeto";
    private AudioSource pushAudioSource;
    private bool wasMoving = false;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void StartPush(Transform pusher)
    {
        InteractionHintUI.Instance.HideHint();

        _pusher = pusher;
        _offset = transform.position - _pusher.position;

        _rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    public void StopPush()
    {
        _pusher = null;
        _rigidBody.linearVelocity = Vector3.zero;
        _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void FixedUpdate()
    {
        if (_pusher != null)
        {
            Vector3 targetPosition = _pusher.position + _offset;
            Vector3 moveDirection = (targetPosition - transform.position);
            _rigidBody.linearVelocity = moveDirection * 10f;
            InteractionHintUI.Instance.ShowHint("Pressione E para parar de empurrar.");

            bool isMoving = moveDirection.magnitude > 0.05f;
            if (isMoving && !wasMoving)
            {
                AudioManager.Instance.PlaySoundEffect("Audio/ArrastaObjeto", loop: true, position: transform.position, spatialBlend: 0);

            }
            else if (!isMoving && wasMoving)
            {
                AudioManager.Instance.StopSoundEffect("ArrastaObjeto");
            }
            wasMoving = isMoving;
        }
        else
        {
            if (wasMoving)
            {
                AudioManager.Instance.StopSoundEffect("ArrastaObjeto");
                wasMoving = false;
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sublieutenant"))
        {
            InteractionHintUI.Instance.ShowHint("Pressione E para empurrar.");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sublieutenant"))
        {
            InteractionHintUI.Instance.HideHint();
        }
    }
}

