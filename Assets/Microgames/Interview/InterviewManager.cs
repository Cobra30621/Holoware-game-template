using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Localization;

public class InterviewManager : MonoBehaviour
{
        // Every MicroGame Method
    [HideInInspector] public BGMManager bgm;
    public float start_time = 10;
    public float timer; 
    public bool cleared; // a microgame is considered cleared if cleared = true
    public bool timeOver; // once set to true, the microgame will exit

    public SFXManager sfx;
    public float bpm = 136, safeBeats = 0.5f;
    public int lives = 3;
    public SpeechSetArray speechSetArray;
    public GameObject matsuriBubble;
    public TextMeshProUGUI matsuriSpeech1, matsuriSpeech2;
    public Text livesDisp;
    public RectTransform[] avatarButtonContainers;
    public TextMeshProUGUI[] avatarBubbles;
    public Animator matsuriAnimator;
    float nextEventTime, failFXTimer;
    int nextEventBeat, correctAnswerIndex, correctCharacterIndex;
    List<CharacterAvatar> buttonAvatars;
    List<int> setPool, availableCharacters, choices;
    public bool canSelect;

    // Start is called before the first frame update
    void Start()
    {
        buttonAvatars = new List<CharacterAvatar>();
        setPool = Utils.GenerateIndexPool(speechSetArray.sets.Length);
        availableCharacters = Utils.GenerateIndexPool(speechSetArray.sets.Length);


        bgm = GetComponent<BGMManager>();
        timer = start_time;
        Game();
    }

    // Update is called once per frame
    void Update()
    {
        // Countdown();
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
        matsuriAnimator.speed = bpm / 60f;
        SetNextEventTime(4);
        livesDisp.text = lives.ToString();

        bgm.PlayBGM(0);
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        while (bgm.audioSource.time < nextEventTime) yield return null;
        matsuriBubble.SetActive(true);
        SetMatsuriText("Interview/Intro1", true, false);
        SetNextEventTime(8);
        while (bgm.audioSource.time < nextEventTime) yield return null;
        SetMatsuriText("Interview/Intro2", true, false);
        SetNextEventTime(4);
        while (bgm.audioSource.time < nextEventTime) yield return null;
        matsuriSpeech1.text = "";
        for (int i = 3; i >= 0; i--)
        {
            SetMatsuriText(i > 0 ? i.ToString() : "", false, true);
            SetNextEventTime(1);
            while (bgm.audioSource.time < nextEventTime) yield return null;
        }
        for (int i = 0; i < 7; i++)
        {
            StartCoroutine(FillButtons());
            SetMatsuriText("Interview/" + speechSetArray.sets[correctCharacterIndex].name, true, true);
            SetNextEventTime(4);
            while (bgm.audioSource.time < nextEventTime) yield return null;
            if (canSelect)
            {
                lives--;
                livesDisp.text = lives.ToString();
                sfx.PlaySFX(2);
            }
            if (lives == 0)
            {
                livesDisp.color = Color.red;
                break;
            }
        }
        foreach (CharacterAvatar i in buttonAvatars)
        {
            Destroy(i.gameObject);
        }
        buttonAvatars.Clear();
        for (int i = 0; i < avatarBubbles.Length; i++)
        {
            avatarBubbles[i].transform.parent.gameObject.SetActive(false);
        }
        canSelect = false;
        if (lives == 0)
        {
            matsuriAnimator.SetTrigger("fail");
            SetMatsuriText("Interview/Fail", true, true);
            failFXTimer = 3f;
            while (failFXTimer > 0)
            {
                bgm.audioSource.pitch = Mathf.Clamp(bgm.audioSource.pitch - 0.333f * Time.deltaTime, 0f, Mathf.Infinity);
                failFXTimer -= Time.deltaTime;
                yield return null;
            }
            timeOver = true;
            yield break;
        }
        SetMatsuriText("Interview/Intermission", true, false);
        SetNextEventTime(4);
        while (bgm.audioSource.time < nextEventTime) yield return null;
        for (int i = 0; i < 8; i++)
        {
            StartCoroutine(FillButtons());
            SetMatsuriText("Interview/" + speechSetArray.sets[correctCharacterIndex].name, true, true);
            SetNextEventTime(4);
            while (bgm.audioSource.time < nextEventTime) yield return null;
            if (canSelect)
            {
                lives--;
                livesDisp.text = lives.ToString();
                sfx.PlaySFX(2);
            }
            if (lives == 0)
            {
                livesDisp.color = Color.red;
                break;
            }
        }
        foreach (CharacterAvatar i in buttonAvatars)
        {
            Destroy(i.gameObject);
        }
        buttonAvatars.Clear();
        for (int i = 0; i < avatarBubbles.Length; i++)
        {
            avatarBubbles[i].transform.parent.gameObject.SetActive(false);
        }
        canSelect = false;
        if (lives == 0)
        {
            matsuriAnimator.SetTrigger("fail");
            SetMatsuriText("Interview/Fail", true, true);
            failFXTimer = 3f;
            while (failFXTimer > 0)
            {
                bgm.audioSource.pitch = Mathf.Clamp(bgm.audioSource.pitch - 0.333f * Time.deltaTime, 0f, Mathf.Infinity);
                failFXTimer -= Time.deltaTime;
                yield return null;
            }
            timeOver = true;
            yield break;
        }
        cleared = true;
        SetMatsuriText("Interview/End", true, false);
        sfx.PlaySFX(0);
        matsuriAnimator.SetTrigger("win");
        yield return new WaitForSeconds(3f);
        timeOver = true;
    }

