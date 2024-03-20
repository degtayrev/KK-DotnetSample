using KK.Sdk;
using KK.Sdk.Structure;
using Helper;
using System.IO;
using Microsoft.VisualBasic;


namespace kriptaConnection
{

    public class KriptaConnectionHandler
    {
        public APIResponse.Types.SessionInformation? SessionInformation { get; set; }
        public KKConnection? KriptaConnection { get; set; }
        public uint SlotiD = uint.Parse(ConfigReader.GetConfigurationValue("slotId"));
        string host = ConfigReader.GetConfigurationValue("kkHost");
        ushort port = ushort.Parse(ConfigReader.GetConfigurationValue("kkPort"));
        string clientCertificatePath = ConfigReader.GetConfigurationValue("clientCertificatePath");
        string clientCAPath = ConfigReader.GetConfigurationValue("clientCAPath");
        string clientPrivateKeyPath = ConfigReader.GetConfigurationValue("clientPrivateKeyPath");
        string slotPassword = ConfigReader.GetConfigurationValue("slotPassword");
        public string encryptionkey = ConfigReader.GetConfigurationValue("encryptionkey");

        public  KriptaConnectionHandler()
        {
            KriptaConnection = KKConnection.createConnectionWithCertificatePEMFile(host,port,clientCertificatePath,clientPrivateKeyPath,clientCAPath);
            try
            {
                SessionInformation = KriptaConnection.Login(SlotiD,slotPassword);
                Console.WriteLine(KK.Sdk.KKConnection.GetVersion());
                Console.WriteLine(SessionInformation.SessionToken);
            }catch (ClientSDKException e)
            {
                Console.WriteLine("Kripta Key Fault Code : " + e.Message);
            }
        }

        public APIResponse.Types.Encrypt Encrypt (APIRequest.Types.Encrypt encryptionRequest)
        {
            try
            {
                var encryptedData = KriptaConnection.Encrypt(SlotiD,SessionInformation.SessionToken, encryptionkey, null,encryptionRequest);
                return encryptedData;
            }
            catch (ClientSDKException e)
            {
                Console.WriteLine("Kripta Key Fault Code : " + e.Message);
                return null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public string[] Decrypt (APIRequest.Types.Decrypt decryptionRequest)
        {
            try
            {
                var decryptedData = KriptaConnection.Decrypt(SlotiD,SessionInformation.SessionToken,decryptionRequest);
                return decryptedData;
            }
            catch (ClientSDKException e)
            {
                Console.WriteLine("Kripta Key Fault Code : " + e.Message);
                return null;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}