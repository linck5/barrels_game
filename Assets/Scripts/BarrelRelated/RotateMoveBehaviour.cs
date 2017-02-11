using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RotateMoveBehaviour : MoveBehaviour
{

    public float rotationDuration = 0.5f;
    public float delayBetweenMoves = 0.5f;
    public bool backAndForth = true;

    public List<float> angles = new List<float>();

    int iterationIncrement = 1;
    int i = 0;

    void Awake () {

    }

    new void Start () {
        base.Start();
        
    }



    IEnumerator Rotate () {

        Easer ease = delayBetweenMoves > 0.0f ? Ease.SineInOut : Ease.Linear;

        while (active) {

            StartCoroutine(transform.RotateTo(Quaternion.Euler(0, 0, angles[i]), rotationDuration, ease));

            if (angles.Count > 1) {

                i += iterationIncrement;

                if (backAndForth) {
                    if (i >= angles.Count - 1) {
                        iterationIncrement = -1;
                    }
                    else if (i <= 0) {
                        iterationIncrement = 1;
                    }
                }
                else {
                    if (i >= angles.Count) i = 0;
                }
            }
            else {
                i = 0;
            }
            
            yield return new WaitForSeconds(rotationDuration + delayBetweenMoves);
        }
       
    }

    void Update () {

    }


    public override void Activate () {
        active = true;
        StartCoroutine(Rotate());
    }

    public override void Deactivate () {
        active = false;
    }
}
