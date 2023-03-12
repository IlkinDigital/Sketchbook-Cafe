using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CanvasBrush : MonoBehaviour
{
    [SerializeField] private Transform tip = null;
    [SerializeField] private int penSize = 5;

    private Renderer renderer;
    private Color[] colors;
    private float tipHeight;
    private RaycastHit touch;
    private Canvas canvas;
    private Vector2 touchPos, lastTouchPos;
    private bool touchedLastFrame = false;
    private Quaternion lastTouchRot;

    void Start()
    {
        if (tip == null)
        {
            tip = GameObject.FindGameObjectWithTag("Tip").transform;
        }
        renderer = tip.GetComponent<Renderer>();
        colors = Enumerable.Repeat(renderer.material.color, penSize * penSize).ToArray();
        tipHeight = tip.localScale.y * transform.localScale.y;
    }

    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        if (Physics.Raycast(tip.position, transform.forward, out touch, tipHeight))
        {
            if (touch.transform.CompareTag("Canvas"))
            {
                if (canvas == null)
                {
                    canvas = touch.transform.GetComponent<Canvas>();
                }

                touchPos = new Vector2(touch.textureCoord.x, touch.textureCoord.y);
                int x = (int)(touchPos.x * canvas.textureSize.x - (penSize / 2));
                int y = (int)(touchPos.y * canvas.textureSize.y - (penSize / 2));

                if (x < 0 || x > canvas.textureSize.x || y < 0 || y > canvas.textureSize.y) { return; }

                if (touchedLastFrame)
                {
                    canvas.texture.SetPixels(x, y, penSize, penSize, colors);

                    for (float f = 0.01f; f < 1.0f; f += 0.03f)
                    {
                        var lerpX = (int)Mathf.Lerp(lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(lastTouchPos.y, y, f);
                        canvas.texture.SetPixels(lerpX, lerpY, penSize, penSize, colors);
                    }

                    transform.rotation = lastTouchRot;
                    canvas.texture.Apply();
                }

                lastTouchPos = new Vector2(x, y);
                lastTouchRot = transform.rotation;
                touchedLastFrame = true;
                return;
            }
        }

        canvas = null;
        touchedLastFrame = false;
    }
}
