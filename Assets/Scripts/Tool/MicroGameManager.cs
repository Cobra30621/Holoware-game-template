using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroGameManager : MonoBehaviour
{
    // Every MicroGame Method
    [HideInInspector] public BGMManager bgm;
    public float initTime = 10; 
    public float timer;
    public float beatLength = 0.5f;
    public bool cleared; // a microgame is considered cleared if cleared = true
    public bool timeOver; // once set to true, the microgame will exit
    public TimerUI timerUI ;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        bgm = GetComponent<BGMManager>();
        Game();
    }

    // Update is called once per frame
    protected void Update()
    {
        Countdown();
    }

    protected virtual void Countdown(){
        if(timeOver){return;}

        timer -= Time.deltaTime;
        if(timer < 0){
            timeOver = true;
        }
    }

    [ContextMenu("Game Start")]
    public virtual void Game(){
        timer = initTime;
        if (timerUI)
        {
            timerUI.gameObject.SetActive(true);
        }
    }

    [ContextMenu("Game End")]
    public virtual void End(){
        if(cleared)
            Debug.Log("Game End : Win");
        else
            Debug.Log("Game End : Lose");
        
    }
}
