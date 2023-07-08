using TMPro;
using UnityEngine;

public class BarsRemainingUI : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI(int barsRemaining)
    {
        if (text != null)
        {
            text.text = "Censor bars: " + barsRemaining;
        }
    }
}
