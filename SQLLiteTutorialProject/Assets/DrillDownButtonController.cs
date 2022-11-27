using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using TMPro;
using System;
using Mono.Data.Sqlite;
using System.Data;

public class DrillDownButtonController : MonoBehaviour
{

    //This "column1" string is used in the UpdateButtons() method found in DrilldownButtonManager.
    //This is mainly so that we can get information about the student number when clicking a student's name.
    public string column1;

    [SerializeField] private TMP_InputField bldg_num_field;
    [SerializeField] private TMP_InputField room_num_field;


    public void OpenDrillDown()
    {
        GameManager.instance.drillDownBackground.SetActive(true);
        GameManager.instance.drillDownPanel.SetActive(true);

        

        string tableName = DrilldownButtonManager.instance.table.name;

        //Debug.Log(DrilldownButtonManager.instance.table.name);

        if(tableName == "Residence Table")
        {
            resDrillDown();
            GameManager.instance.studentUpdater.studentUpdaterParent.SetActive(false);
        }
        else if(tableName == "Students Table")
        {
            stuDrillDown();
            GameManager.instance.studentUpdater.studentUpdaterParent.SetActive(true);
        }
        else if(tableName == "Furniture Table")
        {

        }
        else if(tableName == "Keys Table")
        {
            keyDrillDown();
            GameManager.instance.studentUpdater.studentUpdaterParent.SetActive(false);
        }

        //Debug.Log(column1);

        string text = transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

        /*
        if(text.Equals("John Doe Residence"))
        {

            Debug.Log("SUCCESS");
        }
        else
        {
            Debug.Log("Better luck next time, Mister Falcon");

        }
        */
        


        
        

    }

    //Used for viewing the Room table in the Residence Drill-down view
    public void resDrillDown()
    {
        

        TableUI drillTable = GameManager.instance.drillDownTable;

        //Clear the Drilldown table
        //drillTable.gameObject.SetActive(false);

        TMP_Text title = GameManager.instance.title;
        TMP_Text subtitle = GameManager.instance.subtitle;
        //TMP_Text sublabel1 = GameManager.instance.sublabel1;

        IDbConnection dbConnection = OpenDatabase();

        //Get Residence name from button
        string bldgName = transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

        title.text = bldgName;
        subtitle.text = "Rooms";
        //sublabel1.text = ""; //Hide sublabel for this screen.

        IDbCommand getRooms = dbConnection.CreateCommand();
        getRooms.CommandText = "select BLDG_ABBV, ROOM_NUM, SIZE, NUM_STUDENTS, OCCUPANCY from ROOM_PLUS where BLDG_NAME = @bldg";

        //Assign building number as parameter
        var bldgParam = getRooms.CreateParameter();
        bldgParam.ParameterName = "@bldg";
        bldgParam.Value = bldgName;

        //Prepare parameters
        getRooms.Parameters.Add(bldgParam);
        getRooms.Prepare();

        IDataReader dataReader = getRooms.ExecuteReader();

        int n = 1;

        //Set headings
        drillTable.GetCell(0,0).text = "Room Number";
        drillTable.GetCell(0,1).text = "Size (ft²)";
        drillTable.GetCell(0,2).text = "Occupied/Capacity";
        drillTable.GetCell(0,3).text = "Building";

        while(dataReader.Read())
        {
            drillTable.Rows = n + 1;
            drillTable.transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            drillTable.GetCell(n,0).text = dataReader.GetString(0) + dataReader.GetInt32(1).ToString(); //Room Number (Building abbreviation + num.)
            drillTable.GetCell(n,1).text = dataReader.GetInt32(2).ToString(); //Size (ft^2)
            drillTable.GetCell(n,2).text = dataReader.GetInt32(3).ToString() + "/" + dataReader.GetInt32(4).ToString(); //Occupancy (Occupied/Max)
            drillTable.GetCell(n,3).text = bldgName;
            n++;
        }

        //closing connection
        dbConnection.Close();
        
    }

