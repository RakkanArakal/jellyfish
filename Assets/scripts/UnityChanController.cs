using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnityChanController : MonoBehaviour
{
    public float duration = 2.0f;
    
    private Sequence mySequence ;
    private float distance = 0.0f;
    private Animation anim;

    void Start()
    {
        mySequence = DOTween.Sequence();
        
        mySequence.Append(transform.DOMove(new Vector3(0,0,0), duration)
                .SetEase(Ease.InOutQuad));
        mySequence.Append(transform.DOMove(new Vector3(0,0,0), 2.5f)
            .SetEase(Ease.InOutQuad));
        mySequence.Append(transform.DORotate(new Vector3(0,0,0),0.5f)
                .SetEase(Ease.InOutQuad));
        
    }

}
