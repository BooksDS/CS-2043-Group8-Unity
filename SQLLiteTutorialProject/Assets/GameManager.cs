using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameObject drillDownBackground;

    public GameObject drillDownPanel;

    public TableUI drillDownTable;

    public TMP_Text title;

    public TMP_Text subtitle;

    public TMP_Text sublabel1;

    public StudentUpdater studentUpdater;

    public Toggle eligToggle;

    public TMP_InputField eligReason;

    public GameObject errorPanel;

    private void Start()
    {
        instance = this;
    }

    public void CloseDrillDown()
    {
        drillDownBackground.SetActive(false);
        drillDownPanel.SetActive(false);
    }

    public void CloseError()
    {
        errorPanel.SetActive(false);
    }
}
