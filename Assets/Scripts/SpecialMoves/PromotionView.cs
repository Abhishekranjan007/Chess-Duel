using UnityEngine;

public class PromotionView : MonoBehaviour
{
    public GameObject promotionW, promotionB;
    public Animator promotionWanim, promotionBanim;
    string devicetype, promotion = "";    

    //Shows Promotion strip for White Player
    public void ShowPromotionStripW()
    {        
        promotionW.SetActive(true);
        promotionWanim.SetTrigger("ZoomIN");        
        promotion = "W";
    }

    //Shows Promotion strip for Black Player
    public void ShowPromotionStripB()
    {
        promotionB.SetActive(true);
        promotionBanim.SetTrigger("ZoomIN");       
        promotion = "B";
    }

    //Hides Promotion strip for Either Player
    public void HidePromotionStrip()
    {
        if (promotion == "W")
        {
            promotionWanim.SetTrigger("ZoomOut");
            promotionW.SetActive(false);
        }
        else if (promotion == "B")
        {
            promotionBanim.SetTrigger("ZoomOut");
            promotionB.SetActive(false);
        }
    }
}
