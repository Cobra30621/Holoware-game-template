using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class MicroSolveManager : MonoBehaviour
{
    public SFXManager sfx;
    public NumberPad numberPad;
    public TextMeshProUGUI blackboard;
    public Animator chocoAnimator;
    int[] num;
    int op; // 0 = add, 1 = subtract, 2 = multiply, 3 = divide
    int numAsX;

    // Every MicroGame Method
    [HideInInspector] public BGMManager bgm;
    public float start_time = 10;
    public float timer; 
    public bool cleared; // a microgame is considered cleared if cleared = true
    public bool timeOver; // once set to true, the microgame will exit
    

    void Start()
    {
        bgm = GetComponent<BGMManager>();
        
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
        timer = start_time;

        num = new int[3];
        blackboard.text = "";
        op = Random.Range(0, 4);

        numAsX = 2;
        switch (op)
        {
            case 0:
                num[0] = Random.Range(0, 26);
                num[1] = Random.Range(0, 26);
                num[2] = num[0] + num[1];
                break;
            case 1:
                num[0] = Random.Range(0, 26);
                num[1] = Random.Range(0, num[0] + 1);
                num[2] = num[0] - num[1];
                break;
            case 2:
                num[0] = Random.Range(0, 10);
                num[1] = Random.Range(0, 10);
                num[2] = num[0] * num[1];
                break;
            case 3:
                num[2] = Random.Range(1, 10);
                num[1] = Random.Range(1, 10);
                num[0] = num[1] * num[2];
                break;
        }

        

        if (numAsX != 0)
        {
            blackboard.text += num[0] >= 0 ? num[0].ToString() : "(" + num[0] + ")";
        }
        else
        {
            blackboard.text += "<b><i>x</i></b>";
        }
        switch (op)
        {
            case 0: blackboard.text += " + "; break;
            case 1: blackboard.text += " - "; break;
            case 2: blackboard.text += " × "; break;
            case 3: blackboard.text += " ÷ "; break;
        }
        if (numAsX != 1)
        {
            blackboard.text += num[1] >= 0 ? num[1].ToString() : "(" + num[1] + ")";
        }
        else
        {
            blackboard.text += "<b><i>x</i></b>";
        }
        blackboard.text += " = ";
        if (numAsX != 2)
        {
            blackboard.text += num[2] >= 0 ? num[2].ToString() : "(" + num[2] + ")";
        }
        else
        {
            blackboard.text +=  "?";
        }


        numberPad.gameObject.SetActive(true);

        bgm.PlayBGM(0);
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        while (timer > 0)
        {
            if (cleared)
            {
                break;
            }
            yield return null;
        }
        numberPad.canUse = false;
    }

    public void CheckAnswer()
    {
        if (!numberPad.canUse) return;
        numberPad.canUse = false;
        if (int.Parse(numberPad.stringInput) == num[numAsX])
        {
            cleared = true;
            numberPad.display.color = Color.green;
            chocoAnimator.SetTrigger("win");
            sfx.PlaySFX(0);
            End();
        }
        else
        {
            numberPad.display.color = Color.red;
            chocoAnimator.SetTrigger("fail");
            sfx.PlaySFX(1);
            End();
        }
    }

    [ContextMenu("Game End")]
    public void End(){
        if(cleared)
            Debug.Log("Game End : Win");
        else
            Debug.Log("Game End : Lose");
    }
}

