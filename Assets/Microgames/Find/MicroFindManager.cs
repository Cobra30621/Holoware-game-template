using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroFindManager : MicroGameManager
{
    
    public SFXManager sfx;
    public int numberOfHeads = 25;
    public float scaleMult = 1f, spawnXBound = 6f, spawnYLowerBound = -3.5f, spawnYUpperBound = 4f;
    public Sprite[] sprites;
    public GameObject head;
    List<GameObject> heads;
    GameObject targetHead, clickedHead;


    public override void Game(){
        base.Game();
        heads = new List<GameObject>();
        MicroFindHead newHead;
        for (int i = 0; i < numberOfHeads; i++)
        {
            if (i == 0)
            {
                newHead = Instantiate(head, transform).GetComponent<MicroFindHead>();
                newHead.face.sprite = sprites[0];
                newHead.sortingGroup.sortingOrder = -10;
                newHead.tag = "Goal";
                targetHead = newHead.gameObject;
                newHead.transform.position = new Vector2(Random.Range(-spawnXBound, spawnXBound), Random.Range(spawnYLowerBound, spawnYUpperBound));
            } else
            {
                newHead = Instantiate(head, transform).GetComponent<MicroFindHead>();
                newHead.face.sprite = sprites[Random.Range(1, sprites.Length)];
                newHead.transform.position = new Vector2(Random.Range(-spawnXBound, spawnXBound), Random.Range(spawnYLowerBound, spawnYUpperBound));
            }
            newHead.hat.SetActive(Random.value < 0.5f ? true : false);
            newHead.transform.localScale = Vector3.one * scaleMult;
            heads.Add(newHead.gameObject);
        }
        bgm.PlayBGM(0);
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        while (!cleared && !timeOver)
        {
            yield return null;
            if (Input.GetMouseButtonDown(0))
            {
                clickedHead = null;
                RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.GetMousePosition(), Vector2.zero);
                if (hits.Length > 0)
                {
                    foreach (RaycastHit2D i in hits)
                    {
                        if (i.collider.tag == "Pickup")
                        {
                            if (!clickedHead)
                            {
                                clickedHead = i.collider.gameObject;
                            }
                            else
                            {
                                if (((Vector2)i.collider.transform.position - Utils.GetMousePosition()).magnitude <
                                    ((Vector2)clickedHead.transform.position - Utils.GetMousePosition()).magnitude)
                                {
                                    clickedHead = i.collider.gameObject;
                                }
                            }
                        }

                        if (i.collider.tag == "Goal")
                        {
                            cleared = true;
                            clickedHead = targetHead;
                            break;
                        }
                    }
                }

                if (clickedHead)
                {
                    if (cleared)
                    {
                        sfx.PlaySFX(0);
                    }
                    else
                    {
                        sfx.PlaySFX(1);
                    }

                    for (int i = 0; i < heads.Count; i++)
                    {
                        if (heads[i] != targetHead && heads[i] != clickedHead)
                        {
                            heads[i].SetActive(false);
                        }
                    }

                    iTween.ScaleFrom(clickedHead,
                        iTween.Hash("scale", clickedHead.transform.localScale * 1.2f, "time", 0.25f, "easetype",
                            iTween.EaseType.easeOutBack));
                    End(1);
                    yield break;
                }
            }
        }
        End(1);
    }

 
}
