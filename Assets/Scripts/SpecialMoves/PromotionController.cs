using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PromotionController : MonoBehaviour
{
    public static PromotionController instance { get; private set; }
    public PromotionView promotionView;
    private bool promotionPosibbleFlag = false;
    private string selectedPromotion = "";
    private PromotionType selectedPromotionType = PromotionType.Queen;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    //Controller to show Promotion Strip of White
    public void PromotionStripOnW()
    {
        // set a flag to stop normal game operation
        promotionPosibbleFlag = true;
        promotionView.ShowPromotionStripW();
    }

    //Controller to show Promotion Strip of Black
    public void PromotionStripOnB()
    {
        // set a flag to stop normal game operation
        promotionPosibbleFlag = true;
        promotionView.ShowPromotionStripB();
    }

    //Controller to show Promotion Strip of Either Player
    public void PromotionStripOff()
    {
        // set a flag to stop normal game operation
        promotionPosibbleFlag = false;
        promotionView.HidePromotionStrip();
    }

    //Flag to check if promotion is available
    public bool GetPromotionPosibbleFlag()
    {
        return promotionPosibbleFlag;
    }

    //Return the promotion selected by player
    public string GetSelectedPromotionValue()
    {
        return selectedPromotion;
    }

    //Set the promotion selected by player
    public void SetSelectedPromotionValue(string val)
    {
        selectedPromotion = val;
    }

    //Select white queen for selection
    public void SelectQueenW()
    {
        selectedPromotion = "QueenW";
        selectedPromotionType = PromotionType.Queen;
        CallPromotion();
    }

    //Select white rook for selection
    public void SelectRookW()
    {
        selectedPromotion = "RookW";
        selectedPromotionType = PromotionType.Rook;
        CallPromotion();
    }

    //Select white knight for selection
    public void SelectKnightW()
    {
        selectedPromotion = "KnightW";
        selectedPromotionType = PromotionType.Knight;
        CallPromotion();
    }

    //Select white bishop for selection
    public void SelectBishopW()
    {
        selectedPromotion = "BishopW";
        selectedPromotionType = PromotionType.Bishop;
        CallPromotion();
    }

    //Select black queen for selection
    public void SelectQueenB()
    {
        selectedPromotion = "QueenB";
        selectedPromotionType = PromotionType.Queen;
        CallPromotion();
    }

    //Select black rook for selection
    public void SelectRookB()
    {
        selectedPromotion = "RookB";
        selectedPromotionType = PromotionType.Rook;
        CallPromotion();
    }

    //Select black knight for selection
    public void SelectKnightB()
    {
        selectedPromotion = "KnightB";
        selectedPromotionType = PromotionType.Knight;
        CallPromotion();
    }

    //Select black bishop for selection
    public void SelectBishopB()
    {
        selectedPromotion = "BishopB";
        selectedPromotionType = PromotionType.Bishop;
        CallPromotion();
    }

    //Implement Selected Promotion
    public void CallPromotion()
    {
        Chessboard.Instance.ImplementPromtionMove();
    }

    public PromotionType GetPromotionType()
    {
        return selectedPromotionType;
    }

    //public PromotionType GetPromotionType()
    //{
    //    switch (selectedPromotion)
    //    {
    //        case "RookW":
    //        case "RookB":
    //            return PromotionType.Rook;

    //        case "KnightW":
    //        case "KnightB":
    //            return PromotionType.Knight;

    //        case "BishopW":
    //        case "BishopB":
    //            return PromotionType.Bishop;

    //        default:
    //            return PromotionType.Queen;
    //    }
    //}

}
