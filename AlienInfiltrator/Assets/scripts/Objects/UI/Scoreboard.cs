using System;
using TMPro;
using UnityEngine;

namespace Objects.UI
{
    public class Scoreboard : MonoBehaviour
    {
        private TMP_Text _tmp;

        private GameHandler _gm;
        // Start is called before the first frame update
        void Start()
        {
            _gm = GetComponentInParent<GameHandler>();
            _tmp = GetComponent<TMP_Text>();
            
        }

        private void FixedUpdate()
        {
            _tmp.text = _gm.CurrentScore + "/" + _gm.MaxScore;
        }
    }
}
