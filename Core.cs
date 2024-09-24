using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;




public abstract class Core
{
    private protected Aes aes = Aes.Create();
    private string Now => DateTime.Now.ToShortTimeString();
    private protected Socket socket;
    private IPHostEntry ipHost;
    private IPAddress ipAddr;
    private protected IPEndPoint ipEndPoint;
    private protected Thread waitingForMessage;

    private string host;
    private int port;
    private protected Button bServer;
    private protected Button bClient;
    private Button bSend;
    private protected ListBox listLog;
    public Core(string host, int port, Button bServer, Button bClient, Button bSend, ListBox listLog)
    {
        this.host = host;
        this.port = port;
        this.bServer = bServer;
        this.bClient = bClient;
        this.listLog = listLog;
        this.bSend = bSend;
    }
    public void Open()
    {
        ipHost = Dns.Resolve(host);
        for (int i = 0; i < ipHost.AddressList.Length; i++)
        {
            listLog.Invoke(new Action(() =>
            {
                listLog.Items.Add("Попытка: " + (i + 1) + "/" + ipHost.AddressList.Length);
            }));
            ipAddr = ipHost.AddressList[i];
            ipEndPoint = new IPEndPoint(ipAddr, port);


            try
            {
                socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                if (OpenNext())
                {
                    waitingForMessage = new Thread(new ParameterizedThreadStart(GetMessages));
                    waitingForMessage.Start();
                    bSend.Invoke(new Action(() => {
                        bSend.Enabled = true;
                        bSend.BackColor = Color.Transparent;
                    }));
                    return;
                }
            }
            catch (Exception ex)
            {
                listLog.Invoke(new Action(() =>
                {
                    listLog.Items.Add("Error:" + ex.Message);
                }));
                continue;
            }
        }
        bServer.Invoke(new Action(() => {
            bServer.BackColor = Color.Transparent;
            bServer.Enabled = true;
        }));
        bClient.Invoke(new Action(() => {
            bClient.BackColor = Color.Transparent;
            bClient.Enabled = true;
        }));
    }

    private void GetMessages(object? obj)
    {
        ConnectSecurity();
        while (true)
        {
            Thread.Sleep(50);
            try
            {
                string data = GetData();
                Log("Get:", data);
            } catch { }
        }
    }
    private protected string GetData()
    {
        string GetInformation = "";
        byte[] GetBytes = new byte[1024];
        int BytesRec = socket.Receive(GetBytes);
        GetInformation += Encoding.Unicode.GetString(GetBytes, 0, BytesRec);
        return GetInformation;
    }
    private int iSend = 0;
    public void Send(string data)
    {
        int i = iSend++;
        byte[] SendMsg = Encoding.Unicode.GetBytes(data);
        Log("Try send [" + i + "]:", data);
        socket.Send(SendMsg);
        Log("Success send [" + i + "]!");
    }




    private protected void Log(string title, params string[] log)
    {
        listLog.Invoke(new Action(() => {
            listLog.Items.Add(Now + " " + title);
            for (int i = 0; i < log.Length; i++)
            {
                listLog.Items.Add("\t" + log[i]);
            }
        }));
    }





    private protected virtual bool OpenNext(){
        return false;
    }
    private protected virtual void ConnectSecurity()
    {

    }
}
