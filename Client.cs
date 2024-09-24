using System.Security.Cryptography;
using System.Text;

public class Client : Core
{
    public Client(string host, int port, Button bServer, Button bClient, Button bSend, ListBox listLog)
        : base(host, port, bServer, bClient, bSend, listLog)
    {

    }



    private protected override bool OpenNext()
    {
        while (true)
        {
            socket.Connect(ipEndPoint);
            if (socket.Connected)
            {
                bClient.Invoke(new Action(() => bClient.BackColor = Color.Green));
                return true;
            }
        }
    }

    private protected override void ConnectSecurity()
    {
        RSACryptoServiceProvider RSA1 = new RSACryptoServiceProvider(1024); //мой ключ
        RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider(2048); //ключ сервера

        while (true)
        {
            Thread.Sleep(50);
            try
            {
                string data = GetData();
                //устанавливаем публичный ключ сервера
                RSA2.FromXmlString(data);
                Log("Key Get:", data);



                //зашифровываем наш публичный ключ, по публичному ключу сервера
                RSAParameters publicKeyParams = RSA1.ExportParameters(false);
                string modulus = Convert.ToBase64String(
                        RSA2.Encrypt(
                            publicKeyParams.Modulus,
                            false
                    ));
                string exponent = Convert.ToBase64String(
                    RSA2.Encrypt(
                            publicKeyParams.Exponent,
                            false
                    ));
                //и отправляем его серверу
                Send(modulus + ":" + exponent);


                break;
            }
            catch { }
        }

        while (true)
        {
            Thread.Sleep(50);
            try
            {
                string data = GetData();
                Log("Key Get:", data);

                //получаем ключ и расшифровываем
                string[] datas = data.Split(':'); 
                aes.Key = RSA1.Decrypt(
                            Convert.FromBase64String(datas[0]),
                            false
                    );
                aes.IV = RSA1.Decrypt(
                            Convert.FromBase64String(datas[1]),
                            false
                    );
                //Log("Get:", data, aes.ToString());
                break;
            }
            catch { }
        }
        Log("key set");
    }
}
