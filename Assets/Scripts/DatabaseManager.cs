using Mono.Data.Sqlite;
using System.Data;
using Facebook.Unity;
using UnityEngine;
using System.IO;
using System;

// A function that handles the creation of the local database
// as well as the read, write 
public class DatabaseManager : MonoBehaviour
{
    // Reference to the player stats
    [SerializeField]
    PlayerStats playerStats;

    
    private string connectionString;
    // A reference to the facebook manager
    FacebookManager fbManager;
    // Name of the table to be created
    const string TableName = "PlayerStats";

    // indicates if currently reading database
    public bool readingDB = false;

    void Start()
    {
        fbManager = GetComponent<FacebookManager>();
        ConnectToDatabase();
        ReadDatabase();
    }

    // A function that connects to the database (if not exists creates one)
    // and (if not exists) creates the table PlayerStats
    void ConnectToDatabase()
    {
        // different path of db for pc/android
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
        
        // open a connection with the database
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + connectionString))
        {

            dbConnection.Open(); //Open connection to the database.
            // create table PlayerStats if not exists
            using (IDbCommand dbcmd = dbConnection.CreateCommand())
            {
                

                string sqlQuery = "Create Table if not exists PlayerStats (" + "Id TEXT NOT NULL PRIMARY KEY,"
                + "HighScore INTEGER NOT NULL,"
                + "PlayerName TEXT NOT NULL,"
                + "CorrectQuestions INTEGER NOT NULL,"
                + "Level INTEGER NOT NULL," + "Experience INTEGER NOT NULL,"
                + "HighestStreak INTEGER NOT NULL);";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }

    // A function that reads the local database for the current user and updates the playerStats
    public void ReadDatabase()
    {
        readingDB = true;
        using (IDbConnection dbConnection = new SqliteConnection("URI=file:" + connectionString))
        {
            dbConnection.Open(); //Open connection to the database.
            string uid = "0"; // the primaryKey (id) of the user - 0 when user is anonymous (no fb login)
            // if logged in get his access facebook token and use it as id
            if (FB.IsLoggedIn)
                uid = fbManager.GetAccessToken().UserId;
            // get all columns for user with this id
            string sqlQuery = "Select * From PlayerStats Where Id = " + uid;
            IDbCommand dbcmd = dbConnection.CreateCommand();
            dbcmd.CommandText = sqlQuery;

            using (IDataReader reader = dbcmd.ExecuteReader())
            {
                int j = 0;
                // Get the contents of each column
                while (reader.Read())
                {
                    
                    string id = reader.GetString(0);
                    int highScore = reader.GetInt32(1);
                    string name = reader.GetString(2);
                    int correctAnswered = reader.GetInt32(3);
                    int level = reader.GetInt32(4);
                    int experience = reader.GetInt32(5);
                    int highestStreak = reader.GetInt32(6);
                    // Update playerStats from what is read acoordingly
                    playerStats.Name = name;
                    playerStats.HighScore = highScore;
                    playerStats.TotalCorrectQuestionsAnswered = correctAnswered;
                    playerStats.Level = level;
                    playerStats.Experience = experience;
                    playerStats.HighestStreak = highestStreak;
                                      
                    ++j;
                }

                reader.Close();
                // No entry has been found in the db for user with this id
                // -> intialize player stats and then create entry for this player
                if(j == 0)
                {
                    playerStats.Initialize();                 
                    SaveToDatabase();                 
                }
            }
            
            dbcmd.Dispose();
            dbcmd = null;
            dbConnection.Close();
        }

        readingDB = false;
        
    }



    // A function that updates the columns of the database for the given player
    // If no player exists with this id then creates a new row 
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
                    
                    sqlQuery = "Insert into PlayerStats (Id,HighScore,PlayerName,CorrectQuestions,Level,Experience,HighestStreak) Values("
                    + id +  "," + playerStats.HighScore + "," + "\'" + nameAdded + "\'"
                    + "," + playerStats.TotalCorrectQuestionsAnswered 
                    + "," + playerStats.Level + "," + playerStats.Experience + ","
                    + playerStats.HighestStreak + ");";
                    dbcmd.CommandText = sqlQuery;
                    dbcmd.ExecuteNonQuery();
                    ReadDatabase();
                }
                else // current entry exists
                {
                    // Update columns
                    sqlQuery = "Update PlayerStats " +
                        "Set (HighScore, CorrectQuestions, Level, Experience, HighestStreak) = (" + playerStats.HighScore
                        + "," + playerStats.TotalCorrectQuestionsAnswered + "," + playerStats.Level 
                        + "," + playerStats.Experience + "," + playerStats.HighestStreak + ")"
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
