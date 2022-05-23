using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Need Script
// Util



public class MicroClimbManager : MonoBehaviour
{
    public SFXManager sfx;
    public GameObject player;

    // Every MicroGame Method
    [HideInInspector] public BGMManager bgm;
    public float start_time = 10;
    public float timer; 
    public bool cleared; // a microgame is considered cleared if cleared = true
    public bool timeOver; // once set to true, the microgame will exit; must be set manually if useStandardTimer = false

    public void Start()
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
    public void Game()
    {
        player.SetActive(true);
        bgm.PlayBGM(0);
    }

    [ContextMenu("Game End")]
    public void End(){
        Debug.Log("Game End");
    }


}