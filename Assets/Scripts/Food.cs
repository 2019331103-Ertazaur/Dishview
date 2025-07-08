using System;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class Food : MonoBehaviour
{
    [SerializeField]
    private string targetID;
    public string TargetID => targetID;

    [SerializeField]
    private string title;
    public string Title => title;

    [SerializeField]
    private string[] ingredients;
    public string[] Ingredients => ingredients;

    [SerializeField]
    private int price;
    public int Price => price;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI ingredientsText;
    [SerializeField]
    private TextMeshProUGUI priceText;


    private void OnEnable()
    {
        canvas.worldCamera = Camera.main;
        titleText.text = title;
        ingredientsText.text = $"Ingredients: {string.Join(", ", ingredients)}";
        priceText.text = $"{price}BDT";
    }


    private void OnDestroy()
    {
        
    }
}