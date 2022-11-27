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




public class PasswordChangeManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField oldPasswordField;
    [SerializeField] private TMP_InputField newPasswordField;
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
    public void ChangePassword()
    {



       //This part gets the current password (or the hash) from the database.



       //Open connection to database
        IDbConnection dbConnection = OpenDatabase();
        //creating command to leverage against server
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        //Command to get password from database
        dbCommandReadValues.CommandText = "SELECT PASS_HASH FROM LOGIN";
        //set new dataReader object as result of executed command
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();






        //hard coded cell data. reading info out of datareader as strings.
        string oldPassword = oldPasswordField.text;
        string newPassword = newPasswordField.text;



       
        



       //Get the first (and only) row of data
        dataReader.Read();



       //So these should hash the passwords
        string oldPasswordHash = hashPassword(oldPassword);    
        string newPasswordHash = hashPassword(newPassword);




        //String comparison. to whats found in the database. Since we only have 1 login we can just use a single 'if'
        if (oldPasswordHash.Equals(dataReader.GetString(0)))
        {
            //Creates an SQL command to update the password.
            IDbCommand passWriter = dbConnection.CreateCommand();
                                  
            
            //Creates the SQL statement to insert the password. Note that x will be replaced later.
            passWriter.CommandText = "update LOGIN SET PASS_HASH = @x WHERE USER_NAME = 'admin'";



           //Creates a parameter to replace @x with the new password hashed.
            var parameter = passWriter.CreateParameter();
            parameter.ParameterName = "@x";
            parameter.Value = newPasswordHash;



           //These two lines replace @x with the string containing the new password
            passWriter.Parameters.Add(parameter);
            passWriter.Prepare();



           //Since updates are not queries, we use ExecuteNonQuery()
            passWriter.ExecuteNonQuery();
            Debug.Log("successfully changed password");



           //Always make sure to close the connection when you're done with it
            dbConnection.Close();
            ReturnToMainPanel();



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
        text.SetActive(false);
    }



   public void ReturnToMainPanel()
    {
        mainPanel.SetActive(true);
        passwordChangePanel.SetActive(false);
        
    }



   //This creates the connection to the SQLite database, which is stored in a file in the project folder.
    private IDbConnection OpenDatabase()
    {
        string dbUri = "URI=file:Housing_Database.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();



       return dbConnection;
    }
}