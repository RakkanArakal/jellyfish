using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AnimationController : MonoBehaviour
{
    public float delayTime = 1.0f;
    public float moveDistance = 0.1f;
    public float loopsTimes = 5;
    
    private Sequence mySequence ;
    private float distance = 0.0f;
    private Animation anim;

    void Start()
    {
        Vector3 rotatedUp =  transform.rotation *  Vector3.up;
        mySequence = DOTween.Sequence();
        
        for (int i = 1; i <= loopsTimes; ++i)
        {
            mySequence.Append(transform.DOMove(transform.position + rotatedUp * (moveDistance + distance), 1.16f)
                .SetEase(Ease.InOutQuad));
            distance = 4 * i * moveDistance;
            mySequence.Append(transform.DOMove(transform.position + rotatedUp * distance, 1.5f)
                .SetEase(Ease.InOutQuad));
        }
        mySequence.Pause();
        
        anim = GetComponent<Animation>();
        
        foreach (AnimationState state in anim)
        {
            state.enabled = false;
        }


        StartCoroutine(ActivateAfterDelay());

    }

    IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        
        mySequence.Play();

        foreach (AnimationState state in anim)
        {
            state.enabled = true;
        }
    }
}
