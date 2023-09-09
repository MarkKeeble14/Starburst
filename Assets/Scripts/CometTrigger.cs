using UnityEngine;

public class CometTrigger : MonoBehaviour
{
    private bool hasHit;
    [SerializeField] private Comet parentComet;
    [SerializeField] private LayerMask player;
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, player)) return;
        if (hasHit) return;
        hasHit = true;
        parentComet.ContactPlayer(other.GetComponent<MainBall>());
    }
}
