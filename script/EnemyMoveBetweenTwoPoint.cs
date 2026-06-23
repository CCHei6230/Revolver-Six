using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
public class EnemyMoveBetweenTwoPoint : EnemyNonBossBase
{
    protected enum Facing
    {
        L = -1,
        R = 1
    }
    protected   Facing facing = Facing.L;
    [SerializeField] protected bool turnAround = true;

    [SerializeField] protected Transform pointA;
    [SerializeField] protected Transform pointB;
    [SerializeField] protected float duration;
    [SerializeField] protected Ease ease;
    protected Tweener tweenOnGoning;
    protected Vector3 fixPositionA;
    protected Vector3 fixPositionB;
    [SerializeField]protected bool Debug_Pause = false;

    protected void Awake()
    {
        fixPositionA = pointA.position;
        fixPositionB = pointB.position;
        transform.position = fixPositionA;
        MoveToPointB();
    }

    protected void  MoveToPointA()
    {
        tweenOnGoning = transform.DOMove(fixPositionA, duration).OnComplete(()=> MoveToPointB()).SetEase(ease);
        if (!turnAround)
        {
            return;
        }

        if (facing == Facing.R)
        {
            facing = Facing.L;
        }
        else
        {
            facing = Facing.R;
        }

        FaceCheck();
    }

    protected void  MoveToPointB()
    {
        tweenOnGoning = transform.DOMove(fixPositionB, duration).OnComplete(()=> MoveToPointA()).SetEase(ease);
        if (!turnAround)
        {
            return;
        }
        if (facing == Facing.R)
        {
            facing = Facing.L;
        }
        else
        {
            facing = Facing.R;
        }

        FaceCheck();
    }

    protected void  FaceCheck()
    {
        if (facing == Facing.R)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    protected void Update()
    {

    }
}
