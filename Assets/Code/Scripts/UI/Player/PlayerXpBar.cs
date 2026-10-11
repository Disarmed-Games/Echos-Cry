using TMPro;
using UnityEngine;

public class PlayerXpBar : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI xpText;

    // Variables Provided from here: https://youtu.be/CFASjEuhyf4?si=ri_WpIV1OxgtQdgp at 5:00
    [SerializeField] private UnityEngine.UI.Image frontXpBar;
    [SerializeField] private UnityEngine.UI.Image backXpBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private FloatFloatIntEventChannel eventChannel;
    [SerializeField] private GameObject upgradeReminder;

    public float chipSpeed = 2f;
    private float lerpTimer;
    private float hFraction = 0f;

    private void OnEnable()
    {
        if (eventChannel != null) eventChannel.Channel += UpdateXP;
        RefreshFromPlayer();
    }
    private void OnDisable()
    {
        if (eventChannel != null) eventChannel.Channel -= UpdateXP;
    }

    private void Start() => RefreshFromPlayer();

    private void RefreshFromPlayer()
    {
        var player = Player.Instance;
        if (player == null || player.XP == null) return;
        UpdateXP(player.XP.CurrentXPAmount, player.XP.GoalXPAmount, player.XP.CurrentLevel);
    }

    private void UpdateXP(float xp, float goalXP, int currentLevel)
    {
        xpText.text = $"{xp} / {goalXP}";
        levelText.text = currentLevel.ToString();
        hFraction = goalXP > 0f ? Mathf.Clamp01(xp / goalXP) : 0f;
        lerpTimer = 0f;
    }

    private void Update()
    {
        //Code Section Provided Here: https://youtu.be/CFASjEuhyf4?si=ri_WpIV1OxgtQdgp at 13:19
        float fillF = frontXpBar.fillAmount;
        float fillB = backXpBar.fillAmount;

        if (fillB > hFraction)
        {
            frontXpBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backXpBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }

        if (fillF < hFraction)
        {
            backXpBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontXpBar.fillAmount = Mathf.Lerp(fillF, backXpBar.fillAmount, percentComplete);
        }

        upgradeReminder.SetActive(UpgradeManager.Instance != null && UpgradeManager.Instance.GetAvailablePoints() > 0);
    }
}
