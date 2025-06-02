using TMPro;
using UnityEngine;

public class NPCUI : Singleton<NPCUI>
{
    [SerializeField] private TextMeshProUGUI talkWithName;

    private void Start()
    {
        Hide();
    }

    public void Show(string name)
    {
        talkWithName.text = "Talk with " + name; 
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
