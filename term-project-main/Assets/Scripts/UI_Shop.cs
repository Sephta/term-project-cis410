using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    
    private GameObject player;
    private PlayerController pc;

    private void Awake()
    {
        container = transform.Find("Container");
        shopItemTemplate = container.Find("ShopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
    }

    private void Start()
    {
        // generate shop item buttons
        CreateItemButton("Sword Upgrade", 50, 0);

        HideShop();
    }

    private void CreateItemButton(string itemName, int itemCost, int positionIndex)
    {
        // ^^ WILL NEED TO EDIT DECLARATION TO TAKE SPRITES IF WE DECIDE TO HAVE EQUIPMENT SPRITES LATER ^^

        // create item template inside of container; get RectTransform data
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();
        
        // define button color behavior
        Button button = shopItemTransform.GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.highlightedColor = Color.grey;
        cb.pressedColor = Color.cyan;
        button.colors = cb;

        // populate shop item buttons
        float shopItemSpace = -100f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, (shopItemSpace * positionIndex));

        // assign parameters to shop items
        shopItemTransform.Find("ItemName").GetComponent<Text>().text = itemName;
        shopItemTransform.Find("ItemCost").GetComponent<Text>().text = itemCost.ToString();
        // ~ shopItemTransform.Find("ItemIcon").GetComponent<Image>().sprite = itemSprite;


        shopItemTransform.gameObject.SetActive(true);
    }

    public void DisplayShop()
    {
        gameObject.SetActive(true);
    }

    public void HideShop()
    {
        gameObject.SetActive(false);
    }

    public void BuyItem(int cost)
    {
        if (player != null && pc != null)
        {
            if (pc.SpendCurrency(cost))
            {
                pc.EquipItem();
            }
        }
    }
}

