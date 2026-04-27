using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] private float range = 2f;
    [SerializeField] GameObject indicatorObject;
    void Update()
    {
        indicatorObject.SetActive((Vector3.Distance(this.transform.position, Player.Instance.transform.position) <= range));
    }
}
