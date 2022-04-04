using System;
using System.Collections;
using System.Collections.Generic;
using mactinite.EDS.Basic;
using mactinite.ToolboxCommons;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Scene] public string gameScene;
    [Scene] public string menuScene;
    public GameObject gameOverScreen;
    public GameObject pauseScreen;
    public BasicDamageReceiver playerDamageReceiver;
    public CameraFollow followCam;
    public List<Collectable> AllCollectables;
    public bool paused = false;
    public GameObject collectableListUI;
    public GameObject collectableTextPrefab;
    public void Restart()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void Pause()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            ReDrawList();
            pauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);

        }
    }

    private void Start()
    {
        playerDamageReceiver.OnDestroyed += PlayerDead;
        followCam.input.actions["Pause"].performed += TogglePause;
        ReDrawList();

    }

    private void ReDrawList()
    {
        foreach (Transform child in collectableListUI.transform) {
            GameObject.Destroy(child.gameObject);
        }
        
        for (int i = 0; i < AllCollectables.Count; i++)
        {
            var textGO = Instantiate(collectableTextPrefab, collectableListUI.transform);
            var tmp = textGO.GetComponent<TMP_Text>();
            if (PlayerPrefs.GetInt(AllCollectables[i].CollectableName) == 1)
            {
                tmp.text =  AllCollectables[i].CollectableName;
            }
            else
            {
                tmp.text = "<color=grey> ?????? </color>";

            }
        }
    }

    private void TogglePause(InputAction.CallbackContext obj)
    {
        Pause();
    }

    private void PlayerDead(Vector2 pos)
    {
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        followCam.UnlockCursor();
        followCam.enabled = false;
        yield return new WaitForSeconds(0.5f);
        gameOverScreen.SetActive(true);

    }
}
