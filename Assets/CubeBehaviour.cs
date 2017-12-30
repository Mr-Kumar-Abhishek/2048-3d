﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    Vector3 oldPos;
    Vector3 destPos;
    Transform destCube;
    int value = 2;
    public static float moveSpeed;
    bool spawning = true;

    Dictionary<int, string> colors = new Dictionary<int, string>()
    {
        {2, "#ecdfc7"},
        {4, "#efcbac"},
        {8, "#f2b179"},
        {16, "#f59563"},
        {32, "#f67c5f"},
        {64, "#f95c30"},
        {128, "#edce68"},
        {256, "#eecd57"},
        {512, "#eec943"},
        {1024, "#eec62c"},
        {2048, "#eec308"}
    };

	void Start ()
    {
        destPos = transform.position;
        oldPos = transform.position;
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    public void SetValue(int val)
    {
        float size = 0.05f;
        if (val >= 1024)
            size = 0.03f;
        else if (val >= 128)
            size = 0.04f;
        Component[] children = gameObject.GetComponentsInChildren(typeof(TextMesh));
        foreach (Component child in children)
        {
            TextMesh text = (TextMesh)child;
            text.text = val.ToString();
            text.characterSize = size;
        }
        Color col = new Color();
        ColorUtility.TryParseHtmlString(colors[val], out col);
        GetComponent<Renderer>().material.color = col;
        value = val;
    }

    public void SetTarget(Vector3 dest, Transform replace = null)
    {
        destPos = dest;
        destCube = replace;
        spawning = false;
    }
	
	void Update ()
    {
        if (destPos != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime);
            if (destPos == oldPos) return; // Happens when moved back and forth along one axis quickly
            if ((transform.position - oldPos).magnitude < (transform.position - destPos).magnitude)
            {
                float ratio = (destPos - transform.position).magnitude / (destPos - oldPos).magnitude;
                if (ratio > 0.7)
                    transform.localScale = ratio * new Vector3(1.3f, 1.3f, 1.3f);
            }
            else
            {
                float ratio = (transform.position - oldPos).magnitude / (destPos - oldPos).magnitude;
                if (ratio > 0.7)
                    transform.localScale = ratio * new Vector3(1.3f, 1.3f, 1.3f);
            }
        }
        else if (oldPos != transform.position)
        {
            oldPos = transform.position;
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            GameBehaviour.movingCount--;
            if (destCube != null)
            {
                if (value <= 1024)
                {
                    SetValue(2 * value);
                    Destroy(destCube.gameObject);
                    destCube = null;
                    if (value == 2048)
                    {
                        print("You Won!");
                        GameBehaviour.QuitGame();
                    }
                }
                else // Should only happen during testing
                {
                    print("Can't go above 2048!");
                    GameBehaviour.QuitGame();
                }
            }
        }
        else if (spawning)
        {
            if (transform.localScale.x < 1.3)
                transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            else
                spawning = false;
        }
	}
}
