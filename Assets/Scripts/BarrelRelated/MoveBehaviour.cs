using System;
using System.Collections;
using UnityEngine;

public abstract class MoveBehaviour : MonoBehaviour
{
    protected Barrel barrelScript;
    protected bool active = false;

    public bool ignoreActivation = false;
    public bool activateAutomatically = false;
    public float initialAngle = 0f;

    protected void Start () {
        barrelScript = GetComponent<Barrel>();
        if (!barrelScript) throw new System.Exception("This move behaviour doesn't work without a Barrel script");

        if (activateAutomatically) {
            Activate();
        }
    }

    public bool IsActive () {
        return active;
    }

    public abstract void Activate ();
    public abstract void Deactivate ();

    public void Activate (float delay) {
        StartCoroutine(ActivateAfterDelay(delay));
    }

    IEnumerator ActivateAfterDelay (float delay) {
        yield return new WaitForSeconds(delay);
        if (!ignoreActivation) {
            Activate();
        }
    }

    public void RotateToAngle (float angle, float rotationDuration) {
        StartCoroutine(transform.RotateTo(Quaternion.Euler(0, 0, angle), rotationDuration, Ease.SineInOut));
    }

    public void RotateToInitialAngle (float rotationDuration) {
        StartCoroutine(transform.RotateTo(Quaternion.Euler(0, 0, initialAngle), rotationDuration, Ease.SineInOut));
    }

}
