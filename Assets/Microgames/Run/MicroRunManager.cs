using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroRunManager : MonoBehaviour
{
    public SFXManager sfx;
    public MicroRunPlayer player;
    public MicroRunSet[] sets;
    public MicroRunSpawn[] spawnObjects;
    public int spawnAmount;
    public float firstSpawnPosition = 15f, maxSpawnDistance = 50f, spawnIntervalVariation = 2f;
    int playerID, spawnID;
    float spawnX;

    // Every MicroGame Method
    [HideInInspector] public BGMManager bgm;
    public float start_time = 10;
    public float timer; 
    public bool cleared; // a microgame is considered cleared if cleared = true
    public bool timeOver; // once set to true, the microgame will exit

    public void Start()
    {
        spawnX = firstSpawnPosition;
        while (spawnX < maxSpawnDistance)
        {
            spawnID = Random.Range(0, spawnObjects.Length);
            Instantiate(spawnObjects[spawnID].spawnObject, new Vector2(spawnX, 0), Quaternion.identity, transform);
            spawnX += spawnObjects[spawnID].intervalToNextSpawn + Random.Range(0, spawnIntervalVariation);
        }
        
        cleared = false;
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
    public void Game()
    {
        playerID = Random.Range(0, sets.Length);
        player.gameObject.SetActive(true);
        player.runSprite = sets[playerID].run;
        player.slideSprite = sets[playerID].slide;
        player.failSprite = sets[playerID].fail;
        player.spriteRenderer.sprite = player.runSprite;
        bgm.PlayBGM(0);
    }

    [ContextMenu("Game End")]
    public void End(){
        if(cleared)
            Debug.Log("Game End : Win");
        else
            Debug.Log("Game End : Lose");
    }

    [System.Serializable]
    public class MicroRunSet
    {
        public Sprite run, slide, fail;
    }

    [System.Serializable]
    public class MicroRunSpawn
    {
        public GameObject spawnObject;
        public float intervalToNextSpawn;
    }
}