    public void stuDrillDown()
    {

        //Get shared elements from Game Manager
        TableUI drillTable = GameManager.instance.drillDownTable;
        TMP_Text title = GameManager.instance.title;
        TMP_Text subtitle = GameManager.instance.subtitle;
        //TMP_Text sublabel1 = GameManager.instance.sublabel1;
        Toggle eligToggle = GameManager.instance.eligToggle;
        TMP_InputField eligReason = GameManager.instance.eligReason;

        GameManager.instance.studentUpdater.student_id_field = column1;

        //Clear the Drilldown table
        //drillTable.gameObject.SetActive(false);
        //drillTable.gameObject.SetActive(true);
        
        IDbConnection dbConnection = OpenDatabase();

        //Get student name from button
        string stuName = transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

        //Set header to be the student's name
        title.text = stuName;

        //Set subtitle as Student History
        subtitle.text = "Student History";

        
        

        //Get student history
        IDbCommand getStuHist = dbConnection.CreateCommand();
        getStuHist.CommandText = "select MOVE_IN_DATE, MOVE_OUT_DATE, BLDG_ABBV, ROOM_NUM, KEY_NUM "
         + " from (STUDENT_HIST LEFT JOIN RESIDENCE ON STUDENT_HIST.BLDG_NUM = RESIDENCE.BLDG_NUM) where STUDENT_ID = @id";

        //Assign student number as parameter
        var studentID = getStuHist.CreateParameter();
        studentID.ParameterName = "@id";
        studentID.Value = column1; //When a button was pressed in the Student table, column1 contains the student ID.

        getStuHist.Parameters.Add(studentID);
        getStuHist.Prepare();
        IDataReader dataReader = getStuHist.ExecuteReader();

        int n = 1; //Row number of table to edit

        //Reset table even if no data is read.
        drillTable.Rows = n;
        //drillTable.transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);

        drillTable.GetCell(0,0).text = "Move-in date";
        drillTable.GetCell(0,1).text = "Move-out date";
        drillTable.GetCell(0,2).text = "Room Number";
        drillTable.GetCell(0,3).text = "Key Number";

        

        while(dataReader.Read())
        {
            drillTable.Rows = n + 1;
            drillTable.transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            drillTable.GetCell(n,0).text = dataReader.GetDateTime(0).ToString(); //Move In Date.
            drillTable.GetCell(n,1).text = dataReader.IsDBNull(1) ? "N/A" : dataReader.GetDateTime(1).ToString() ; //Move Out Date - check if null.
            drillTable.GetCell(n,2).text = dataReader.GetString(2) + dataReader.GetInt32(3).ToString(); //Room Number. Need to edit SQL to include abbreviation
            drillTable.GetCell(n,3).text = dataReader.GetInt32(4).ToString(); //Key Number
            n++;
        }

        //Get student eligibility. Can reuse studentID parameter from earlier.
        IDbCommand stuQuery = dbConnection.CreateCommand();
        stuQuery.CommandText = "select STUDENT_ELIGIBLE, STUDENT_ELIGREASON from STUDENT where STUDENT_ID = @id";
        stuQuery.Parameters.Add(studentID);
        stuQuery.Prepare();

        IDataReader eligCheck = stuQuery.ExecuteReader();

        //print the student's eligibility
        eligCheck.Read();

        //sublabel1.text = "Eligible: " + eligCheck.GetBoolean(0).ToString() + "\nReason: " + eligCheck.GetString(1);
        eligToggle.isOn = eligCheck.GetBoolean(0);
        eligReason.text = eligCheck.GetString(1);
        


        dbConnection.Close();



    }

    public void keyDrillDown()
    {
        //Define elements to change
        TableUI drillTable = GameManager.instance.drillDownTable;
        TMP_Text title = GameManager.instance.title;
        TMP_Text subtitle = GameManager.instance.subtitle;
        TMP_Text sublabel1 = GameManager.instance.sublabel1;

        //Open the database
        IDbConnection dbConnection = OpenDatabase();

        //Get keynum from button
        string keyNum = transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text;

        title.text = "Key " + keyNum;
        subtitle.text = "Key History";

        //Get key history
        IDbCommand getKeyHist = dbConnection.CreateCommand();

        //
        getKeyHist.CommandText = "select MOVE_IN_DATE, MOVE_OUT_DATE, BLDG_ABBV, ROOM_NUM, STUDENT_ID "
         + " from KEY_HIST where KEY_NUM = @key";

        var keyNumParam = getKeyHist.CreateParameter();
        keyNumParam.ParameterName = "@key";
        keyNumParam.Value = keyNum;

        getKeyHist.Parameters.Add(keyNumParam);
        getKeyHist.Prepare();
        IDataReader dataReader = getKeyHist.ExecuteReader();

        int n = 1; //Row number of table to edit

        //Reset table even if no data is read.
        drillTable.Rows = n;
        //drillTable.transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);

        drillTable.GetCell(0,0).text = "Move-in date";
        drillTable.GetCell(0,1).text = "Move-out date";
        drillTable.GetCell(0,2).text = "Room Number";
        drillTable.GetCell(0,3).text = "Student Number";

        while(dataReader.Read())
        {
            drillTable.Rows = n + 1;
            drillTable.transform.GetChild(0).GetChild(n).transform.localScale = new Vector3(1f, 1f, 1f);
            drillTable.GetCell(n,0).text = dataReader.GetDateTime(0).ToString(); //Move In Date.
            drillTable.GetCell(n,1).text = dataReader.IsDBNull(1) ? "N/A" : dataReader.GetDateTime(1).ToString() ; //Move Out Date - check if null.
            drillTable.GetCell(n,2).text = dataReader.GetString(2) + dataReader.GetInt32(3).ToString(); //Room Number. Need to edit SQL to include abbreviation
            drillTable.GetCell(n,3).text = dataReader.GetInt32(4).ToString(); //Student Number
            n++;
        }

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

