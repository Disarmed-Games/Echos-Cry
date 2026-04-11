using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    [Header("System Dependencies")]
    [SerializeField] InputTranslator translator;
    [Header("Relevant Interactable Variables")]
    [Tooltip("Half extents of interaction box")]
    [SerializeField] private Vector3 halfExtents = Vector3.one;
    [Tooltip("Interactable layer")]
    [SerializeField] private LayerMask layer;

    private void Interact()
    {
        Collider collider = CheckCollision();
        if (collider == null) return;
        collider.GetComponent<IInteractable>()?.Execute();
    }
    private Collider CheckCollision()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, halfExtents, Quaternion.identity, layer);
        if (colliders.Length > 0) return colliders[0];
        else return null;
    }

    private void OnEnable()
    {
        translator.OnInteractEvent += Interact;
    }
    private void OnDisable()
    {
        translator.OnInteractEvent -= Interact;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, halfExtents);
    }
}