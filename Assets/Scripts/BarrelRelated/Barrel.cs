using UnityEngine;
using System.Collections;

public class Barrel : MonoBehaviour {

    [HideInInspector] public Vector3 unprocessedPosition;
    [HideInInspector] public Renderer barrelRenderer;

    [HideInInspector] public System.Func<Vector3> NextBarrelPosition;

    [HideInInspector] public int difficulty;

    [HideInInspector] public bool autoFiring = false;

    void Awake () {
        barrelRenderer = GetComponent<Renderer>();
    }

	void Start () {

	}

    public void HoldPlayer () {
        GameManager.inst.barrelHolingPlayer = this.gameObject;
        MovePlayerTransformTogether();
        GameManager.inst.playerLiveZone.transform.position = GameManager.inst.barrelHolingPlayer.transform.position;
    }

    public void FirePlayer (float delay = 0f) {
        if (delay > 0f) {
            StartCoroutine(FirePlayerAfterDelay(delay));
        }
        else {
            GameManager.inst.HandlePlayerFired();
            autoFiring = false;
        }
    }

    IEnumerator FirePlayerAfterDelay (float delay) {
        yield return new WaitForSeconds(delay);
        FirePlayer();
    }

    public void AutoFire (float rotationDuration) {

        Vector3 dir = GameManager.inst.nextBarrel.transform.position - GameManager.inst.barrelHolingPlayer.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GetComponent<MoveBehaviour>().RotateToAngle(angle, rotationDuration);

        FirePlayer(rotationDuration);

        autoFiring = true;
    }

    void MovePlayerTransformTogether () {
        GameManager.inst.player.transform.position = this.transform.position;
        GameManager.inst.player.transform.rotation = this.transform.rotation;
       
    }

	void Update () {
        if (GameManager.inst.barrelHolingPlayer == this.gameObject) {
            MovePlayerTransformTogether();
        }
	}

    public bool IsOffScreen () {
        return !barrelRenderer.isVisible;
    }


    void safeTranslate (float x, float y, float z) {
        Quaternion tempRotation = this.transform.rotation;
        this.transform.rotation = Quaternion.identity;
        this.transform.position = unprocessedPosition;
        this.transform.Translate(x, y, z);
        unprocessedPosition = this.transform.position;
        this.transform.rotation = tempRotation;
    }


}
