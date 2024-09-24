using System.Net.Sockets;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Text;
public class Server : Core
{

    public Server(string host, int port, Button bServer, Button bClient, Button bSend, ListBox listLog)
        : base(host, port, bServer, bClient, bSend, listLog)
    {

    }


    private protected override bool OpenNext()
    {
        try
        {
            socket.Bind(ipEndPoint);
            socket.Listen(10);
            while (true)
            {
                Socket A = socket.Accept();
                if (A.Connected)
                {
                    bServer.Invoke(new Action(() => bServer.BackColor = Color.Green));
                    socket = A;
                    return true;
                }
            }
        }
        catch (Exception e)
        {

            listLog.Invoke(new Action(() =>
            {
                listLog.Items.Add("Error:" + e.Message);
            }));
            return false;
        }
    }

    private protected override void ConnectSecurity()
    {
        RSACryptoServiceProvider RSA1 = new RSACryptoServiceProvider(2048); //мой ключ
        RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider(1024); //ключ клиента
        string publickey = RSA1.ToXmlString(false); //получим открытый ключ
        string privatekey = RSA1.ToXmlString(true); //получим закрытый ключ

        //отправляем наш публичный ключ
        Send(publickey);

        while (true)
        {
            Thread.Sleep(50);
            try
            {
                string data = GetData();
                Log("Key Get:", data);

                //расшифровываем публичный ключ клиента
               string[] datas = data.Split(":");
                RSAParameters publicKeyParams = new RSAParameters();
                publicKeyParams.Modulus = RSA1.Decrypt(Convert.FromBase64String(datas[0]), false);
                publicKeyParams.Exponent = RSA1.Decrypt(Convert.FromBase64String(datas[1]), false);
                RSA2.ImportParameters(publicKeyParams);

               //, RSA2.ToXmlString(false));

                //отправляем симетричный ключ, зашифрованный по публичному ключу клиента
                aes.GenerateKey();
                aes.GenerateIV();
                string key = Convert.ToBase64String(
                        RSA2.Encrypt(aes.Key, false)
                        ) + 
                    ":" +
                    Convert.ToBase64String(
                        RSA2.Encrypt(aes.IV, false)
                        );
                Send(key);
                break;
            }
            catch { }
        }
        Log("key set");
    }
}
