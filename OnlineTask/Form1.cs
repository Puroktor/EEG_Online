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
        readonly Label[] labels;
        readonly LinkedList<double>[] channels = new LinkedList<double>[4];
        readonly int[] channelsInd = { 2, 6, 13, 17 };
        readonly int sampleRate = 1000;
        readonly int freqFrom = 8;
        readonly int freqTo = 13;
        readonly int window;
        int direction = 0;

        public Form1()
        {
            InitializeComponent();
            labels = new Label[] { labelC3, labelT3, labelC4, labelT4 };
            window = (int)(24 * Math.Ceiling(sampleRate / 24.0));
            for (int i = 0; i < channels.Length; i++)
            {
                channels[i] = new LinkedList<double>();
            }
            InitChart();
            reseiveClientControl.Client.Reseive += Client_Reseive;
            reseiveClientControl.Client.Error += Client_Error;
            System.Timers.Timer timer = new System.Timers.Timer(10);
            timer.Elapsed += Timer_Elapsed; ;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (reseiveClientControl.Client.IsRunning)
            {
                System.Windows.Forms.Cursor.Position = new Point(
                    System.Windows.Forms.Cursor.Position.X + 5 * direction,
                    System.Windows.Forms.Cursor.Position.Y);
            }
        }

        private void InitChart()
        {
            var title = new Title("ЭЭГ")
            {
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            chart.Titles.Add(title);
            chart.Legends.Clear();
            chart.Series.Clear();
            chart.ChartAreas.Clear();

            String[] name = { "C3", "T3", "C4", "T4" };
            for (int i = 0; i < 4; i++)
            {
                ChartArea chartArea = new ChartArea(name[i])
                {
                    Position = new ElementPosition(0, i * 23 + 5, 100, 24)
                };
                chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartArea.AxisX.ScaleView.Zoomable = true;
                chartArea.AxisY.Minimum = short.MinValue;
                chartArea.AxisY.Maximum = short.MaxValue;
                chartArea.AxisX.Minimum = 0;
                chartArea.AxisX.Maximum = window;
                chartArea.CursorX.AutoScroll = true;
                chartArea.CursorX.IsUserSelectionEnabled = true;
                chart.ChartAreas.Add(chartArea);
                chart.Legends.Add(new Legend(name[i]) { DockedToChartArea = name[i], Docking = Docking.Right });
                chart.Series.Add(new Series() { ChartArea = name[i], Legend = name[i], LegendText = name[i] });
                chart.Series[i].ChartType = SeriesChartType.FastLine;
            }
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
                }
                while (channels[i].Count > window)
                {
                    channels[i].RemoveFirst();
                }
                chart.Series[i].Points.DataBindY(channels[i]);
            }
            if (channels[0].Count == window)
            {
                double[] sum = new double[4];
                for (int i = 0; i < 4; i++)
                {
                    double[] data = channels[i].ToArray();
                    Trend.SubtractionTrend(data);
                    alglib.fftr1d(data, out alglib.complex[] fur);
                    int from = freqFrom * window / sampleRate;
                    int to = freqTo * window / sampleRate;
                    for (int j = from; j <= to; j++)
                    {
                        sum[i] += fur[j].x * fur[j].x + fur[j].y * fur[j].y;
                    }
                    labels[i].Text = sum[i].ToString("0.##");
                }
                double leftAvg = (sum[0] + sum[1]) / 2;
                double rightAvg = (sum[2] + sum[3]) / 2;
                double k = leftAvg / rightAvg;

                if (k > 1.5)
                {
                    direction = -1;
                    labelDir.Text = "←";
                }
                else if (k < 0.5)
                {
                    direction = 1;
                    labelDir.Text = "→";
                }
                else
                {
                    direction = 0;
                    labelDir.Text = "-";
                }
                labelL.Text = leftAvg.ToString("0.##");
                labelR.Text = rightAvg.ToString("0.##");
                labelK.Text = k.ToString("0.##");
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (!reseiveClientControl.Client.IsRunning)
            {
                reseiveClientControl.Client.StartClient();
                reseiveClientControl.Enabled = false;
                button.Text = "Стоп";
            }
            else
            {
                reseiveClientControl.Client.StopClient();
                reseiveClientControl.Enabled = true;
                button.Text = "Старт";
            }
        }
    }
}
