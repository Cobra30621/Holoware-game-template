
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MicroMowGrass : MonoBehaviour
{
    public Sprite[] grassSprites;
    [HideInInspector] public MicroMowManager microgame;
    public Transform grassParticle;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = grassSprites[Random.Range(0, grassSprites.Length)];
        spriteRenderer.flipX = Random.value < 0.5f ? true : false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            grassParticle.transform.SetParent(microgame.transform);
            grassParticle.gameObject.SetActive(true);
            microgame.grassAmount--;
            microgame.sfx.PlaySFX(1);
            Destroy(gameObject);
        }
    }
}
