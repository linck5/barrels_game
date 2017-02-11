using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [HideInInspector]
    public bool midAir = false;

    [HideInInspector]
    public Rigidbody2D rigidBody2D;

    public void StartMoreGravityOverTimeCoroutine () {
        StartCoroutine("MoreGravityOverTime");
    }

    public void StopMoreGravityOverTimeCoroutine () {
        StopCoroutine("MoreGravityOverTime");
    }

    public void ResetRB2DForcesAndGravity () {
        StopMoreGravityOverTimeCoroutine();
        rigidBody2D.gravityScale = 0;
        rigidBody2D.velocity = Vector2.zero;
        rigidBody2D.angularVelocity = 0;
    }

    public bool ShouldDie () {

        return !GameManager.inst.playerLiveZone.OverlapPoint(transform.position);
    }

    public IEnumerator MoreGravityOverTime () {
        
        float noGravityTime = 0.1f;
        float regainGravityDuration = 1f;
        float maxGravity = 1f;

        float regainGravityElapsed = 0f;
        Rigidbody2D playerRB = GameManager.inst.playerRB2D;

        playerRB.gravityScale = 0;
        yield return new WaitForSeconds(noGravityTime);

        while (playerRB.gravityScale < maxGravity) {

            if (regainGravityElapsed < regainGravityDuration) {
                regainGravityElapsed += Time.deltaTime;
            }
            else {
                regainGravityElapsed = regainGravityDuration;
            }

            float gravityCurrent = regainGravityElapsed / regainGravityDuration * maxGravity;

            playerRB.gravityScale = Utils.EaseValue(0, maxGravity, gravityCurrent, Ease.SineIn);
            yield return null;
        }
        playerRB.gravityScale = maxGravity;

    }

    void Awake () {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

	void Start () {
	
	}
	
	void Update () {
	
	}

    void OnTriggerEnter2D (Collider2D other) {

        if(other.tag == "Barrel"){

            GameObject otherObj = other.gameObject;

            if (
                    otherObj != GameManager.inst.barrelHolingPlayer &&
                    otherObj != GameManager.inst.prevBarrelHoldingPlayer
            ) {
                GameManager.inst.HandleBarrelHit(otherObj);
                ResetRB2DForcesAndGravity();
            }
        }

    }
}
