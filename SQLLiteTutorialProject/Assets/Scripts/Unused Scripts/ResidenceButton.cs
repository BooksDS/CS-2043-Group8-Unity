using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;

public class ResidenceButton : MonoBehaviour


{
    //Reference to the data table
    public TableUI table;
    //public Button resButton;

    
    void Start()
    {
        // lambda expression. I think it works by creating a temporary function that is "copying" TaskOnClick(), but don't quote me.
        //resButton.onClick.AddListener(() => TaskOnClick());

        //table.Rows = 3;
        //table.Columns = 4;
    }

    public void ShowResidence()
    {

        Debug.Log("Button has been clicked");
        //assigning headers to data table
        table.GetCell(0,0).text = "Residence";
        table.GetCell(0,1).text = "Address";
        table.GetCell(0,2).text = "Postal Code";
        table.GetCell(0,3).text = "Building Number";

        //opening database connection
        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM RESIDENCE";
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        //while datareader has something to read...
        while(dataReader.Read())
        {
            //table.Rows++;
            //hard coded cell data. reading info out of datareader as strings.
            table.GetCell(n,0).text = dataReader.GetString(1); //Building Name
            table.GetCell(n,1).text = dataReader.GetString(2); //Building Address
            table.GetCell(n,2).text = dataReader.GetString(3); //Building Postal Code
            table.GetCell(n,3).text = dataReader.GetInt32(0).ToString(); //Building Number
            n++;
            
            
        }

        //closing connection
        dbConnection.Close();
        
    }



    //implementation of interface for OpenDatabase   
    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        return dbConnection;
    }
}
