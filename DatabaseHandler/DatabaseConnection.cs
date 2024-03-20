using Microsoft.Data.Sqlite;
using DatabaseModel;
using KK.Sdk.Structure;
using KK.Sdk;
using Helper;
using kriptaConnection;
namespace DatabaseHelper
{
    public class DatabaseConnection
    {
        public string DatabaseName = "Data Source=Kripta.db";
        public DatabaseConnection()
        {
            using var connection = new SqliteConnection(DatabaseName);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS UserInformation (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        Name TEXT NOT NULL,
                        Address TEXT NOT NULL,
                        Phonenumber TEXT NOT NULL
                    );";
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void AddData(string tableName, List<string> data)
        {
            using var connection = new SqliteConnection(DatabaseName);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO {tableName} (Name, Address, Phonenumber) VALUES (@Name, @Address, @Phonenumber);";
            command.Parameters.AddWithValue("@Name", tableName[0]);
            command.Parameters.AddWithValue("@Address", tableName[1]);
            command.Parameters.AddWithValue("@Phonenumber", tableName[2]);


            command.ExecuteNonQuery();
            connection.Close();
        }

        public List<CustomerInfo> GetData(string tableName)
        {
            using var connection = new SqliteConnection(DatabaseName);
            List<CustomerInfo> customerInfos = new List<CustomerInfo>();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName};";

            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                var customerInfo = new CustomerInfo(){
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Phonenumber"))
                };
                customerInfos.Add(customerInfo);
            }
            return customerInfos;
        }

        public void AddEncryptedData(string tableName, List<string> data)
        {
            kriptaConnection.KriptaConnectionHandler kkConnection = new kriptaConnection.KriptaConnectionHandler();
            APIRequest.Types.Encrypt encryptionRequest = new();
            encryptionRequest.Plaintext.Add(new APIRequest.Types.SingleEncrypt
                        {
                            Aad = "TEST1",
                            Plaintext = data[0]
                        });
            encryptionRequest.Plaintext.Add(new APIRequest.Types.SingleEncrypt
                        {
                            Aad = "TEST1",
                            Plaintext = data[1]
                        });
            encryptionRequest.Plaintext.Add(new APIRequest.Types.SingleEncrypt
                        {
                            Aad = "TEST1",
                            Plaintext = data[2]
                        });
            
            using var connection = new SqliteConnection(DatabaseName);
            connection.Open();
            try 
            {
                var encryptedData = kkConnection.Encrypt(encryptionRequest);
                
                var command = connection.CreateCommand();
                command.CommandText = $"INSERT INTO {tableName} (Name, Address, Phonenumber) VALUES (@Name, @Address, @Phonenumber);";
                command.Parameters.AddWithValue("@Name",encryptedData.KeyVersion + "|" + encryptedData.Ciphertext[0].Ciphertext + "|" + encryptedData.Ciphertext[0].Mac + "|" + encryptedData.Ciphertext[0].Iv);
                command.Parameters.AddWithValue("@Address",encryptedData.KeyVersion + "|" + encryptedData.Ciphertext[1].Ciphertext + "|" + encryptedData.Ciphertext[1].Mac + "|" + encryptedData.Ciphertext[1].Iv);
                command.Parameters.AddWithValue("@Phonenumber",encryptedData.KeyVersion + "|" + encryptedData.Ciphertext[2].Ciphertext + "|" + encryptedData.Ciphertext[2].Mac + "|" + encryptedData.Ciphertext[2].Iv);
                command.ExecuteNonQuery();

            }
            catch (ClientSDKException e)
            {
                Console.WriteLine("Cannot Encrypt , ERROR : ",e);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR : ",e);
            }
            connection.Close();
        }

        public Task<List<CustomerInfo>> getDecryptedData(string tableName, KriptaConnectionHandler kriptaConnectionHandler)
        {
            using var connection = new SqliteConnection(DatabaseName);
            List<CustomerInfo> customerInfos = new List<CustomerInfo>();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName};";
            using var reader = command.ExecuteReader();
            
            if (!reader.HasRows)
                return Task.FromResult(customerInfos);
            while (reader.Read())
            {
                var customerInfo = new CustomerInfo(){
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Phonenumber"))
                };
                customerInfos.Add(customerInfo);
            }
            connection.Close();
            Console.WriteLine("ASDasdasdASD");
            List<CustomerInfo> decryptedCustomerInfos = new List<CustomerInfo>();
            
            try
            {
                foreach (var iter in customerInfos)
                {
                    string[] dataArray = iter.Name.Split("|");
                    string[] dataArray2 = iter.Address.Split("|");
                    string[] dataArray3 = iter.PhoneNumber.Split("|");
                    Console.WriteLine(dataArray[0]);
                    APIRequest.Types.Decrypt decryptionRequest = new();
                    decryptionRequest.Ciphertext.Add(new APIRequest.Types.SingleDecrypt
                    {
                        Aad = "TEST1",
                        KeyVersion = uint.Parse(dataArray[0]),
                        Ciphertext = dataArray[1].ToString(),
                        Mac = dataArray[2].ToString(),
                        Iv = dataArray[3].ToString(),
                        KeyID = ConfigReader.GetConfigurationValue("encryptionkey")
                    });
                    decryptionRequest.Ciphertext.Add(new APIRequest.Types.SingleDecrypt
                    {
                        Aad = "TEST1",
                        KeyVersion = uint.Parse(dataArray2[0]),
                        Ciphertext = dataArray2[1].ToString(),
                        Mac = dataArray2[2].ToString(),
                        Iv = dataArray2[3].ToString(),
                        KeyID = ConfigReader.GetConfigurationValue("encryptionkey")
                    });
                    decryptionRequest.Ciphertext.Add(new APIRequest.Types.SingleDecrypt
                    {
                        Aad = "TEST1",
                        KeyVersion = uint.Parse(dataArray3[0]),
                        Ciphertext = dataArray3[1].ToString(),
                        Mac = dataArray3[2].ToString(),
                        Iv = dataArray3[3].ToString(),
                        KeyID = ConfigReader.GetConfigurationValue("encryptionkey")
                    });
                    var decryptedData = kriptaConnectionHandler.Decrypt(decryptionRequest);
                    CustomerInfo customerInfo = new CustomerInfo()
                    {
                        Name = decryptedData[0],
                        Address = decryptedData[1],
                        PhoneNumber = decryptedData[2]
                    };
                    decryptedCustomerInfos.Add(customerInfo);   
                } 
                return Task.FromResult(decryptedCustomerInfos);
            }
                catch(Exception e)
                {
                    Console.WriteLine("Error = " + e);
                    Environment.Exit(1);
                    return Task.FromResult(new List<CustomerInfo>());
                }
        }
    }
}