    IEnumerator FillButtons()
    {
        canSelect = false;
        foreach (CharacterAvatar i in buttonAvatars)
        {
            Destroy(i.gameObject);
        }
        buttonAvatars.Clear();
        for (int i = 0; i < avatarBubbles.Length; i++)
        {
            avatarBubbles[i].transform.parent.gameObject.SetActive(false);
        }
        correctCharacterIndex = availableCharacters[Random.Range(0, availableCharacters.Count)];
        availableCharacters.Remove(correctCharacterIndex);
        choices = Utils.RandomFromIntPool(setPool, avatarButtonContainers.Length, false);
        bool charInChoices = false;
        for (int i = 0; i < choices.Count; i++)
        {
            if (choices[i] == correctCharacterIndex)
            {
                charInChoices = true;
                correctAnswerIndex = i;
                break;
            }
        }
        if (!charInChoices)
        {
            correctAnswerIndex = Random.Range(0, avatarButtonContainers.Length);
            choices[correctAnswerIndex] = correctCharacterIndex;
        }
        for (int i = 0; i < choices.Count; i++)
        {
            buttonAvatars.Add(Instantiate(speechSetArray.sets[choices[i]].avatar, avatarButtonContainers[i]).GetComponent<CharacterAvatar>());
            buttonAvatars[i].SetExpression(0);
        }
        yield return new WaitForSeconds(60f / bpm * safeBeats);
        canSelect = true;
    }

    public void CheckAnswer(int buttonIndex)
    {
        if (!canSelect) return;
        avatarBubbles[buttonIndex].transform.parent.gameObject.SetActive(true);
        avatarBubbles[buttonIndex].text = LeanLocalization.GetTranslationText("Interview/" + speechSetArray.sets[choices[buttonIndex]].name + "_Greeting_" + Random.Range(0, speechSetArray.sets[choices[buttonIndex]].numberOfLines));
        if (buttonIndex == correctAnswerIndex)
        {
            buttonAvatars[buttonIndex].SetExpression(1);
            sfx.PlaySFX(1);
        } else
        {
            buttonAvatars[buttonIndex].SetExpression(1);
            buttonAvatars[correctAnswerIndex].SetExpression(2);
            sfx.PlaySFX(2);
            lives--;
            livesDisp.text = lives.ToString();
            if (lives == 0) livesDisp.color = Color.red;
        }
        canSelect = false;
    }

    public void SetNextEventTime(int beats)
    {
        nextEventBeat += beats;
        nextEventTime = 60f / bpm * nextEventBeat;
    }

    public void SetMatsuriText(string text, bool localize, bool bigText)
    {
        if (!bigText)
        {
            matsuriSpeech1.text = localize ? LeanLocalization.GetTranslationText(text) : text;
            matsuriSpeech2.text = "";
        } else
        {
            matsuriSpeech2.text = localize ? LeanLocalization.GetTranslationText(text) : text;
            matsuriSpeech1.text = "";
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
