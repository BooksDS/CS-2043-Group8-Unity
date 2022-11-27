using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;

public class TestChangingValues : MonoBehaviour


{
    //Reference to the data table
    public TableUI table;


    // Start is called before the first frame update
    void Start()
    {
        //assigning headers to data table
        table.GetCell(0,0).text = "Residence";
        table.GetCell(0,1).text = "Address";

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
            //hard coded cell data. reading info out of datareader as strings.
            table.GetCell(n,0).text = dataReader.GetString(1);
            table.GetCell(n,1).text = dataReader.GetString(2);
            n++;
        }

        //closing connection
        dbConnection.Close();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //implimentation of interface for OpenDatabase   
    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        return dbConnection;
    }
}
