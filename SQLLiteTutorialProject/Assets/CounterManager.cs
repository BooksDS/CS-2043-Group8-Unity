using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterManager : MonoBehaviour
{
    public static CounterManager instance;
    private TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        if (!PlayerPrefs.HasKey("CounterTotal"))
        {
            PlayerPrefs.SetInt("CounterTotal", 0);
        }

        text = GetComponent<TMP_Text>();

        UpdatCounter();
    }

    // Update is called once per frame
    public void UpdatCounter()
    {
        text.text = "Total Moves: " + PlayerPrefs.GetInt("CounterTotal");
    }
}
