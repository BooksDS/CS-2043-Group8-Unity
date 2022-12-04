using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;
using UnityEngine.UI;
using TMPro;

public class DrillDownSecondLayer : MonoBehaviour
{

    public static DrillDownSecondLayer instance;

    [SerializeField] public TableUI table;
    [SerializeField] private GameObject prefab;



    Color selectedTableColor = new Color(1f, 0.5882353f, 0f);


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void RefreshTable()
    {
        for (int i = 0; i < table.Rows; i++)
        {

            TMP_Text text = transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>();
            Debug.Log(text);
            text.text = table.GetCell(i, 0).text;


        }
    }

    public void ClearButtons()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        UpdateButtons();

    }

    //Clears buttons without adding new ones
    public void DestroyButtons()
    {


        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }


    }

    // Update is called once per frame
    public void UpdateButtons()
    {
        for (int i = 0; i < table.Rows; i++)
        {

            //Debug.Log(transform.childCount + "   " + table.Rows );
            GameObject buttonInstance;
            buttonInstance = Instantiate(prefab, transform);
            Image image = buttonInstance.GetComponent<Image>();
            DrillDownTwo controller = buttonInstance.GetComponent<DrillDownTwo>();

            if (i == 0)
            {

                Button button = buttonInstance.GetComponent<Button>();

                buttonInstance.transform.GetChild(0).gameObject.SetActive(false);

                image.color = new Color(0f, 0f, 0f, 0f);
                button.interactable = false;


            }
            else
            {
                image.color = selectedTableColor;
                TMP_Text text = buttonInstance.transform.GetChild(0).GetComponent<TMP_Text>();
                text.text = table.GetCell(i, 0).text;

            }




            //instantiate prefab, assign to row, first item.
        }

    }



}


