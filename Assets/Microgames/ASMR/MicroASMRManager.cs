using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicroASMRManager : MicroGameManager
{
    public SFXManager sfx, duckSfx;
    public AudioSource duckAudioSource;
    public Transform duck;
    public Animator duckAnimator;
    public ParticleSystem duckParticle;
    public SpriteRenderer human;
    public Sprite humanWin;
    public float maxQuackPower = 0.125f, effectRange = 90f, effectDrain = 0.333f;
    public AnimationCurve quackCurve;
    public Image bar, barOutline;
    float targetAngle, progress, duckAngle;

    

    public override void Game()
    {
        base.Game();
        bgm.PlayBGM(0);
        targetAngle = Random.Range(-90f, 90f);
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        while (timer > 0)
        {
            if (!cleared)
            {
                progress -= effectDrain * Time.deltaTime;
                if (progress < 0) progress = 0;
            }
            duckAngle = Utils.GetMousePosition().y >= duck.position.y ? 
                Mathf.Clamp(Vector2.SignedAngle(Vector2.up, Utils.GetMousePosition() - (Vector2)duck.position), -90f, 90f) : 
                -90f * Mathf.Sign(Utils.GetMousePosition().x);
            duck.transform.eulerAngles = new Vector3(0, 0, duckAngle);
            if (duckAngle < 0)
            {
                duckAnimator.transform.localEulerAngles = new Vector3(180f, 0, 0);
            } else
            {
                duckAnimator.transform.localEulerAngles = Vector3.zero;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Quack(duckAngle);
            }
            bar.fillAmount = progress;
            barOutline.color = Color.Lerp(barOutline.color, Color.black, 0.05f);
            yield return null;
        }
        yield return new WaitForSeconds(1);
        End();
    }

    void Quack(float angle)
    {
        duckAudioSource.panStereo = -angle / 90f;
        duckSfx.PlaySFX(0);
        duckParticle.Play();
        duckAnimator.SetTrigger("quack");

        float effect = quackCurve.Evaluate(Mathf.Clamp01(1 - Mathf.Abs(targetAngle - angle) / effectRange));
        progress += maxQuackPower * effect;
        barOutline.color = Color.Lerp(Color.black, Color.white, effect);

        if (progress >= 1 && !cleared)
        {
            cleared = true;
            sfx.PlaySFX(0);
            bar.color = Color.green;
            human.sprite = humanWin;
            
        }
    }

  
}
