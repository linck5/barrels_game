using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathMoveBehaviour : MoveBehaviour {

    public List<Vector3> pathPoints = new List<Vector3>();
    public float moveDuration = 0.25f;
    public float delayBetweenMoves = 0.25f;
    public bool backAndForth = true;

    int iterationIncrement = 1;
    int i = 0;
    

    new void Start () {
        base.Start();
    }

    void Update () {

    }

    IEnumerator FollowPath () {

        while (active) {

            StartCoroutine(transform.MoveTo(pathPoints[i], moveDuration, Ease.SineInOut));

            if (pathPoints.Count > 1) {

                i += iterationIncrement;

                if (backAndForth) {
                    if (i >= pathPoints.Count - 1) {
                        iterationIncrement = -1;
                    }
                    else if (i <= 0) {
                        iterationIncrement = 1;
                    }
                }
                else {
                    if (i >= pathPoints.Count) i = 0;
                }
            }
            else {
                i = 0;
            }

            yield return new WaitForSeconds(moveDuration + delayBetweenMoves);
        }
        

    }

    

    public override void Activate () {
        active = true;
        StartCoroutine(FollowPath());
    }

    public override void Deactivate () {
        active = false;
    }
}
