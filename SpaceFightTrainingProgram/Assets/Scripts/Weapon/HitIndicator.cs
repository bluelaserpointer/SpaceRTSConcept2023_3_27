using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HitIndicator : MonoBehaviour
{
    [SerializeField]
    GameObject hitIndicatorPiecePrefab;
    [SerializeField]
    float angleGap;
    [SerializeField]
    float fadeTime;
    float angle;
    public void Hit(BulletHitFeedback feedback)
    {
        GameObject piece = Instantiate(hitIndicatorPiecePrefab, transform);
        piece.transform.eulerAngles = Vector3.forward * angle;
        angle += angleGap;
        piece.GetComponent<Timer>().time = fadeTime;
        piece.GetComponent<UIFader>().transSpeed = 1 / fadeTime;
        if(feedback.effect.kill)
        {
            foreach(Image image in piece.GetComponentsInChildren<Image>())
            {
                image.color = Color.red;
            }
        }
    }
    private void OnEnable()
    {
        transform.DestroyAllChildren();
    }
}
