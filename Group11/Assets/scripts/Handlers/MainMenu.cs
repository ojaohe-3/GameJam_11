using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private bool _isHost;
    private string _host = "";
    private string _playerName = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHost(string host)
    {
        _host = host;
    }

    public void SetPlayerName(string name)
    {
        _playerName = name;
    }

    public void SetIsHost(bool isHost)
    {
        _isHost = isHost;
    }

    public void LaunchGame()
    {
        Debug.Log(_playerName);
        Debug.Log(_host);
        Debug.Log(_isHost);
        if (!_playerName.Equals("") && (!_host.Equals("") || _isHost))
        {
            GameHandler.PlayerName = _playerName;
            if (_isHost)
                GameHandler.Host = "";
            else
                GameHandler.Host = _host;
            SceneManager.LoadScene("game map");
        }
    }
}