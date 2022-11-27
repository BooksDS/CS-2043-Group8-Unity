using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class AddStudentController : MonoBehaviour
{
    [SerializeField] private TMP_InputField studentNameInput;
    [SerializeField] private TMP_InputField studentIDInput;

    public void AddStudent()
    {
        string studentName = studentNameInput.text;
        string studentID = studentIDInput.text;

        IDbConnection dbConnection = OpenDatabase();
        
        IDbCommand addStu = dbConnection.CreateCommand();
        addStu.CommandText = "INSERT INTO STUDENT (STUDENT_ID, STUDENT_NAME, STUDENT_ELIGIBLE, STUDENT_ELIGREASON) VALUES(@id, @name, true, 'N/A')";

        var stuNumParam = addStu.CreateParameter();
        stuNumParam.ParameterName = "@id";
        stuNumParam.Value = studentID;

        var nameParam = addStu.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = studentName;

        addStu.Parameters.Add(stuNumParam);
        addStu.Parameters.Add(nameParam);
        addStu.Prepare();

        addStu.ExecuteNonQuery();

        UpdateTransactions(dbConnection, "Created new student " + studentName + " with student ID " + studentID);

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
