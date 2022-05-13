using FoundationLibrary.TransformSignal;
using NetManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OnlineTask
{
    public partial class Form1 : Form
    {
        List<double>[] channels = new List<double>[4];
        int[] channelsInd = { 2, 6, 13, 17 };
        int window = 24 * 20;
        public Form1()
        {
            InitializeComponent();
            for(int i = 0; i< channels.Length; i++)
            {
                channels[i] = new List<double>(window);
            }
            reseiveClientControl.Client.Reseive += Client_Reseive;
            reseiveClientControl.Client.Error += Client_Error;
        }

        private void Client_Error(object sender, EventMsgArgs e)
        {
            MessageBox.Show(e.Msg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Client_Reseive(object sender, EventClientMsgArgs e)
        {
            Frame frame = new Frame(e.Msg);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    channels[i].Add(Convert.ToDouble(frame.Data[channelsInd[i] * 24 + j]));
                }
            }
            if (channels[0].Count == window)
            {               
                for (int i = 0; i < 4; i++)
                {
                    double[] data = channels[i].ToArray();
                    Trend.SubtractionTrend(data);
                    alglib.fftr1d(data, out alglib.complex[] fur);
                    channels[i].Clear();
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (!reseiveClientControl.Client.IsRunning)
            {
                reseiveClientControl.Client.StartClient();
                button.Text = "Стоп";
            }
            else
            {
                reseiveClientControl.Client.StopClient();
                button.Text = "Старт";
            }
        }
    }
}
