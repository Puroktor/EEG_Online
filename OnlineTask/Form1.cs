using NetManager;
using FoundationLibrary.TransformSignal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineTask
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            double[] data = frame.Data.Select(x => Convert.ToDouble(x)).ToArray();
            alglib.complex[] fur = new alglib.complex[frame.Data.Length];
            Trend.SubtractionTrend(data);
            alglib.fftr1d(data, out fur);
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
