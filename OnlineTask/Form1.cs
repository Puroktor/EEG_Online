using FoundationLibrary.TransformSignal;
using NetManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace OnlineTask
{
    public partial class Form1 : Form
    {
        Chart[] charts;
        LinkedList<double>[] channels = new LinkedList<double>[4];
        int[] channelsInd = { 2, 6, 13, 17 };
        int sampleRate = 1000;
        int window;
        int direction = 0;

        public Form1()
        {
            InitializeComponent();
            window = (int)(24 * Math.Ceiling(sampleRate / 24.0));
            for (int i = 0; i < channels.Length; i++)
            {
                channels[i] = new LinkedList<double>();
            }
            charts = new Chart[] { chart1, chart2, chart3, chart4 };
            String[] titles = { "C3", "T3", "C4", "T4" };
            for (int i = 0; i < charts.Length; i++)
            {
                var title = new Title(titles[i])
                {
                    Font = new Font("Arial", 12, FontStyle.Bold)
                };
                charts[i].Titles.Add(title);
                charts[i].Legends.Clear();

                var graph = charts[i].Series[0];
                graph.ChartType = SeriesChartType.FastLine;
                graph.Points.AddXY(0, 0);

                var area = charts[i].ChartAreas[0];
                area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                area.AxisX.ScaleView.Zoomable = true;
                area.AxisX.Maximum = window / 24;
                area.CursorX.AutoScroll = true;
                area.CursorX.IsUserSelectionEnabled = true;
            }
            reseiveClientControl.Client.Reseive += Client_Reseive;
            reseiveClientControl.Client.Error += Client_Error;
            timer.Interval = 1;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Position = new Point(
                System.Windows.Forms.Cursor.Position.X + 2 * direction,
                System.Windows.Forms.Cursor.Position.Y);
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
                    double y = Convert.ToDouble(frame.Data[channelsInd[i] * 24 + j]);
                    channels[i].AddLast(y);
                    Invoke(new Action(() => charts[i].Series[0].Points.AddY(y)));
                }
                while (channels[i].Count > window)
                {
                    channels[i].RemoveFirst();
                }
                if (charts[i].Series[0].Points.Count == window / 24)
                {
                    Invoke(new Action(() => charts[i].Series[0].Points.Clear()));
                }
                
            }
            if (channels[0].Count == window)
            {
                for (int i = 0; i < 4; i++)
                {
                    double[] data = channels[i].ToArray();
                    Trend.SubtractionTrend(data);
                    alglib.fftr1d(data, out alglib.complex[] fur);
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
