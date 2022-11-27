using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;

public class StudentsButton : MonoBehaviour
{
    //Reference to the data table
    public TableUI table;
    //public Button studentsButton;

    
    void Start()
    {
        // lambda expression. I think it works by creating a temporary function that is "copying" TaskOnClick(), but don't quote me.
        //studentsButton.onClick.AddListener(() => TaskOnClick());

        //table.Rows = 3;
        
    }

    public void ShowStudents()
    {
        table.Columns = 5; //Set columns to 5. Currently does not work

        Debug.Log("Student Button has been clicked");
        //assigning headers to data table
        table.GetCell(0,0).text = "Student Name";
        table.GetCell(0,1).text = "Student Number";
        table.GetCell(0,2).text = "Current Residence";
        table.GetCell(0,3).text = "Room Number";
        table.GetCell(0,4).text = "Eligibility";

        //opening database connection
        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM STUDENT_PLUS"; //STUDENT_PLUS is the view that adds building name to student list
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        int n = 1;

        //while datareader has something to read...
        while(dataReader.Read())
        {
            //table.Rows++;
            //hard coded cell data. reading info out of datareader as strings.
            table.GetCell(n,0).text = dataReader.GetString(0); //Student Name
            table.GetCell(n,1).text = dataReader.GetInt64(1).ToString(); //Student Number
            table.GetCell(n,2).text = dataReader.GetString(2); //Current Residence
            table.GetCell(n,3).text = dataReader.GetInt32(3).ToString(); //Room Number
            table.GetCell(n,4).text = dataReader.GetBoolean(4).ToString(); //Eligibible for Residence
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
