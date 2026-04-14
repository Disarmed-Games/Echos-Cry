using UnityEngine;

public class ManageBeatShaderMaterials : MonoBehaviour
{
    [SerializeField] private Material _material;

    void Update()
    {
        float t = BeatManager.Instance.BeatProgress;
        float pulse = Mathf.Sin(t * Mathf.PI);
        _material.SetFloat("_BeatTime", pulse);
    }

    private void OnDisable()
    {
        _material.SetFloat("_BeatTime", 0f);
    }
}
