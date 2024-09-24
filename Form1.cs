public partial class Form1 : Form
{
    Core core;
    Thread socketThread;
    public Form1()
    {
        InitializeComponent();
    }
    private void bServer_Click(object sender, EventArgs e)
    {
        bClient.BackColor = Color.Red;
        bServer.BackColor = Color.Orange;
        core = new Server(iIP.Text, (int)iPort.Value, bServer, bClient, bSend, listLog);
        Open();
    }

    private void bClient_Click(object sender, EventArgs e)
    {
        bClient.BackColor = Color.Orange;
        bServer.BackColor = Color.Red;
        core = new Client(iIP.Text, (int)iPort.Value, bServer, bClient, bSend, listLog);
        Open();
    }
    private void Open()
    {
        bClient.Enabled = false;
        bServer.Enabled = false;
        socketThread = new Thread(new ThreadStart(core.Open));
        socketThread.IsBackground = true;
        socketThread.Start();
    }


    private void bSend_Click(object sender, EventArgs e)
    {
        if (iMSG.Text.Length > 0)
        {
            core.Send(iMSG.Text);
        }
    }
}