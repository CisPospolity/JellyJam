using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardButton cardPrefab;

    private LevelUpManager levelUpManager;

    public void Initialize(LevelUpManager manager)
    {
        levelUpManager = manager;
    }

    public void ShowOptions(LevelUpOption[] options)
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var option in options)
        {
            CardButton card = Instantiate(cardPrefab, cardContainer);
            card.Setup(option, levelUpManager);
        }

    }
}
