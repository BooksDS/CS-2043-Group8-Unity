using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;
using UnityEngine.UI;
using TMPro;

public class DrilldownButtonManager : MonoBehaviour
{
    
    public static DrilldownButtonManager instance;

    [SerializeField] public TableUI table;
    [SerializeField] private GameObject prefab;

    
 
    Color selectedTableColor;

    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    private void RefreshTable()
    {
        for(int i = 0; i < table.Rows; i++)
        {
            
            TMP_Text text = transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>();
            Debug.Log(text);
            text.text = table.GetCell(i, 0).text;
            

        }
    }

    public void ClearButtons(TableUI Table, Color tableColor)
    {
        table = Table;

        selectedTableColor = tableColor;

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
         DrillDownButtonController controller = buttonInstance.GetComponent<DrillDownButtonController>();

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
            controller.column1 = table.GetCell(i, 1).text;
            

         }


                    
      
                    //instantiate prefab, assign to row, first item.
      }
                
    }
            
            
            
}
    

