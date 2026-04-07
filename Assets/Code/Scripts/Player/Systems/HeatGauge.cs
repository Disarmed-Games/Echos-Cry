using UnityEngine;

public class HeatGauge : MonoBehaviour
{
    [SerializeField] private int m_maxCharge = 9;
    private int m_currentCharge = 0;

    public int MaxCharge { get => m_maxCharge; set => m_maxCharge = value; }
    public int CurrentCharge { get => m_currentCharge; set => m_currentCharge = value; }
}
