using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using TMPro;
using System;
using Mono.Data.Sqlite;
using System.Data;

public class DrillDownTwo : MonoBehaviour
{

    [SerializeField] private TMP_InputField bldg_num_field;
    [SerializeField] private TMP_InputField room_num_field;


    public void OpenDrillDown()
    {
        GameManager.instance.drillDownBackground.SetActive(true);
        GameManager.instance.drillDownPanel.SetActive(true);

        //Debug.Log(DrilldownButtonManager.instance.table.name);

        //Debug.Log(column1);

        string text = transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

        RoomDrillDown();

    }

    //Used for viewing the Room table in the Residence Drill-down view
    public void RoomDrillDown()
    {
        DrillDownSecondLayer.instance.DestroyButtons();
        GameManager.instance.drilldownReturnButton.SetActive(true);

        TableUI drillTable = GameManager.instance.drillDownTable;

        //Clear the Drilldown table
        //drillTable.gameObject.SetActive(false);

        TMP_Text title = GameManager.instance.title;
        TMP_Text subtitle = GameManager.instance.subtitle;
        //TMP_Text sublabel1 = GameManager.instance.sublabel1;

        IDbConnection dbConnection = OpenDatabase();

        //Get Residence name from button
        string roomName = transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;
        string abbv = null;
        string roomNum = null;

        foreach (char c in roomName)
            {
                if (Char.IsLetter(c))
                {
                    abbv += c;
                }
                if (Char.IsNumber(c))
                {
                    roomNum += c;
                }
            }


        title.text = roomName;
        subtitle.text = "Occupancy History";
        //sublabel1.text = ""; //Hide sublabel for this screen.

        IDbCommand getRooms = dbConnection.CreateCommand();
        getRooms.CommandText = "select MOVE_IN_DATE, MOVE_OUT_DATE, STUDENT_ID, KEY_NUM from (STUDENT_HIST left join RESIDENCE on STUDENT_HIST.BLDG_NUM = RESIDENCE.BLDG_NUM) where (BLDG_ABBV = @abbv and ROOM_NUM = @num)";

        //Assign building number as parameter
        var abbvParam = getRooms.CreateParameter();
        abbvParam.ParameterName = "@abbv";
        abbvParam.Value = abbv;

        var roomParam = getRooms.CreateParameter();
        roomParam.ParameterName = "@num";
        roomParam.Value = roomNum;

        //Prepare parameters
        getRooms.Parameters.Add(abbvParam);
        getRooms.Parameters.Add(roomParam);
        getRooms.Prepare();

        IDataReader dataReader = getRooms.ExecuteReader();

        int n = 1;

        drillTable.Rows = n;

        //Set headings
        drillTable.GetCell(0, 0).text = "Move-in Date";
        drillTable.GetCell(0, 1).text = "Move-out Date";
        drillTable.GetCell(0, 2).text = "Student ID";
        drillTable.GetCell(0, 3).text = "Key Number";

        while (dataReader.Read())
        {
            drillTable.Rows = n + 1;
            drillTable.transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            drillTable.GetCell(n,0).text = dataReader.GetDateTime(0).ToString(); //Move In Date.
            drillTable.GetCell(n,1).text = dataReader.IsDBNull(1) ? "N/A" : dataReader.GetDateTime(1).ToString() ; //Move Out Date - check if null.
            drillTable.GetCell(n,2).text = dataReader.GetInt32(2).ToString(); //Student Number
            drillTable.GetCell(n,3).text = dataReader.GetInt32(3).ToString(); //Key Number
            n++;
        }

        //closing connection
        dbConnection.Close();

    }

    


    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);

        dbConnection.Open();

        return dbConnection;
    }
}


