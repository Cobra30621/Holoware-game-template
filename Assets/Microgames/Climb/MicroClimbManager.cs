using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MicroClimbManager :  MicroGameManager 
{
    public SFXManager sfx;
    public GameObject player;

    
    public override void Game()
    {
        base.Game();
        bgm.PlayBGM(0);
        player.SetActive(true);
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        
        yield return new  WaitUntil ( () => timer <= 0);
        End(1);
    }
        
}

