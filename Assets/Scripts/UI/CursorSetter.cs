using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSetter : MonoBehaviour
{
    public Texture2D cursor;
    public Vector3 positionOffset = Vector3.zero;

    void Awake()
    {
        Cursor.SetCursor(cursor, positionOffset, CursorMode.Auto);
    }
}
