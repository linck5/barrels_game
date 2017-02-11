using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;
    public static GameManager inst {
        get { if (!instance) instance = GameObject.FindObjectOfType<GameManager>(); return instance; }
    }

    #region properties

    //general
    public bool gameplayIsRunning = false;
    public bool showDebug = false;
    string barrelsBeatStr = "";
    int barrelsToJump = 0;

    //barrel
    public GameObject barrelPF;
    public List<GameObject> barrels = new List<GameObject>();
    [HideInInspector] public GameObject prevBarrelHoldingPlayer;
    [HideInInspector] public GameObject nextBarrel;
    [HideInInspector] public PolygonCollider2D barrelHolingPlayerPC2D;
    private GameObject _barrelHolingPlayer;
    [HideInInspector] public GameObject barrelHolingPlayer {
        get { return _barrelHolingPlayer; }
        set {
            _barrelHolingPlayer = value;
            barrelHolingPlayerPC2D = barrelHolingPlayer ? barrelHolingPlayer.GetComponent<PolygonCollider2D>() : null;
        }
    }
    [HideInInspector] public List<GameObject> lastBarrelsJumped = new List<GameObject>();
    const int INITIAL_BARRELS = 10;
    const int DESIRED_MAX_BARRELS = 20;
    public int barrelsBeat = 1;
    public int initialBarrelsBeat = 1;
    public int initialBarrelsSpawned = 0;

   

    //camera
    public GameObject mainCamera;
    [HideInInspector] public CameraFollow2D mainCameraScript;
    [HideInInspector] public Camera mainCameraCameraComp;

    //player
    public GameObject player;
    [HideInInspector] public Rigidbody2D playerRB2D;
    [HideInInspector] public Player playerScript;
    [HideInInspector] public PolygonCollider2D playerPC2D;
    public BoxCollider2D playerLiveZone;

    #endregion

    void Awake () {
        playerRB2D = player.GetComponent<Rigidbody2D>();
        playerPC2D = player.GetComponent<PolygonCollider2D>();
        playerScript = player.GetComponent<Player>();

        mainCameraScript = mainCamera.GetComponent<CameraFollow2D>();
        mainCameraCameraComp = mainCamera.GetComponent<Camera>();
    }


	// Use this for initialization
	void Start () {

        ClearGame();
        SaveAndLoad.Load();
	}


    public void EndGame () {
        gameplayIsRunning = false;

        SaveAndLoad.Save();

        ClearGame();

        player.SetActive(false);
    }

    public void RestartGame () {

        EndGame();

        barrelsBeat = initialBarrelsBeat;

        player.SetActive(true);
        player.GetComponent<Player>().ResetRB2DForcesAndGravity();

        DifficultyManager.UpdateDifficulty();
        for (int i = 0; i < INITIAL_BARRELS; i++) {
            BarrelGenerator.inst.GenerateBarrel(DifficultyManager.currentDifficulty);
            initialBarrelsSpawned++;
            DifficultyManager.UpdateDifficulty();
        }

        barrels[0].GetComponent<Barrel>().HoldPlayer();
        barrels[0].GetComponent<MoveBehaviour>().Activate();
        nextBarrel = barrels[1];

        mainCameraScript.target = barrelHolingPlayer.transform;
        mainCameraScript.fixedYPosition = barrelHolingPlayer.transform.position.y;

        gameplayIsRunning = true;
        StartCoroutine(RestartIfPlayerDies());
        StartCoroutine(DestroyBarrelsInExcess());
    }

    public void ClearBarrels () {
        foreach (GameObject barrel in barrels) {
            Destroy(barrel.gameObject);
        }
        barrels.Clear();
    }

    public void ClearGame () {
        ClearBarrels();
        barrelsBeat = 1;
        initialBarrelsSpawned = 0;
        player.SetActive(false);
    }

    public void FocusCameraOnBarrel (GameObject barrel) {
        mainCameraScript.fixedYPosition = barrel.transform.position.y;
        mainCameraScript.target = barrel.transform;
    }

    void OnGUI () {


        if (GUI.Button(new Rect(10, 10, 100, 30), showDebug? "Hide Debug" : "Show Debug")) {
            showDebug = !showDebug;
        }

        if (showDebug) {

            if (GUI.Button(new Rect(10, 50, 120, 30), "Clear Saved Data")) {
                SaveAndLoad.ClearData();
                SaveAndLoad.Load();
            }

           barrelsBeatStr = GUI.TextField(new Rect(10, 90, 70, 20), barrelsBeatStr);

           if (GUI.Button(new Rect(90, 90, 40, 30), ">>")) {
               barrelsBeat = int.Parse(barrelsBeatStr);
               initialBarrelsBeat = barrelsBeat;
           }

           GUI.Label(new Rect(Screen.width - 100, 80, 500, 30), "-- Info -- ");
           GUI.Label(new Rect(Screen.width - 100, 100, 500, 30), "Curr Barrel: " + (barrels.IndexOf(barrelHolingPlayer) + 1));
           GUI.Label(new Rect(Screen.width - 100, 120, 500, 30), "Prev Barrel: " + (barrels.IndexOf(prevBarrelHoldingPlayer) + 1));
           GUI.Label(new Rect(Screen.width - 100, 140, 500, 30), "Lst N of JB: " + lastBarrelsJumped.Count);
           GUI.Label(new Rect(Screen.width - 100, 160, 500, 30), "Game TIme: " + Mathf.Floor(Time.realtimeSinceStartup));
           GUI.Label(new Rect(Screen.width - 100, 180, 500, 30), "Barrls Beat: " + barrelsBeat);
           GUI.Label(new Rect(Screen.width - 100, 200, 500, 30), "Record: " + DifficultyManager.barrelsBeatRecord);
           GUI.Label(new Rect(Screen.width - 100, 220, 500, 30), "Difficulty: " + DifficultyManager.currentDifficulty);
           GUI.Label(new Rect(Screen.width - 100, 240, 500, 30), "CurrB Diff: " + (barrelHolingPlayer ? "" + barrelHolingPlayer.GetComponent<Barrel>().difficulty : "N/A"));
       } 
    }

    public IEnumerator RestartIfPlayerDies () {

        while (gameplayIsRunning) {

            if (playerScript.ShouldDie()) {
                RestartGame();
            }
            yield return new WaitForSeconds(0.25f);
        } 
    }

    public IEnumerator DestroyBarrelsInExcess () {

        while (gameplayIsRunning) {
            bool canDestroy = true;
            while (barrels.Count > DESIRED_MAX_BARRELS && canDestroy) {
                if (barrels[0].GetComponent<Barrel>().IsOffScreen()) {
                    Destroy(barrels[0]);
                    barrels.RemoveAt(0);
                }
                else {
                    canDestroy = false;
                }
            }
            yield return new WaitForSeconds(0.53f);
        }
    }

    public IEnumerator SmoothTimeScaleChange (float targetScale) {
        float duration = 1f;
        float current = 0f;

        float initialScale = Time.timeScale;

        float scaleDif = Mathf.Abs(targetScale - initialScale);
        int inverter = targetScale > initialScale ? 1 : -1;

        if (Time.timeScale != targetScale) {
            while (current < duration) {
                current += Time.unscaledDeltaTime;

                float easedTime = Utils.EaseValue(0, duration, current, Ease.QuadOut);

                float progress = Utils.ValueProgress(0, duration, easedTime);

                Time.timeScale = initialScale + progress * scaleDif * inverter;

                yield return null;
            }
        }
        Time.timeScale = targetScale;
    }

    public void PauseGameplay () {
        StartCoroutine(SmoothTimeScaleChange(0));
    }

    public void HandlePlayerFired () {

        player.GetComponent<Player>().StartMoreGravityOverTimeCoroutine();
        MoveBehaviour barrelHoldingPlayerMoveBehaviour = barrelHolingPlayer.GetComponent<MoveBehaviour>();
        barrelHoldingPlayerMoveBehaviour.Deactivate();
        barrelHoldingPlayerMoveBehaviour.ignoreActivation = true;

        prevBarrelHoldingPlayer = GameManager.inst.barrelHolingPlayer;
        barrelHolingPlayer = null;
        playerRB2D.AddRelativeForce(new Vector2(2000, 0));
    }

    public void HandleBarrelHit (GameObject barrelObjHit) {

        HandleBarrelBeat(barrelObjHit);

        float rotationDuration = DifficultyManager.MoveDurationBasedOnDifficulty();

        Barrel barrelObjHitBarrelScript = barrelObjHit.gameObject.GetComponent<Barrel>();

        barrelObjHitBarrelScript.HoldPlayer();

        int potentialNextBarrelIndex = barrels.IndexOf(barrelHolingPlayer) + 1;
        nextBarrel = potentialNextBarrelIndex < barrels.Count ? barrels[potentialNextBarrelIndex] : null;

        if (barrelsToJump > 0) {
            barrelObjHitBarrelScript.AutoFire(0.5f);
            barrelsToJump--;
        }
        else {
            MoveBehaviour barrelObjHitMoveBehaviour = barrelObjHit.gameObject.GetComponent<MoveBehaviour>();
            barrelObjHitMoveBehaviour.Activate(); //barrel will only move after the move duration, witch is the rotation duration
            barrelObjHitMoveBehaviour.RotateToInitialAngle(rotationDuration);
        }

        

        if (nextBarrel) {
            FocusCameraOnBarrel(nextBarrel);
        }

        HandleBarrelsJumped();

        
    }

    public void HandleBarrelsJumped () {

        int indexOfBarrelHoldingPlayer = barrels.IndexOf(barrelHolingPlayer);
        int indexOfPrevBarrelHoldingPlayer = barrels.IndexOf(prevBarrelHoldingPlayer);
        int numberOfBarrelsJumped = indexOfBarrelHoldingPlayer - indexOfPrevBarrelHoldingPlayer - 1;

        lastBarrelsJumped.Clear();
        for (int i = 0; i < numberOfBarrelsJumped; i++) {
            GameObject currentJumpedBarrel = barrels[indexOfPrevBarrelHoldingPlayer + i + 1];
            lastBarrelsJumped.Add(currentJumpedBarrel);
            HandleBarrelBeat(currentJumpedBarrel);
            
            currentJumpedBarrel.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    public void HandleBarrelBeat (GameObject barrelObj) {

        BarrelGenerator.inst.GenerateBarrel(DifficultyManager.currentDifficulty);

        barrelsBeat++;
        if (barrelsBeat > DifficultyManager.barrelsBeatRecord) {
            DifficultyManager.barrelsBeatRecord = barrelsBeat;
        }
        DifficultyManager.UpdateDifficulty();
    }

    public void HandeClickOrTouchAnywhere () {
        AttempToFirePlayer();
    }

    public void AttempToFirePlayer() {
        if (barrelHolingPlayer) {
            Barrel barrelHolingPlayerBarrelScript = barrelHolingPlayer.GetComponent<Barrel>();
            if (!barrelHolingPlayerBarrelScript.autoFiring) {
                barrelHolingPlayerBarrelScript.FirePlayer();
            }
        }
    }


    

    public void UnpouseGameplay () {
        StartCoroutine(SmoothTimeScaleChange(1));
    }

    

	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            AttempToFirePlayer();
        }

        

        if (nextBarrel && barrelHolingPlayer) {

            Vector3 dir = barrelHolingPlayer.transform.position - nextBarrel.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            nextBarrel.transform.localRotation = Quaternion.Lerp(
                nextBarrel.transform.localRotation, 
                Quaternion.Euler(0f, 0f, angle), 
                0.25f
                );
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            Debug.Log("test > " + mainCamera.GetComponent<Camera>().WorldToViewportPoint(player.transform.position));
        }

	}

    void OnApplicationQuit () {
        SaveAndLoad.Save();
    }
}
