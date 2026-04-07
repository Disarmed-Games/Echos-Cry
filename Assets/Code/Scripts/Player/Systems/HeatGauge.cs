using UnityEngine;

public class HeatGauge : MonoBehaviour
{
    [SerializeField] private int m_maxCharge = 9;
    private int m_currentCharge = 0;

    public int MaxCharge { get => m_maxCharge; set => m_maxCharge = value; }
    public int CurrentCharge { get => m_currentCharge; }

    public void IncreaseCharge(int amount)
    {
        if (m_currentCharge + amount > m_maxCharge) m_currentCharge = m_maxCharge;
        else m_currentCharge += amount;
    }
    public bool UseCharge(int amount)
    {
        if(m_currentCharge - amount < 0) return false;
        m_currentCharge -= amount;
        return true;
    }

    private void Update()
    {
        Debug.Log(m_currentCharge + " / " + m_maxCharge);
    }
}