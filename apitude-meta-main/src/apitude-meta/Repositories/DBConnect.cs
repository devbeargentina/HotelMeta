using apitude_meta.Models;
using MySql.Data.MySqlClient;

namespace apitude_meta.Helper;

class DBConnect
{
    private MySqlConnection connection;

    //Constructor
    public DBConnect()
    {
        Initialize();
    }

    //Initialize values
    private void Initialize()
    {
        connection = (MySqlConnection)apitude_meta.Environment.GetConnection();
    }

    //open connection to database
    public bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {
            //When handling errors, you can your application's response based 
            //on the error number.
            //The two most common error numbers when connecting are as follows:
            //0: Cannot connect to server.
            //1045: Invalid user name and/or password.
            switch (ex.Number)
            {
                case 0:
                    Console.WriteLine("Cannot connect to server.  Contact administrator");
                    break;

                case 1045:
                    Console.WriteLine("Invalid username/password, please try again");
                    break;
            }
            return false;
        }
    }

    //Close connection
    public bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    //Insert statement
    public void Insert(string query)
    {
        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);

            var count = cmd.ExecuteNonQuery();

            this.CloseConnection();
        }
        else
        {
            throw new Exception("Problem with database connection");
        }
    }

    public async Task<MySqlDataReader> Select(string query)
    {

        if (this.OpenConnection() == true)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlDataReader dataReader = cmd.ExecuteReader();

            return dataReader;
        }
        else
        {
            return null;
        }
    }
}