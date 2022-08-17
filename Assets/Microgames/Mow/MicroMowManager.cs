using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroMowManager : MonoBehaviour
{
    // Every MicroGame Method
    [HideInInspector] public BGMManager bgm;
    public float start_time = 10;
    public float timer; 
    public bool cleared; // a microgame is considered cleared if cleared = true
    public bool timeOver; // once set to true, the microgame will exit

    public SFXManager sfx;
    public GameObject grass;
    public Transform player;
    public Rigidbody2D playerRb;
    public Animator playerAnimator;
    public int grassPatches = 2, grassPerPatch = 10;
    public float xBound = 6f, yBound = 4.5f, spawnXBound = 4.5f, spawnYBound = 3.5f, spawnRadius = 2f, playerSpeed = 6f, turnTime = 0.2f;
    public AnimationCurve turnCurve;
    [HideInInspector] public int grassAmount;
    Vector2 playerPosition, moveVector;
    Coroutine turn;
    float currentDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        bgm = GetComponent<BGMManager>();
        timer = start_time;
        GrassSpawn();
        playerPosition = player.position;
        Game();
    }

    // Update is called once per frame
    void Update()
    {
        Countdown();
        
    }

    void Countdown(){
        if(timeOver){return;}

        timer -= Time.deltaTime;
        if(timer < 0){
            timeOver = true;
        }
    }

    [ContextMenu("Game Start")]
    public void Game(){
        bgm.PlayBGM(0);
        StartCoroutine(GameCoroutine());
    }

    [ContextMenu("Game End")]
    public void End(){
        if(cleared)
            Debug.Log("Game End : Win");
        else
            Debug.Log("Game End : Lose");
    }
    
     IEnumerator GameCoroutine()
    {
        while (timer > 0)
        {
            if (grassAmount == 0 && !cleared)
            {
                cleared = true;
                sfx.PlaySFX(0);
            }
            moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            playerAnimator.SetFloat("speed", moveVector.magnitude);
            if (moveVector.magnitude > 0)
            {
                if (Mathf.Sign(moveVector.x) != currentDirection)
                {
                    currentDirection = Mathf.Sign(moveVector.x);
                    if (turn != null) StopCoroutine(turn);
                    turn = StartCoroutine(TurnCoroutine(Mathf.Sign(moveVector.x)));
                }
                playerPosition += Vector2.ClampMagnitude(moveVector, 1f) * playerSpeed * Time.fixedDeltaTime;
                playerPosition = new Vector2(Mathf.Clamp(playerPosition.x, -xBound, xBound), Mathf.Clamp(playerPosition.y, -yBound, yBound));
                playerRb.MovePosition(playerPosition);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator TurnCoroutine(float direction)
    {
        float startX = player.localScale.x;
        float endX = direction;
        float timer = 0;
        while (timer < turnTime)
        {
            timer += Time.deltaTime;
            player.localScale = new Vector3(Mathf.Lerp(startX, endX, turnCurve.Evaluate(timer / turnTime)), 1f, 1f);
            yield return null;
        }
        player.localScale = new Vector3(endX, 1f, 1f);
    }

    void GrassSpawn()
    {
        MicroMowGrass spawnedGrass;
        Vector2 patchPosition, spawnPosition;
        for (int i = 0; i < grassPatches; i++)
        {
            patchPosition = new Vector2(Random.Range(-spawnXBound, spawnXBound), Random.Range(-spawnYBound, spawnYBound));
            for (int j = 0; j < grassPerPatch; j++)
            {
                spawnPosition = patchPosition + Random.insideUnitCircle * spawnRadius;
                spawnPosition = new Vector2(Mathf.Clamp(spawnPosition.x, -xBound, xBound), Mathf.Clamp(spawnPosition.y, -yBound, yBound));
                spawnedGrass = Instantiate(grass, spawnPosition, Quaternion.identity, transform).GetComponent<MicroMowGrass>();
                spawnedGrass.microgame = this;
                grassAmount++;
            }
        }
    }
}
