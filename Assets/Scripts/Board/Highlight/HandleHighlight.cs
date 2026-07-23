using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleHighlight : MonoBehaviour
{
    public Material highlightMaterial, futureMoveMaterial, selectedMaterial;
    public Sprite futureMoveSprite, pastmovesSprite, selectedpointSprite;
    private string selectedColor = "#6281C6", pastmoveColor = "#A0DDDD", devicetype, whiteColor = "#FFFFFF";
    Color selCol, historicalmoveCol, whiteCol;
    private float scale, scale2, scale3;


    // Start is called before the first frame update
    // initializes all the cell background color
    void Start()
    {
        if (!ColorUtility.TryParseHtmlString(selectedColor, out selCol))
        {
            selCol = Color.blue;
        }

        if (!ColorUtility.TryParseHtmlString(pastmoveColor, out historicalmoveCol))
        {
            historicalmoveCol = Color.blue;
        }

        if (!ColorUtility.TryParseHtmlString(whiteColor, out whiteCol))
        {
            whiteCol = Color.white;
        }
    }

    //Highlights the available spots on the board
    public void HighlightTiles()
    {
        
        //scale = devicetype == Constants.Mobile ? 0.35f : (devicetype == Constants.Tab ? 0.3f : 0.3f);
        scale = 1.5f;
        scale2 = 2.5f;
        List<Vector2Int> availableMoves = Chessboard.Instance.GetAvailablemoves();
        //Debug.Log("In highlight first " + availableMoves.Count);


        for (int i = 0; i < availableMoves.Count - 1; i++)
        {

            GameObject plane = GameObject.Find("Tiledt " + availableMoves[i].x + " " + availableMoves[i].y);
            plane.GetComponent<SpriteRenderer>().sortingOrder = 6;          
            
            plane.GetComponent<SpriteRenderer>().color = whiteCol;
            plane.GetComponent<SpriteRenderer>().sprite = futureMoveSprite;
            plane.transform.localScale = Vector3.one * 1.5f;//new Vector3(scale, scale, scale);//0.5 for hint on mobile, 0.3f for desk, 0.2 for tab
                                                     //0.7 for selcol on desktop, 1f for selcol on mobile, 0.6 for tab




            //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //plane.name = "available" + i;
            ////plane.transform.position = GetTileCenter2(availableMoves[i].x, availableMoves[i].y);
            //plane.transform.localScale = new Vector3(scale, scale, 1f);
            //plane.AddComponent<Image>();
            //plane.GetComponent<Image>().sprite = futureMoveSprite;
            //plane.transform.SetParent(Chessboard.Instance.GetChessboard().transform);
            //plane.transform.localPosition = Vector2.zero;
            //plane.transform.localPosition = Chessboard.Instance.GetTileCenter3(availableMoves[i].x, availableMoves[i].y);
            //if (Chessboard.Instance.GetChessboard().transform.childCount == 2)
            //{
            //    plane.transform.SetAsFirstSibling();
            //}
            //else
            //{
            //    plane.transform.SetSiblingIndex(1);
            //}


            ////plane.transform.localScale = new Vector3(0.023f, 0.023f, 0.023f);
            ////plane.GetComponent<Renderer>().material = futureMoveMaterial;
            //////print("In highlight " + availableMoves[i].x + " :: " + availableMoves[i].y);

            Chessboard.Instance.GetHighlightedspots().Add(plane);

        }


        GameObject plane2 = GameObject.Find("Tiledt " + availableMoves[availableMoves.Count - 1].x + " " + availableMoves[availableMoves.Count - 1].y);
        plane2.GetComponent<SpriteRenderer>().sortingOrder = 6;
        plane2.GetComponent<SpriteRenderer>().color = selCol;
        plane2.GetComponent<SpriteRenderer>().sprite = selectedpointSprite;
        plane2.transform.localScale = new Vector3(scale2, scale2, scale2);
        //GameObject plane2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //plane2.name = "Selected";
        //plane2.transform.localScale = new Vector3(scale2, scale2, scale2);
        //plane2.AddComponent<Image>();
        //plane2.GetComponent<Image>().color = selCol;
        //plane2.transform.SetParent(Chessboard.Instance.GetChessboard().transform);
        //plane2.transform.localPosition = Vector2.zero;
        ////plane2.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        ////plane2.GetComponent<Renderer>().material = selectedMaterial;//selectedMaterial;
        //plane2.transform.localPosition = Chessboard.Instance.GetTileCenter3(availableMoves[availableMoves.Count - 1].x, availableMoves[availableMoves.Count - 1].y);
        //plane2.transform.SetAsFirstSibling();


        Chessboard.Instance.GetHighlightedspots().Add(plane2);
        ////print("In highlight " + availableMoves[availableMoves.Count - 1].x + " :: " + availableMoves[availableMoves.Count - 1].y + " :: " + availableMoves.Count);
    }

    //Removes Highlights from the available spots on the board
    public void RemoveHighlightTiles()
    {
        //Debug.Log("Inside removehighlights");
        Color c = whiteCol;
        c.a = 0f;
        for (int i = 0; i < Chessboard.Instance.GetHighlightedspots().Count; i++)
        {
            GameObject k = Chessboard.Instance.GetHighlightedspots()[i];
            k.GetComponent<SpriteRenderer>().color = c;
            k.GetComponent<SpriteRenderer>().sprite = null;
            //Destroy(Chessboard.Instance.GetHighlightedspots()[i]);
            //tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        Chessboard.Instance.GetHighlightedspots().Clear();
        Chessboard.Instance.GetAvailablemoves().Clear();
        Chessboard.Instance.Clearlastmove();
        ////print("After removehighlights");
    }

    //Highlight Previous tiles from where the chesspiece has moved
    public void HighlightPrevTiles()
    {

        scale3 =  2f;
        //Debug.Log("Inside HighlightPrevTiles ");
        for (int i = 0; i < Chessboard.Instance.GetPrevAvailablemoves().Count; i++)
        {
            GameObject plane = GameObject.Find("Tiledt " + Chessboard.Instance.GetPrevAvailablemoves()[i].x + " " + Chessboard.Instance.GetPrevAvailablemoves()[i].y);
            plane.GetComponent<SpriteRenderer>().sortingOrder = 6;
            plane.GetComponent<SpriteRenderer>().color = historicalmoveCol;
            plane.GetComponent<SpriteRenderer>().sprite = pastmovesSprite;
            plane.transform.localScale = new Vector3(scale3, scale3, scale3);
            Chessboard.Instance.GetPrevHighlightedspots().Add(plane);
            //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //plane.name = "past" + i;
            //plane.transform.localScale = new Vector3(scale3, scale3, scale3);
            //plane.AddComponent<Image>();
            //plane.GetComponent<Image>().color = historicalmoveCol;
            //plane.transform.SetParent(Chessboard.Instance.GetChessboard().transform);
            //plane.transform.localPosition = Vector2.zero;
            ////plane.transform.position = GetTileCenter2(Chessboard.Instance.GetPrevAvailablemoves()[i].x, Chessboard.Instance.GetPrevAvailablemoves()[i].y);           
            ////plane.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            ////plane.GetComponent<Renderer>().material = highlightMaterial;
            //plane.transform.localPosition = Chessboard.Instance.GetTileCenter3(Chessboard.Instance.GetPrevAvailablemoves()[i].x, Chessboard.Instance.GetPrevAvailablemoves()[i].y);
            //plane.transform.SetAsFirstSibling();
            //Chessboard.Instance.GetPrevHighlightedspots().Add(plane);
            ////print("Inside HighlightPrevTiles i " + i + " :: " + Chessboard.Instance.GetPrevAvailablemoves()[i].x + " :: " + Chessboard.Instance.GetPrevAvailablemoves()[i].y);

            //tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    //Removes Highlights from Previous tiles where the chesspiece has moved
    public void RemovePrevHighlightTiles()
    {
        //Debug.Log("Inside removePrevhighlights");
        Color c = whiteCol;
        c.a = 0f;
        for (int i = 0; i < Chessboard.Instance.GetPrevHighlightedspots().Count; i++)
        {
            GameObject k = Chessboard.Instance.GetPrevHighlightedspots()[i];
            k.GetComponent<SpriteRenderer>().color = c;
            k.GetComponent<SpriteRenderer>().sprite = null;
            //Destroy(Chessboard.Instance.GetPrevHighlightedspots()[i]);
            //tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        Chessboard.Instance.GetPrevHighlightedspots().Clear();
        Chessboard.Instance.GetPrevAvailablemoves().Clear();
        ////print("After removePrevhighlights");
    }

    //Utility function to get center of the cell
    //private Vector3 GetTileCenter2(int x, int y)//##
    //{
    //    return new Vector3(x * Chessboard.Instance.tileSize, 1f, y * Chessboard.Instance.tileSize) - Chessboard.Instance.GetBounds() + new Vector3(Chessboard.Instance.tileSize / 2, 0, Chessboard.Instance.tileSize / 2);
    //}
}
