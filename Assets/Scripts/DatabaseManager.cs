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
        ReadDatabase();
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
                

                string sqlQuery = "Create Table if not exists PlayerStats (" + "Id TEXT NOT NULL PRIMARY KEY,"
                + "HighScore INTEGER NOT NULL,"
                + "PlayerName TEXT NOT NULL,"
                + "CorrectQuestions INTEGER NOT NULL,"
                + "Level INTEGER NOT NULL," + "Experience INTEGER NOT NULL);";
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
                int j = 0;
                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    int highScore = reader.GetInt32(1);
                    string name = reader.GetString(2);
                    int correctAnswered = reader.GetInt32(3);
                    int level = reader.GetInt32(4);
                    int experience = reader.GetInt32(5);

                    playerStats.Name = name;
                    playerStats.HighScore = highScore;
                    playerStats.TotalCorrectQuestionsAnswered = correctAnswered;
                    playerStats.Level = level;
                    playerStats.Experience = experience;

                    Debug.Log("id= " + id + "  name =" + name + "  correct =" + correctAnswered + "High Score= " + highScore
                        + "Level= " + level + "Experience= " + experience + "\n");
                    ++j;
                }

                reader.Close();

                if(j == 0)
                {
                    playerStats.Initialize();
                }

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
            string nameAdded = "Guest";

            if(FB.IsLoggedIn)
            {
                // Save user with fb username
                
                AccessToken token =  fbManager.GetAccessToken();
                sqlQuery = "select count(*) from PlayerStats where Id = " + token.UserId;
                id = token.UserId;
                nameAdded = fbManager.FbName;
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
                    
                    sqlQuery = "Insert into PlayerStats (Id,HighScore,PlayerName,CorrectQuestions,Level,Experience) Values("
                    + id +  "," + playerStats.HighScore + "," + "\'" + nameAdded + "\'"
                    + "," + playerStats.TotalCorrectQuestionsAnswered 
                    + "," + playerStats.Level + "," + playerStats.Experience + ");";
                    dbcmd.CommandText = sqlQuery;
                    dbcmd.ExecuteNonQuery();
                }
                else // current entry exists
                {
                    
                    sqlQuery = "Update PlayerStats " +
                        "Set (HighScore, CorrectQuestions, Level, Experience) = (" + playerStats.HighScore
                        + "," + playerStats.TotalCorrectQuestionsAnswered + "," + playerStats.Level 
                        + "," + playerStats.Experience + ")"
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
