using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.TableUI;
using System;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject passwordChangePanel;
    [SerializeField] private GameObject text;


    //method that hashes the password
    string hashPassword(string password)
    {
        var sha = SHA256.Create();
        var byteArray = Encoding.Default.GetBytes(password);
        var hash = sha.ComputeHash(byteArray);
        return Convert.ToBase64String(hash);
   }    
 
    //THIS SCRIPT will need to have the login table created for the user.
    public void Login()
    {
        //opening database connection
        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //assigning command
        dbCommandReadValues.CommandText = "SELECT * FROM LOGIN";
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        

        
        //hard coded cell data. reading info out of datareader as strings.
        string username = usernameField.text;
        string password = passwordField.text;

        //Converts password to hashed version
        string passHash = hashPassword(password);



        //Reads one row of data from the database. Only need to do this once as there is only 1 account
        dataReader.Read(); 

        //string comparison. to whats found in the database. Since we only have 1 login we can just use a single 'if'
        //Username is stored at 0, password is stored at 1.

        //

        if(username.Equals(dataReader.GetString(0)) && passHash.Equals(dataReader.GetString(1)))
        {
            //load main scene. index 1
            dbConnection.Close();
            SceneManager.LoadScene(1);

        }
        else
        {

            text.SetActive(true);
            Invoke("CloseText", 3f);

        }

        dbConnection.Close();

    }

    public void CloseText()
    {
        if(text != null)
        {
            text.SetActive(false);
        }
    }

    public void OpenPasswordChangePanel()
    {
        passwordChangePanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        return dbConnection;
    }
}
