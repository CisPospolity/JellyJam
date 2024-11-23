using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private LevelUpOption _levelUpOption;
    private LevelUpManager levelUpManager;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    public void Setup(LevelUpOption option, LevelUpManager manager)
    {
        this._levelUpOption = option;
        this.levelUpManager = manager;

        if (option.icon != null)
            iconImage.sprite = option.icon;
        else
            iconImage.gameObject.SetActive(false);

        titleText.text = option.title;
        descriptionText.text = option.description;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        levelUpManager.SelectOption(_levelUpOption);
    }
}