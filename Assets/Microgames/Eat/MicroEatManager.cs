using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroEatManager : MonoBehaviour
{
    public MicroEatPlayer player;
    public MicroEatSet[] sets;
    public GameObject bombDrop;
    public float dropInterval = 0.1f, foodSpeed = 3f, hazardSpeed = 5f;
    public int hazardInterval = 10;
    int playerID, dropsSpawned;

    // Every MicroGame Method
    [HideInInspector] public BGMManager bgm;
    public float start_time = 10;
    public float timer; 
    public bool cleared; // a microgame is considered cleared if cleared = true
    public bool timeOver; // once set to true, the microgame will exit

    // Start is called before the first frame update
    void Start()
    {
        bgm = GetComponent<BGMManager>();
        timer = start_time;
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
        playerID = Random.Range(0, sets.Length);
        player.spriteRenderer.sprite = sets[playerID].run;
        player.failSprite = sets[playerID].fail;
        Debug.Log(playerID);

        bgm.PlayBGM(playerID);
        StartCoroutine(DropFood());
    }

    IEnumerator DropFood()
    {
        Rigidbody2D food;

        while (timer > 0)
        {
            dropsSpawned++;
            if (dropsSpawned % hazardInterval != 0)
            {
                food = Instantiate(sets[playerID].drop, new Vector2(Random.Range(-6f, 6f), 6f), Quaternion.identity, transform).GetComponent<Rigidbody2D>();
                food.velocity = new Vector2(0, -foodSpeed);
                food.angularVelocity = Random.Range(-360f, 360f);
            } else
            {
                food = Instantiate(bombDrop, new Vector2(Random.Range(-6f, 6f), 6f), Quaternion.identity, transform).GetComponent<Rigidbody2D>();
                food.velocity = new Vector2(0, -hazardSpeed);
                food.angularVelocity = Random.Range(-360f, 360f);
            }
            yield return new WaitForSeconds(dropInterval);
        }
    }

    [System.Serializable]
    public class MicroEatSet {
        public Sprite run, fail;
        public GameObject drop;
    }

    [ContextMenu("Game End")]
    public void End(){
        if(cleared)
            Debug.Log("Game End : Win");
        else
            Debug.Log("Game End : Lose");
    }
}
