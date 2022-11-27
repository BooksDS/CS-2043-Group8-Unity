using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class SqliteExampleSimple : MonoBehaviour
{

    [SerializeField] private int hitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();
        dbCommandReadValues.CommandText = "SELECT * FROM HitCountTableSimple";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();

        while(dataReader.Read())
        {
            hitCount = dataReader.GetInt32(1);
        }

        dbConnection.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        hitCount++;
        IDbConnection dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand();
        dbCommandInsertValue.CommandText = "INSERT OR REPLACE INTO HitCountTableSimple (id, hits) VALUES (0, " + hitCount + ")";
        dbCommandInsertValue.ExecuteNonQuery();

        dbConnection.Close();
        
    }

    private IDbConnection CreateAndOpenDatabase()
    {
        string dbUri = "URI=file:MyDatabase.sqlite";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();

        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS HitCountTableSimple (id INTEGER PRIMARY KEY, hits INTEGER)";

        dbCommandCreateTable.ExecuteReader();
        return dbConnection;
    }
}
