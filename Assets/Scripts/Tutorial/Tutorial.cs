using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject panelTutsMiddle, panelTutsDown;
    [SerializeField] GameObject tutsMidBtnPrev, tutsMidBtnNext, tutsDownBtnPrev, tutsDownBtnNext;

    [SerializeField] Transform tutsMiddleParent, tutsDown1Parent, tutsDown2Parent;
    [SerializeField] GameObject catcherPlayerPrefab, catcherEnemyPrefab, pestPlayerPrefab, pestPrefab, ballPrefab;
    [SerializeField] TextMeshProUGUI textPoints, textAnnouncer;
    TutCatcher catcherPlayer, catcher;
    TutPest pestPlayer, pest;
    GameObject ball;
    int pagesCounter;
    bool tutPartOne;
    Coroutine crtn;

    void Start()
    {
        panelTutsDown.SetActive(false);
        tutsMidBtnPrev.SetActive(false);
        tutsDownBtnPrev.SetActive(false);

        foreach (Transform child in tutsMiddleParent)
            child.gameObject.SetActive(false);
        foreach (Transform child in tutsDown1Parent)
            child.gameObject.SetActive(false);
        foreach (Transform child in tutsDown2Parent)
            child.gameObject.SetActive(false);

        textPoints.text = "0 points";

        tutsMiddleParent.GetChild(0).gameObject.SetActive(true);
        tutsDown1Parent.GetChild(0).gameObject.SetActive(true);

        tutsDown2Parent.gameObject.SetActive(false);

        pagesCounter = 1;
        tutPartOne = true;
    }

    void Update()
    {
        if (tutPartOne && pest != null)
        {
            textPoints.text = catcherPlayer.Score + " points";
            tutsDownBtnNext.SetActive(pest.IsEliminated);
            if (pest.IsEliminated)
                crtn ??= StartCoroutine(AnnouncerText("Gothca!"));
            else if (catcherPlayer.IsEliminated)
            {
                crtn ??= StartCoroutine(AnnouncerText("Try again!"));
                catcherPlayer.Respawn();
            }
        }
        else if (!tutPartOne && catcher != null)
        {
            textPoints.text = pestPlayer.Score + " points";
            tutsDownBtnNext.SetActive(catcher.IsEliminated);
            if (catcher.IsEliminated)
                crtn ??= StartCoroutine(AnnouncerText("Nice dodge!"));
            else if (pestPlayer.IsEliminated)
            {
                crtn ??= StartCoroutine(AnnouncerText("Try again!"));
                pestPlayer.Respawn();
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main menu");
    }

    public void NextPageMid()
    {
        if (tutPartOne)
        {
            tutsMiddleParent.GetChild(0).gameObject.SetActive(false);
            tutsMiddleParent.GetChild(1).gameObject.SetActive(true);
            tutsMidBtnPrev.SetActive(true);
            tutsMidBtnNext.SetActive(false);
            pagesCounter = 2;
        }
    }

    public void PrevPageMid()
    {
        if (tutPartOne)
        {
            tutsMiddleParent.GetChild(1).gameObject.SetActive(false);
            tutsMiddleParent.GetChild(0).gameObject.SetActive(true);
            tutsMidBtnPrev.SetActive(false);
            tutsMidBtnNext.SetActive(true);
            pagesCounter = 1;
        }
    }

    public void NextPageDown()
    {
        if (tutPartOne)
        {
            int activePage = 0;
            for (int i = 0; i < tutsDown1Parent.childCount; i++)
                if (tutsDown1Parent.GetChild(i).gameObject.activeInHierarchy)
                {
                    activePage = i;
                    break;
                }

            if (activePage < tutsDown1Parent.childCount - 1)
            {
                tutsDown1Parent.GetChild(activePage).gameObject.SetActive(false);
                tutsDown1Parent.GetChild(activePage + 1).gameObject.SetActive(true);

                pagesCounter++;

                if (pagesCounter == 4 && ball == null)
                    ball = Instantiate(ballPrefab);

                else if (pagesCounter == 5 && pest == null)
                {
                    ball.gameObject.SetActive(false);
                    pest = Instantiate(pestPrefab).GetComponent<TutPest>();
                    tutsDownBtnNext.SetActive(false);
                }

                tutsDownBtnPrev.SetActive(activePage + 1 > 0);
                if (pest != null)
                    tutsDownBtnNext.SetActive(pest.IsEliminated);
                else
                    tutsDownBtnNext.SetActive(activePage + 1 < tutsDown1Parent.childCount - 1);
            }
            else
            {
                tutsDown1Parent.gameObject.SetActive(false);
                tutsDown2Parent.gameObject.SetActive(true);
                tutsDownBtnPrev.SetActive(false);
                pest.gameObject.SetActive(false);
                catcherPlayer.gameObject.SetActive(false);

                pestPlayer = Instantiate(pestPlayerPrefab).GetComponent<TutPest>();
                ball.gameObject.SetActive(true);

                tutPartOne = false;
            }
        }
    }

    public void PrevPageDown()
    {
        if (tutPartOne)
        {
            int activePage = 0;
            for (int i = 0; i < tutsDown1Parent.childCount; i++)
                if (tutsDown1Parent.GetChild(i).gameObject.activeInHierarchy)
                {
                    activePage = i;
                    break;
                }

            if (activePage > 0)
            {
                tutsDown1Parent.GetChild(activePage).gameObject.SetActive(false);
                tutsDown1Parent.GetChild(activePage - 1).gameObject.SetActive(true);

                pagesCounter--;

                tutsDownBtnPrev.SetActive(activePage - 1 > 0);
                tutsDownBtnNext.SetActive(activePage - 1 < tutsDown1Parent.childCount - 1);
            }
            else tutsDownBtnPrev.SetActive(false);
        }
    }

    public void NextPageDown2()
    {
        if (!tutPartOne)
        {
            int activePage = -1;
            for (int i = 0; i < tutsDown2Parent.childCount; i++)
                if (tutsDown2Parent.GetChild(i).gameObject.activeInHierarchy)
                {
                    activePage = i;
                    break;
                }

            if (activePage < tutsDown2Parent.childCount - 1)
            {
                if (activePage > -1)
                    tutsDown2Parent.GetChild(activePage).gameObject.SetActive(false);
                tutsDown2Parent.GetChild(activePage + 1).gameObject.SetActive(true);

                pagesCounter++;

                if (pagesCounter == 10)
                {
                    ball.gameObject.SetActive(false);
                    pestPlayer.hitRadiusImage.SetActive(true);
                }
                else if (pagesCounter == 11 && catcher == null)
                {
                    catcher = Instantiate(catcherEnemyPrefab).GetComponent<TutCatcher>();
                    catcher.goal = pestPlayer.transform;
                }

                tutsDownBtnPrev.SetActive(activePage + 1 > 0);
                tutsDownBtnNext.SetActive(activePage + 1 < tutsDown2Parent.childCount - 1);
            }
            else BackToMainMenu();
        }
    }

    public void PrevPageDown2()
    {
        if (!tutPartOne)
        {
            int activePage = 0;
            for (int i = 0; i < tutsDown2Parent.childCount; i++)
                if (tutsDown2Parent.GetChild(i).gameObject.activeInHierarchy)
                {
                    activePage = i;
                    break;
                }

            if (activePage > 0)
            {
                tutsDown2Parent.GetChild(activePage).gameObject.SetActive(false);
                tutsDown2Parent.GetChild(activePage - 1).gameObject.SetActive(true);

                pagesCounter--;

                tutsDownBtnPrev.SetActive(activePage - 1 > 0);
                tutsDownBtnNext.SetActive(activePage - 1 < tutsDown2Parent.childCount - 1);
            }
            else tutsDownBtnPrev.SetActive(false);
        }
    }

    public void PlayerTakeCatcher()
    {
        panelTutsMiddle.SetActive(false);
        panelTutsDown.SetActive(true);
        pagesCounter = 3;
        catcherPlayer = Instantiate(catcherPlayerPrefab).GetComponent<TutCatcher>();
    }

    IEnumerator AnnouncerText(string text)
    {
        textAnnouncer.gameObject.SetActive(true);
        textAnnouncer.text = text;

        yield return new WaitForSeconds(1.6f);

        textAnnouncer.gameObject.SetActive(false);
    }

}
