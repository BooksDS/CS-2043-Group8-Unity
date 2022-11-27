using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Mono.Data.Sqlite;
using System.Data;

public class SQLiteUI : MonoBehaviour
{

    //public TMP_Text sampleText;
    public String dbText;
    // Start is called before the first frame update
    void Start()
    {
        IDbConnection dbConnection = OpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT * FROM RESIDENCE";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while(dataReader.Read())
        {
            dbText = dbText + "\n" + dataReader.GetString(1);
        }

        dbConnection.Close();

        //sampleText.text = dbText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        return dbConnection;
    }

    //Looks like a prioir implimentation of TestChangingValues
}
