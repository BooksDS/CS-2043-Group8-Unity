using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.SceneManagement;


public class ButtonScript : MonoBehaviour
{
    public TableUI[] table = new TableUI[5];
    public DrilldownButtonManager drillDownButtonManager;

    public Button BlacklistButton;

    Color tableGreen = new Color(0f, 0.901f, 0f);
    Color tableBlue = new Color(0f, 0.5712419f, 0.9019608f);
    Color tableYellow = new Color(0.9019608f, 0.9019608f, 0f);
    Color tableCyan = new Color(0f, 0.8720993f, 0.9019608f);

    private void Start()
    {
        ShowResidence();
    }

    public void ClearTable()
    {
        for(int i = 0; i < table.Length; i++)
        {
            table[i].gameObject.SetActive(false);
        }
    }


    public void ShowFurniture()
    {
        
        //opening database connection
        ClearTable();
        table[2].gameObject.SetActive(true);

        //Hide blacklist button
        BlacklistButton.gameObject.SetActive(false);

        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM FURNITURE_PLUS"; //STUDENT_PLUS is the view that adds building name to student list
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        
        //while datareader has something to read...
        while(dataReader.Read())
        {
            table[2].Rows = n + 1;
            table[2].transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            //hard coded cell data. reading info out of datareader as strings.
            table[2].GetCell(n,0).text = dataReader.GetInt32(0).ToString(); //Furniture ID
            table[2].GetCell(n,1).text = dataReader.GetString(1); //Furn. type
            table[2].GetCell(n,2).text = dataReader.GetString(2); //Furn. description
            table[2].GetCell(n,3).text = dataReader.GetString(3); //Residence Name

            //This adds the residence abbreviation to the room number
            table[2].GetCell(n,4).text = (dataReader.GetString(4)+dataReader.GetInt32(5).ToString()); //Room Number + abbrev
            n++;
            
        }

        drillDownButtonManager.DestroyButtons();
        //closing connection
        dbConnection.Close();

    }
    public void ShowStudents()
    {
       
        //opening database connection
        ClearTable();
        table[1].gameObject.SetActive(true);
        BlacklistButton.gameObject.SetActive(true);

        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM STUDENT_PLUS"; //STUDENT_PLUS is the view that adds building name to student list
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        
        //while datareader has something to read...
        while (dataReader.Read())
        {
            table[1].Rows = n + 1;
            table[1].transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);

            //hard coded cell data. reading info out of datareader as strings.
            table[1].GetCell(n,0).text = dataReader.GetString(0); //Student Name
            table[1].GetCell(n,1).text = dataReader.GetInt64(1).ToString(); //Student Number

            //For Residence and Room number, check if they're null before trying to print. Some students may not have been assigned rooms yet
            table[1].GetCell(n,2).text = (dataReader.IsDBNull(2) ? "(Not assigned)"  : dataReader.GetString(2)); //Current Residence

            //This adds the residence abbreviation to the room number
            table[1].GetCell(n,3).text = (dataReader.IsDBNull(4) ? "(Not assigned)"  :dataReader.GetString(3)+dataReader.GetInt32(4).ToString()); //Room Number + abbrev
            table[1].GetCell(n,4).text = dataReader.GetBoolean(5).ToString(); //Eligibible for Residence
            
            n++;
            
            
        }
        drillDownButtonManager.ClearButtons(table[1], tableBlue);
        //closing connection
        dbConnection.Close();
        
    }

    public void ShowBlacklist()
    {
        //opening database connection
        ClearTable();
        table[1].gameObject.SetActive(true);
        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM STUDENT_PLUS WHERE STUDENT_ELIGIBLE = FALSE"; //STUDENT_PLUS is the view that adds building name to student list
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        //Reset size of table even if no data has been read
        table[1].Rows = n;

        
        //while datareader has something to read...
        while (dataReader.Read())
        {
            table[1].Rows = n + 1;
            table[1].transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);

            //hard coded cell data. reading info out of datareader as strings.
            table[1].GetCell(n,0).text = dataReader.GetString(0); //Student Name
            table[1].GetCell(n,1).text = dataReader.GetInt64(1).ToString(); //Student Number

            //For Residence and Room number, check if they're null before trying to print. Some students may not have been assigned rooms yet
            table[1].GetCell(n,2).text = (dataReader.IsDBNull(2) ? "(Not assigned)"  : dataReader.GetString(2)); //Current Residence

            //This adds the residence abbreviation to the room number
            table[1].GetCell(n,3).text = (dataReader.IsDBNull(4) ? "(Not assigned)"  :dataReader.GetString(3)+dataReader.GetInt32(4).ToString()); //Room Number + abbrev
            table[1].GetCell(n,4).text = dataReader.GetBoolean(5).ToString(); //Eligibible for Residence
            
            n++;
            
            
        }
        drillDownButtonManager.ClearButtons(table[1], tableBlue);
        //closing connection
        dbConnection.Close();
    }

    public void ShowResidence()
    {
        
        //opening database connection
        ClearTable();
        table[0].gameObject.SetActive(true);
        BlacklistButton.gameObject.SetActive(false);

        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM RES_PLUS";
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        
        //while datareader has something to read...
        while (dataReader.Read())
        {
            table[0].Rows = n + 1;
            table[0].transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            //hard coded cell data. reading info out of datareader as strings.
            table[0].GetCell(n,0).text = dataReader.GetString(0); //Building Name
            table[0].GetCell(n,1).text = dataReader.GetString(1); //Building Address
            table[0].GetCell(n,2).text = dataReader.GetString(2); //Building Postal Code
            table[0].GetCell(n,3).text = dataReader.GetInt32(3).ToString() + "/" + dataReader.GetInt32(4).ToString(); // Occupied/Max
            table[0].GetCell(n,4).text = dataReader.GetInt32(5).ToString(); //Building Number
            n++;
            
            
        }
        drillDownButtonManager.ClearButtons(table[0], tableGreen);
        //closing connection
        dbConnection.Close();
        
    }

    public void showKeys()
    {
        ClearTable();
        table[3].gameObject.SetActive(true);
        BlacklistButton.gameObject.SetActive(false);

        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT ROOM_KEY.KEY_NUM, BLDG_NAME, BLDG_ABBV, ROOM_KEY.ROOM_NUM, STUDENT_ID FROM (ROOM_KEY INNER JOIN RESIDENCE ON ROOM_KEY.BLDG_NUM = RESIDENCE.BLDG_NUM) LEFT JOIN STUDENT ON ROOM_KEY.KEY_NUM = STUDENT.KEY_NUM";
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        while(dataReader.Read())
        {
            table[3].Rows = n + 1;
            table[3].transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            table[3].GetCell(n,0).text = dataReader.GetInt32(0).ToString(); //Key number
            table[3].GetCell(n,1).text = dataReader.GetString(1); //Building Name
            table[3].GetCell(n,2).text = dataReader.GetString(2) + dataReader.GetInt32(3).ToString(); //Building abbrev. + room num.
            table[3].GetCell(n,3).text = (dataReader.IsDBNull(4) ? "N/A" : dataReader.GetInt64(4).ToString());
            n++;
            

        }
        drillDownButtonManager.ClearButtons(table[3], tableCyan);
        dbConnection.Close();

    }

    public void showLog()
    {
        ClearTable();
        table[4].gameObject.SetActive(true);
        BlacklistButton.gameObject.SetActive(false);

        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM TRANSACTIONS";
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        while (dataReader.Read())
        {
            table[4].Rows = n + 1;
            table[4].transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            table[4].GetCell(n, 0).text = dataReader.GetDateTime(0).ToString(); //TimeStamp
            table[4].GetCell(n, 1).text = dataReader.GetString(1); //Transaction Log
            
            n++;


        }

        drillDownButtonManager.DestroyButtons();
        dbConnection.Close();

    }

    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        return dbConnection;
    }

    public void CloseWindow()
    {

        SceneManager.LoadScene(0);

    }
}
