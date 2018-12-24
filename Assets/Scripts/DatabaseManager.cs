using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField]
    PlayerStats playerStats;

    IDbConnection dbconn;

    const string TableName = "PlayerStats";

    void Start()
    {
        ConnectToDatabase();
        ReadDatabase();
        
    }

    void ConnectToDatabase()
    {
        string conn = "URI=file:" + Application.dataPath + "/Stats.s3db"; //Path to database.
        
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.

        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "Create Table if not exists PlayerStats (" + "Id	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,"
        + "HighScore INTEGER NOT NULL," + "CorrectQuestions	INTEGER NOT NULL);";

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();
    }

    void ReadDatabase()
    {
        string sqlQuery = "Select * From PlayerStats";
        IDbCommand dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = sqlQuery;

        IDataReader reader = dbcmd.ExecuteReader();
        
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            int highScore = reader.GetInt32(1);
            int correctAnswered = reader.GetInt32(2);


            playerStats.HighScore = highScore;
            playerStats.TotalCorrectQuestionsAnswered = correctAnswered;

            Debug.Log("id= " + id + "  name =" + highScore + "  correct =" + correctAnswered);
                   
        }
        
        reader.Close();
        
        reader = null;
        
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        //dbconn = null;
    }

    public void SaveToDatabase()
    {
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "select count(*) from PlayerStats;";
        dbcmd.CommandText = sqlQuery;

        int rowCount = 0;
        rowCount = Convert.ToInt32(dbcmd.ExecuteScalar());
        
        // If stats does not exist create them
        if (rowCount <= 0)
        {
            sqlQuery = "Insert into PlayerStats (HighScore, CorrectQuestions) Values(" + playerStats.HighScore
            + "," + playerStats.TotalCorrectQuestionsAnswered + ");";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
        }
        else // stats exist
        {
            sqlQuery = "Update PlayerStats Set HighScore = " + playerStats.HighScore 
                + ",CorrectQuestions = " + playerStats.TotalCorrectQuestionsAnswered + ";";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
        }
        
        
    }
}
