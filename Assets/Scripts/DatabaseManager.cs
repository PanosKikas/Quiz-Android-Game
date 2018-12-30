using Mono.Data.Sqlite;
using System.Data;
using Facebook.Unity;
using UnityEngine;
using System.IO;
using System;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField]
    PlayerStats playerStats;

   // IDbConnection dbconn;

    private string connectionString;
    private string path;
    FacebookManager fbManager;
    const string TableName = "PlayerStats";

    void Start()
    {
        fbManager = GetComponent<FacebookManager>();
        ConnectToDatabase();
        
    }

    void ConnectToDatabase()
    {

        if (Application.platform != RuntimePlatform.Android)
        {
            connectionString = Application.dataPath + "/Stats.db";
        }
        else
        {
            connectionString = Application.persistentDataPath + "/" + "Stats.db";
            if(!File.Exists(connectionString))
            {
                WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "Stats.db");
                while (!loadDB.isDone)
                {
                    
                }
                File.WriteAllBytes(connectionString, loadDB.bytes);
            }
            
        }
        Debug.Log(connectionString);
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + connectionString))
        {

            dbConnection.Open(); //Open connection to the database.

            using (IDbCommand dbcmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "Create Table if not exists PlayerStats (" + "Id	TEXT NOT NULL PRIMARY KEY,"
                + "HighScore INTEGER NOT NULL," + "CorrectQuestions	INTEGER NOT NULL);";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
            }

            dbConnection.Close();
        }
    }

    public void ReadDatabase()
    {
        
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + connectionString))
        {

            dbConnection.Open(); //Open connection to the database.
            string uid = "0";

            if (FB.IsLoggedIn)
                uid = fbManager.GetAccessToken().UserId;

            string sqlQuery = "Select * From PlayerStats Where Id = " + uid;
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = sqlQuery;
            using (IDataReader reader = dbcmd.ExecuteReader())
            {

                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    int highScore = reader.GetInt32(1);
                    int correctAnswered = reader.GetInt32(2);


                    playerStats.HighScore = highScore;
                    playerStats.TotalCorrectQuestionsAnswered = correctAnswered;

                    Debug.Log("id= " + id + "  name =" + highScore + "  correct =" + correctAnswered);

                }

                reader.Close();

            }
            dbcmd.Dispose();
            dbcmd = null;
            dbConnection.Close();
        }
        
    }

    public void SaveToDatabase()
    {
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + connectionString))
        {

            dbConnection.Open(); //Open connection to the database.

            int rowCount = 0;
            string sqlQuery;
            string id;
            if(FB.IsLoggedIn)
            {
                // Save user with fb username
                
                AccessToken token =  fbManager.GetAccessToken();
                sqlQuery = "select count(*) from PlayerStats where Id = " + token.UserId;
                id = token.UserId;
                
            }
            else
            {
                id = "0";
                // Search for anonymous user with id 0
                sqlQuery = "select count(*) from PlayerStats where Id = 0";
            }


            using (IDbCommand dbcmd = dbConnection.CreateCommand())
            {
                dbcmd.CommandText = sqlQuery;
                rowCount = Convert.ToInt32(dbcmd.ExecuteScalar());

                // if entry does not exist create it
                if (rowCount < 1)
                {
                    sqlQuery = "Insert into PlayerStats (Id, HighScore, CorrectQuestions) Values(" +id +"," + playerStats.HighScore
                    + "," + playerStats.TotalCorrectQuestionsAnswered + ");";
                    dbcmd.CommandText = sqlQuery;
                    dbcmd.ExecuteNonQuery();
                }
                else // current entry exists
                {

                    sqlQuery = "Update PlayerStats " +
                        "Set (HighScore, CorrectQuestions) = (" + playerStats.HighScore
                        + "," + playerStats.TotalCorrectQuestionsAnswered + ")"
                        + "WHERE Id = " + id + ";";

                       
                    dbcmd.CommandText = sqlQuery;
                    dbcmd.ExecuteNonQuery();
                }

                dbcmd.Dispose();
            }


            dbConnection.Close();
        }
        
        
    }
}
