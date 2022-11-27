using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class RoomDrillDown : MonoBehaviour
{
    [SerializeField] private TMP_InputField bldg_num_field;
    [SerializeField] private TableUI roomTable;
    public void DrillDownRooms()
    {
        string bldg_num = bldg_num_field.text;

        IDbConnection dbConnection = OpenDatabase();

        //Create command
        IDbCommand getRooms = dbConnection.CreateCommand();
        getRooms.CommandText = "select BLDG_ABBV, ROOM_NUM, SIZE, NUM_STUDENTS, OCCUPANCY from ROOM_PLUS where BLDG_NUM = @bldg";
        
        //Assign building number as parameter
        var bldgParam = getRooms.CreateParameter();
        bldgParam.ParameterName = "@bldg";
        bldgParam.Value = bldg_num;

        //Prepare parameters
        getRooms.Parameters.Add(bldgParam);
        getRooms.Prepare();

        IDataReader dataReader = getRooms.ExecuteReader();

        int n = 1;

        while(dataReader.Read())
        {
            roomTable.GetCell(n,0).text = dataReader.GetString(0) + dataReader.GetInt32(1).ToString(); //Room Number (Building abbreviation + num.)
            roomTable.GetCell(n,1).text = dataReader.GetInt32(2).ToString();
            roomTable.GetCell(n,2).text = dataReader.GetInt32(3).ToString() + "/" + dataReader.GetInt32(4).ToString();
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
