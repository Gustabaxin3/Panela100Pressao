using AUDIO;
using UnityEngine;

public class Zipline : MonoBehaviour {
    [SerializeField] private Zipline _targetZipLine;
    [SerializeField] private float _zipSpeed = 5f;
    [SerializeField] private float _zipScale = 0.2f;
    [SerializeField] private float _arrivalThreshold = 0.4f;
    [SerializeField] private Transform _zipTransform;

    private LineRenderer _lineRenderer;
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _lineYOffset = 1.5f;

    private bool _isZipLineActive = false;
    private GameObject _localZip;

    private void Awake() {
        // Inicializa o LineRenderer
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = _lineMaterial;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.useWorldSpace = true;
    }

    private void Update() {
        UpdateLineRenderer();

        // Se o zipline não está ativo ou o objeto local não existe, sai da função
        if (!_isZipLineActive || _localZip == null) return;

        // Aplica força ao objeto zip para movê-lo em direção ao destino
        _localZip.GetComponent<Rigidbody>().AddForce(
            (_targetZipLine._zipTransform.position - _zipTransform.position).normalized *
            _zipSpeed * Time.deltaTime,
            ForceMode.Acceleration
        );

        // Verifica se o objeto zip chegou próximo o suficiente do destino
        if (Vector3.Distance(_localZip.transform.position, _targetZipLine._zipTransform.position) <= _arrivalThreshold) {
            ResetZipLine();
        }
    }

    private void UpdateLineRenderer() {
        // Atualiza a posição da linha visual do zipline
        if (_targetZipLine == null || _zipTransform == null) return;

        Vector3 startPos = _zipTransform.position + Vector3.up * _lineYOffset;
        Vector3 endPos = _targetZipLine._zipTransform.position + Vector3.up * _lineYOffset;

        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    public void StartZipLine(GameObject player) {
        // Impede iniciar o zipline se já estiver ativo
        if (_isZipLineActive) return;

        // Cria um objeto invisível que servirá de transporte para o jogador
        _localZip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _localZip.GetComponent<Renderer>().enabled = false;
        _localZip.transform.position = player.transform.position;

        // Aumenta o tamanho do objeto invisível
        float enlargedScale = _zipScale * 3f;
        _localZip.transform.localScale = new Vector3(enlargedScale, enlargedScale, enlargedScale);
        _localZip.AddComponent<Rigidbody>().useGravity = false;
        _localZip.GetComponent<Collider>().isTrigger = true;

        // Desabilita a gravidade e o movimento do jogador, e o coloca como filho do objeto zip
        player.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponent<SoldierMovement>().SetMovementEnabled(false);
        player.transform.parent = _localZip.transform;
        _isZipLineActive = true;
    }

    private void ResetZipLine() {
        // Recupera o jogador que está como filho do objeto zip
        GameObject player = _localZip.transform.GetChild(0).gameObject;

        // Restaura as propriedades físicas e de movimento do jogador
        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        player.GetComponent<SoldierMovement>().SetMovementEnabled(true);

        // Restaura o parent original do jogador
        player.transform.parent = player.GetComponent<Sargeant>().ResetParentToOriginal();

        // Destroi o objeto zip e reseta o estado
        Destroy(_localZip);
        _localZip = null;
        _isZipLineActive = false;
    }
}
