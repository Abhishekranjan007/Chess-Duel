using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TouchSelect : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void UpdateSelectionUI()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        //Debug.Log("MOUSE CLICK DETECTED");

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector2 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        Collider2D hit =
    Physics2D.OverlapPoint(worldPosition);

        //Debug.Log("Mouse World Pos = " + worldPosition);

        if (hit != null)
        {
            //Debug.Log("Hit Object = " + hit.name);
        }
        else
        {
            //Debug.Log("Hit Object = NULL");
        }

        if (hit != null)
        {
            ChessTile tile =
                hit.GetComponent<ChessTile>();

            if (tile != null)
            {
                Chessboard.Instance.SetSelectX(tile.x);
                Chessboard.Instance.SetSelectY(tile.y);

                //Debug.Log("CLICKED TILE : " +
                //          tile.x + "," + tile.y);

                return;
            }
        }

        //Debug.Log("CLICKED EMPTY SPACE");

        Chessboard.Instance.SetSelectX(-1);
        Chessboard.Instance.SetSelectY(-1);
    }
}
