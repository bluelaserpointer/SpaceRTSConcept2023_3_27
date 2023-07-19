using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    public float targetAlpha;
    public float transSpeed;
    public CanvasGroup CanvasGroup { get; private set; }
    private void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
    }
    void Update()
    {
        CanvasGroup.alpha = Mathf.MoveTowards(CanvasGroup.alpha, targetAlpha, transSpeed * Time.deltaTime);
    }
}
