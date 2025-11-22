using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartScreen : MonoBehaviour
{ 
    [SerializeField]
    private Button button; 
    private void Start()
    {
        button.onClick.AddListener(() => StartGame());
    }
    
    private void StartGame()
    {
        SceneManager.LoadScene("Market");
    }
}
