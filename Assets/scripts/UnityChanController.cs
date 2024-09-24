using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnityChanController : MonoBehaviour
{
    // public float duration = 2.0f;
    
    private Sequence mySequence ;
    private float distance = 0.0f;
    private Animation anim;
    
    public Transform point;  // 中心点
    public float duration = 5f;  // 旋转时长
    public float speed = 360f; 

    void Start()
    {

        // mySequence = DOTween.Sequence();
        //
        // mySequence.Append(transform.DOMove(new Vector3(0,0,0), duration)
        //         .SetEase(Ease.InOutQuad));
        // mySequence.Append(transform.DOMove(new Vector3(0,0,0), 2.5f)
        //     .SetEase(Ease.InOutQuad));
        // mySequence.Append(transform.DORotate(new Vector3(0,0,0),0.5f)
        //         .SetEase(Ease.InOutQuad));

        // // 旋转角色
        // transform.DORotate(new Vector3(0, angle, 0), duration, RotateMode.LocalAxisAdd)
        //     .SetEase(Ease.Linear);  // 线性运动

        StartCoroutine(RotateAroundCoroutine());

    }
    IEnumerator RotateAroundCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float angle = speed * Time.deltaTime;
            transform.RotateAround(point.position, Vector3.up, angle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.DORotate(new Vector3(0, -90, 0), 2.0f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear);
        
        elapsedTime = 0f;
        while (elapsedTime < 6.5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.DOMove(new Vector3(0.0f, 0, 0.15f), 1.0f).SetEase(Ease.Linear);

    }

}
