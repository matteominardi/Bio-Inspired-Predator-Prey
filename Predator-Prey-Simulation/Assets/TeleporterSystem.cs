using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(ActionPreySystem))]
[UpdateAfter(typeof(ActionPredatorSystem))]
public partial class TeleporterSystem : SystemBase
{
    float _leftWallX;
    float _rightWallX;
    float _topWallY;
    float _bottomWallY;

    float _bottomLocalScaleY;
    float _topLocalScaleY;
    float _rightLocalScaleX;
    float _leftLocalScaleX;

    protected override void OnStartRunning()
    {
        Vector3 left = GameObject.Find("leftWall").GetComponent<Transform>().position;
        _leftWallX = left.x;
        //leftWallY = left.y;
        _leftLocalScaleX = GameObject.Find("leftWall").GetComponent<Transform>().localScale.x * 0.5f;

        Vector3 right = GameObject.Find("rightWall").GetComponent<Transform>().position;
        _rightWallX = right.x; 
        //rightWallY = right.y;
        _rightLocalScaleX = GameObject.Find("rightWall").GetComponent<Transform>().localScale.x * 0.5f;

        Vector3 top = GameObject.Find("topWall").GetComponent<Transform>().position;
        //topWallX = top.x;
        _topWallY = top.y;
        _topLocalScaleY = GameObject.Find("topWall").GetComponent<Transform>().localScale.y * 0.5f;

        Vector3 bottom = GameObject.Find("bottomWall").GetComponent<Transform>().position;
        //bottomWallX = bottom.x;
        _bottomWallY = bottom.y;
        _bottomLocalScaleY = GameObject.Find("bottomWall").GetComponent<Transform>().localScale.y * 0.5f;
    }
    protected override void OnUpdate()
    {
        float topWallY = _topWallY;
        float bottomWallY = _bottomWallY;
        float leftWallX = _leftWallX;
        float rightWallX = _rightWallX;
        float bottomLocalScaleY = _bottomLocalScaleY;
        float topLocalScaleY = _topLocalScaleY;
        float rightLocalScaleX = _rightLocalScaleX;
        float leftLocalScaleX = _leftLocalScaleX;

        Entities.ForEach((Entity e, ref Translation translation) => {
            //Vector2 closestPoint = other.ClosestPoint(transform.position);
            float3 pos = translation.Value;
            // bottom wall
            //print("closestPoint.y: " + closestPoint.y + " bottomWall.transform.position.y: " + bottomWall.transform.position.y + " bottomWall.transform.localScale.y/2: " + bottomWall.transform.localScale.y/2 + "");
            if (pos.y < bottomWallY + bottomLocalScaleY + 1)
            {
                //print("I hit the bottom wall " + bottomWallY + 1 + " teleport to: " + new Vector2(other.gameObject.x, topWall.y + 1));
                translation.Value = new float3(pos.x, topWallY - topLocalScaleY - 3, 0);
            }
            // left wall
            if (pos.x < leftWallX + leftLocalScaleX + 1)
            {
                translation.Value = new float3(rightWallX - rightLocalScaleX-3, pos.y, 0);
            }
            // top wall
            if (pos.y > topWallY - topLocalScaleY - 1)
            {
                translation.Value = new float3(pos.x, bottomWallY + bottomLocalScaleY + 3, 0);
            }
            // right wall
            if (pos.x > rightWallX - rightLocalScaleX - 1)
            {
                translation.Value = new float3(leftWallX + leftLocalScaleX  + 3, pos.y, 0);
            }
        }).WithBurst().ScheduleParallel();
    }
}
