using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class StudentUpdater : MonoBehaviour
{

    public TMP_InputField bldg_num_field;
    public TMP_InputField room_num_field;
    public string student_id_field;

    public GameObject studentUpdaterParent;

    //Singleton architecture - this is used so we can create a global reference for the purposes of adding the student ID
    public static StudentUpdater instance;

    public void Start()
    {
        instance = this;
    }



    public void updateStudent()
    {
        string bldg_num = bldg_num_field.text;
        string room_num = room_num_field.text;
        string student_id = student_id_field;

        Debug.Log("Update:" + student_id);

        IDbConnection dbConnection = OpenDatabase();

        IDbCommand checkHist = dbConnection.CreateCommand();
        //Get the most recent history for that student
        checkHist.CommandText = "select * from STUDENT_HIST where STUDENT_ID = @z ORDER BY MOVE_IN_DATE DESC LIMIT 1";

        //Create reusable parameters

        var param1 = checkHist.CreateParameter();
        param1.ParameterName = "@x";
        param1.Value = bldg_num;

        var param2 = checkHist.CreateParameter();
        param2.ParameterName = "@y";
        param2.Value = room_num;

        var param3 = checkHist.CreateParameter();
        param3.ParameterName = "@z";
        param3.Value = student_id;


        checkHist.Parameters.Add(param3);
        checkHist.Prepare();
        IDataReader dataReader = checkHist.ExecuteReader();

        IDbCommand updater = dbConnection.CreateCommand();

        //If the most recent entry in the history table has no move_out date, add a move_out_date
        //The most recent date is found using a heinous subquery. This should be fixed.
        if (dataReader.Read() && dataReader.IsDBNull(1))
        {
            var moveInDate = checkHist.CreateParameter();
            moveInDate.ParameterName = "@d";
            moveInDate.Value = dataReader.GetDateTime(0);

            updater.CommandText = "update STUDENT_HIST set MOVE_OUT_DATE = DATETIME('now') where (STUDENT_ID = @z and MOVE_IN_DATE = (select MOVE_IN_DATE from STUDENT_HIST where STUDENT_ID = @z ORDER BY MOVE_IN_DATE DESC LIMIT 1))";

            updater.Parameters.Add(moveInDate);
            updater.Parameters.Add(param3);
            updater.ExecuteNonQuery();
        }





        //This command should update the student table to assign them to a room, and assign the first available key for that room.

        //Selects the occupancy and number of students in the room
        IDbCommand checkOccupancy = dbConnection.CreateCommand();
        checkOccupancy.CommandText = "select OCCUPANCY, NUM_STUDENTS from ROOM_PLUS where (BLDG_NUM = @x and ROOM_NUM = @y)";
        checkOccupancy.Parameters.Add(param1);
        checkOccupancy.Parameters.Add(param2);
        checkOccupancy.Prepare();
        IDataReader checkOccupancyReader = checkOccupancy.ExecuteReader();
        checkOccupancyReader.Read();

        //Selects student eligibity 
        IDbCommand checkElig = dbConnection.CreateCommand();
        checkElig.CommandText = "select STUDENT_ELIGIBLE from STUDENT where STUDENT_ID = @z";
        checkElig.Parameters.Add(param3);
        checkElig.Prepare();
        IDataReader checkEligReader = checkElig.ExecuteReader();
        checkEligReader.Read();

        //Checks that the student is eligible and if there is available occupancy in the room
        try
        {
            if (!checkEligReader.GetBoolean(0) || checkOccupancyReader.GetInt32(0) <= checkOccupancyReader.GetInt32(1))
            {
                GameManager.instance.errorPanel.SetActive(true);
            }
            else
            {

                updater.CommandText = "update STUDENT " +
                "set BLDG_NUM = @x, ROOM_NUM = @y, KEY_NUM = (select KEY_NUM from KEYS_AVAIL where BLDG_NUM = @x and ROOM_NUM = @y limit 1) " +
                "where STUDENT_ID = @z";


                updater.Parameters.Add(param1);
                updater.Parameters.Add(param2);
                updater.Parameters.Add(param3);
                updater.Prepare();

                updater.ExecuteNonQuery();

                IDbCommand histAdder = dbConnection.CreateCommand();

                //This adds a row to the student history table. The subquery gets the key number.
                histAdder.CommandText = "insert into STUDENT_HIST (MOVE_IN_DATE, STUDENT_ID, BLDG_NUM, ROOM_NUM, KEY_NUM) VALUES(DATETIME('now'), @z, @x, @y, (select KEY_NUM from STUDENT where STUDENT_ID = @z))";
                histAdder.Parameters.Add(param1);
                histAdder.Parameters.Add(param2);
                histAdder.Parameters.Add(param3);
                histAdder.Prepare();
                histAdder.ExecuteNonQuery();

                UpdateTransactions(dbConnection, "Moved student " + student_id + " to residence " + bldg_num + " room " + room_num);

                //Close connection
                dbConnection.Close();

                PlayerPrefs.SetInt("CounterTotal", PlayerPrefs.GetInt("CounterTotal") + 1);

                CounterManager.instance.UpdatCounter();
            }
        }
        catch (Exception e)
        {

            GameManager.instance.errorPanel.SetActive(true);

        }

    }

    //Used to remove student from residence (not from students database)
    public void removeStudent()
    {
        //string bldg_num = bldg_num_field.text;
        //string room_num = room_num_field.text;
        string student_id = student_id_field;
        Debug.Log("Entered removeStudent()");

        IDbConnection dbConnection = OpenDatabase();

        IDbCommand checkHist = dbConnection.CreateCommand();
        //Get the most recent history for that student
        checkHist.CommandText = "select * from STUDENT_HIST where STUDENT_ID = @z ORDER BY MOVE_IN_DATE DESC LIMIT 1";

        //Create reusable parameters


        var stuNumParam = checkHist.CreateParameter();
        stuNumParam.ParameterName = "@z";
        stuNumParam.Value = student_id;


        checkHist.Parameters.Add(stuNumParam);
        checkHist.Prepare();

        //Debug.Log("Trying to update student ID " + student_id);
        IDataReader dataReader = checkHist.ExecuteReader();

        IDbCommand updater = dbConnection.CreateCommand(); 

        //If the most recent entry in the history table has no move_out date, add a move_out_date
        //The most recent date is found using a heinous subquery. This should be fixed.
        if(dataReader.Read() && dataReader.IsDBNull(1))
        {
            var moveInDate = checkHist.CreateParameter();
            moveInDate.ParameterName = "@d";
            moveInDate.Value = dataReader.GetDateTime(0);

            updater.CommandText = "update STUDENT_HIST set MOVE_OUT_DATE = DATETIME('now') where (STUDENT_ID = @z and MOVE_IN_DATE = (select MOVE_IN_DATE from STUDENT_HIST where STUDENT_ID = @z ORDER BY MOVE_IN_DATE DESC LIMIT 1))";

            updater.Parameters.Add(moveInDate);
            updater.Parameters.Add(stuNumParam); 
            updater.ExecuteNonQuery();
        }






        
        

        updater.CommandText = "update STUDENT " +
        "set BLDG_NUM = null, ROOM_NUM = null, KEY_NUM = null " +
        "where STUDENT_ID = @z";

        updater.Parameters.Add(stuNumParam);
        updater.Prepare();

        updater.ExecuteNonQuery();

        UpdateTransactions(dbConnection, "Removed student " + student_id + " from residence");

        



        //Close connection
        dbConnection.Close();

        PlayerPrefs.SetInt("CounterTotal", PlayerPrefs.GetInt("CounterTotal") + 1);

        CounterManager.instance.UpdatCounter();
        
    }

    //Change Eligibility status
    public void updateElig()
    {
        string student_id = student_id_field;

        Toggle eligToggle = GameManager.instance.eligToggle;
        TMP_InputField eligReason = GameManager.instance.eligReason;

        IDbConnection dbConnection = OpenDatabase();

        IDbCommand eligUpdate = dbConnection.CreateCommand();
        eligUpdate.CommandText = "UPDATE STUDENT SET STUDENT_ELIGIBLE = @e, STUDENT_ELIGREASON = @r where STUDENT_ID = @id";

        var eligParam = eligUpdate.CreateParameter();
        eligParam.ParameterName = "@e";
        eligParam.Value = eligToggle.isOn;

        var reasParam = eligUpdate.CreateParameter();
        reasParam.ParameterName = "@r";
        reasParam.Value = eligReason.text;

        var stuNumParam = eligUpdate.CreateParameter();
        stuNumParam.ParameterName = "@id";
        stuNumParam.Value = student_id;

        eligUpdate.Parameters.Add(eligParam);
        eligUpdate.Parameters.Add(reasParam);
        eligUpdate.Parameters.Add(stuNumParam);
        eligUpdate.Prepare();

        eligUpdate.ExecuteNonQuery();

        UpdateTransactions(dbConnection, "Changed eligibility of student " + student_id + " to " + eligToggle.isOn);

        dbConnection.Close();
        

    }

    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        
        dbConnection.Open();

        return dbConnection;
    }

    private void UpdateTransactions(IDbConnection dbConnection, string logtext)
    {
        IDbCommand logUpdate = dbConnection.CreateCommand();
        logUpdate.CommandText = "INSERT INTO TRANSACTIONS VALUES(DATETIME('now'), @text)";
        var logParam = logUpdate.CreateParameter();
        logParam.ParameterName = "@text";
        logParam.Value = logtext;

        logUpdate.Parameters.Add(logParam);
        logUpdate.Prepare();
        logUpdate.ExecuteNonQuery();
    }
}
