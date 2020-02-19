using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Drawing.Imaging;

namespace CanOfDuty
{
    public partial class Form1 : Form
    {
        /*                   Radar Graphics               */
        Timer t = new Timer();
        int WIDTH = 207, HEIGHT = 207, HAND = 103;
        int u, count=0;  //in degree
        int cx, cy;     //center of the circle
        int x, y;       //HAND coordinate
        int tx, ty, lim = 25;
        Bitmap bmp;
        Pen p;
        Graphics g;
        /*                   Radar Graphics               */


//////////////////////////////////////////////////////////////////////////////////////////////////////////
        
            
            /*           Obtained data first stored in strings           */
        string a = "empty,";
        string komandaid = "9271.0";
        string workingt = "100";
        string numbpackets = "1100";
        string veloc = "100";
        string cen = "100";
        string cuz = "100";
        string telheight = "100";
        string cekilmehadise = "100";
        string b = "empty1";
        string csv_text = "Real_time,ID,Paket_sayi,Isleme_vaxti,Hundurluk,Suret,Cografi_en,Cografi_uzunluq,Cekilme_hadisesi,\n";
        string image_text = "";
        string image_start = "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDABsSFBcUERsXFhceHBsgKEIrKCUlKFE6PTBCYFVlZF9VXVtqeJmBanGQc1tdhbWGkJ6jq62rZ4C8ybqmx5moq6T/2wBDARweHigjKE4rK06kbl1upKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKSkpKT/wAARCAHgAeADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPw";
        List<List<string>> datas;
        /*           Obtained data first stored in strings           */


//////////////////////////////////////////////////////////////////////////////////////////////////////////


        /*           Strings are converted to floats           */
        float idnumber = 0;
        float worktime = 0;
        float numofpackets = 0;
        float velofcan = 0;
        float longtitude = 0;
        float latitude = 0;
        float canheight = 0;
        float cekilmesi = 1;
        /*           Strings are converted to floats           */



//////////////////////////////////////////////////////////////////////////////////////////////////////////

        string q1, q2, q3, q4, q5, q6, q7; //substrings
        static SerialPort _serialPort;// Global variable serial port
        int rownumb = 0; //global variable row number of table

        public Form1()
        {
            InitializeComponent();
            initViewsAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            var ports = SerialPort.GetPortNames();
            comboBox1.DataSource = ports;
            label28.Text = DateTime.Now.ToShortDateString(); //30.5.2012
            dataviewinit();
            chartview_init();
            datas = new List<List<string>>();
            bmp = new Bitmap(WIDTH + 1, HEIGHT + 1);
            File.WriteAllText("myFile.txt", String.Empty);
            using (StreamWriter w = File.AppendText("myFile.txt"))
            {
                w.Write(image_start);
            }


            //background color
            this.BackColor = Color.Black;

            //center
            cx = WIDTH / 2;
            cy = HEIGHT / 2;

            //initial degree of HAND
            u = 0;

            
            //timer
            t.Interval = 5; //in millisecond
            t.Tick += new EventHandler(this.t_Tick);
            t.Start();
            
            




        }

        
        private async void initViewsAsync()
        {
            button5.Click += new System.EventHandler(initProcess);
            
        }


        private void initProcess(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(String.Format("You selected port '{0}'", comboBox1.SelectedItem));
                _serialPort = new SerialPort(comboBox1.SelectedItem.ToString());//Set your board COM comboBox1.SelectedItem.ToString();
                _serialPort.BaudRate = 57600;//int.Parse(comboBox1.SelectedItem.ToString());
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;
                //_serialPort.ReadTimeout = 500;
                _serialPort.Open();
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            }
            catch
            {
                MessageBox.Show("Port Açılmır");
                _serialPort = null;
            }

        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //System.Threading.Thread.Sleep(80);
            SerialPort sp = (SerialPort)sender;

            try
            {
                a = sp.ReadLine();
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            try
            {
                // if it is telemetry
                if (a.Contains("%9271") && a.IndexOf("#") != -1)
                {

                    // getting substrings from coming packets

                    komandaid = a.Substring(1, a.IndexOf(",") - 1);
                    q1 = a.Substring(a.IndexOf(",") + 1);//a.IndexOf(","), a.Length);
                    workingt = q1.Substring(0, q1.IndexOf(","));
                    q2 = q1.Substring(q1.IndexOf(",") + 1);//a.IndexOf(","), a.Length);
                    numbpackets = q2.Substring(0, q2.IndexOf(","));
                    q3 = q2.Substring(q2.IndexOf(",") + 1);
                    telheight = q3.Substring(0, q3.IndexOf(","));
                    q4 = q3.Substring(q3.IndexOf(",") + 1);
                    veloc = q4.Substring(0, q4.IndexOf(","));
                    q5 = q4.Substring(q4.IndexOf(",") + 1);
                    cen = q5.Substring(0, q5.IndexOf(","));
                    q6 = q5.Substring(q5.IndexOf(",") + 1);
                    cuz = q6.Substring(0, q6.IndexOf(","));
                    q7 = q6.Substring(q6.IndexOf(",") + 1);
                    cekilmehadise = q7.Substring(0, q7.IndexOf(","));

                    idnumber = float.Parse(komandaid, System.Globalization.CultureInfo.InvariantCulture);
                    worktime = float.Parse(workingt, System.Globalization.CultureInfo.InvariantCulture);
                    numofpackets = float.Parse(numbpackets, System.Globalization.CultureInfo.InvariantCulture);
                    canheight = float.Parse(telheight, System.Globalization.CultureInfo.InvariantCulture);
                    velofcan = float.Parse(veloc, System.Globalization.CultureInfo.InvariantCulture);
                    longtitude = float.Parse(cuz, System.Globalization.CultureInfo.InvariantCulture);
                    latitude = float.Parse(cen, System.Globalization.CultureInfo.InvariantCulture);
                    cekilmesi = float.Parse(cekilmehadise, System.Globalization.CultureInfo.InvariantCulture);

                    float coeficient = (float)0.7;
                    float hundfloat = coeficient * canheight;
                    int hundurbar = (int)hundfloat;
                    float sonsan = float.Parse(telheight, System.Globalization.CultureInfo.InvariantCulture) / float.Parse(veloc, System.Globalization.CultureInfo.InvariantCulture);
                    this.Invoke((MethodInvoker)delegate
                    {
                        label8.Text = "Log: " + b;
                        label23.Text = "Hündürlük: " + telheight + " metr";
                        label24.Text = "Sürət: " + veloc + " m/san";
                        label25.Text = "Koordinatlar: " + cen + " " + cuz;
                        label10.Text = "Koordinatlar: " + cen + " " + cuz;
                        label26.Text = "Enişə son: " + sonsan.ToString();
                        aGauge1.Value = float.Parse(veloc, System.Globalization.CultureInfo.InvariantCulture);

                        hundbar.Location = new Point(620, 434 - hundurbar);
                        hundbar.Size = new Size(10, hundurbar);
                        //pictureBox2.Location = new Point(242, 80);
                    });


                    a = a.Replace("#", "\n");
                    a = a.Replace("%", "");

                    string timefortemp = DateTime.Now.ToString("HH:mm:ss");
                    csv_text = csv_text+ timefortemp+ "," + a;

                    // inserting floats to the list
                    datas.Add(new List<string> { komandaid, workingt, numbpackets, telheight, veloc, cuz, cen, cekilmehadise});

                    //
                    int sizeofData = datas.Count;
                    this.chart1.Invoke(new Action(() =>
                    {

                        chart1.Series["Series1"].Points.AddXY(worktime, velofcan);
                    }

                    ));

                    this.chart2.Invoke(new Action(() =>
                    {
                        chart2.Series["Series1"].Points.AddXY(worktime, canheight);
                    }

                    ));


                    //inserting values to the table
                    this.dataGridView1.Invoke(new Action(() =>
                    {
                        DataGridViewRow DGrow = (DataGridViewRow)dataGridView1.Rows[rownumb].Clone();
                        DGrow.DefaultCellStyle.BackColor = Color.Black;
                        int celli = 0;
                        List<string> subList = datas[rownumb];
                        foreach (string item in subList)
                        {
                            DGrow.Cells[celli].Value = item;
                            celli++;
                        }
                        celli = 0;
                        dataGridView1.Rows.Add(DGrow);
                        DGrow.DefaultCellStyle.BackColor = Color.Black;
                        rownumb++;
                    }));
                }
                else
                {
                    if (a.Contains("duty"))
                    {
                        
                        setimagetobitmap();
                        File.WriteAllText("myFile.txt", String.Empty);
                        using (StreamWriter w = File.AppendText("myFile.txt"))
                        {
                            w.Write(image_start);
                        }
                    }
                    else
                    {
                        if (a.Contains("separated"))
                        {

                            this.Invoke((MethodInvoker)delegate
                            {
                                pictureBox31.Location = new Point(732, 224);
                                pictureBox41.Location = new Point(770, 319);
                            });
                        }
                        else
                        {
                            if (a.Contains("ready"))
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    label22.Text = "Status: Ready";
                                });
                            }

                            else
                            {
                                using (StreamWriter w = File.AppendText("myFile.txt"))
                                {
                                    w.Write(a);
                                }
                            }
                        }
                    }
                    
                }
                

            }
            catch//(Exception ex) 
            {
                //MessageBox.Show(ex.Message);
            }
            this.Invoke((MethodInvoker)delegate
            {
                label8.Text = a;
            });



        }


        public void dataviewinit()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.Black;
            dataGridView1.DefaultCellStyle.BackColor = Color.Black;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Yellow;
            dataGridView1.RowHeadersDefaultCellStyle.ForeColor = Color.Yellow;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.RowHeadersVisible = false;
        }

        public void chartview_init()
        {
            // chart 1 properties

            chart1.BackColor = Color.Black;
            //0 would be indice of chart area you wish to Change, Color.ColorYouWant
            chart1.ChartAreas[0].AxisX.LineColor = Color.Yellow;
            chart1.ChartAreas[0].AxisY.LineColor = Color.Yellow;

            chart1.Series[0].Color = Color.Yellow;
            chart1.Series[0].LabelForeColor = Color.Yellow;

            chart1.BorderlineColor = Color.Yellow;
            chart1.ChartAreas[0].BorderColor = Color.Yellow;
            chart1.ChartAreas[0].BackColor = Color.Black;


            chart1.ChartAreas[0].AxisX.TitleForeColor = Color.Yellow;
            chart1.ChartAreas[0].AxisY.LineColor = Color.Yellow;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            //To change the Colors of the interlacing lines you access them like so
            chart1.ChartAreas[0].AxisX.InterlacedColor = Color.Yellow;
            chart1.ChartAreas[0].AxisY.InterlacedColor = Color.Yellow;

            //If you are looking to change the color of the Grid Lines
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Black;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Black;

            chart1.Series["Series1"].Points.AddXY(0, 0);
            chart2.Series["Series1"].Points.AddXY(0, 0);


            // chart 2  properties


            chart1.BackColor = Color.Black;
            //0 would be indice of chart area you wish to Change, Color.ColorYouWant
            chart2.ChartAreas[0].AxisX.LineColor = Color.Yellow;
            chart2.ChartAreas[0].AxisY.LineColor = Color.Yellow;

            chart2.Series[0].Color = Color.Yellow;
            chart2.Series[0].LabelForeColor = Color.Yellow;

            chart2.BorderlineColor = Color.Yellow;
            chart2.ChartAreas[0].BorderColor = Color.Yellow;
            chart2.ChartAreas[0].BackColor = Color.Black;


            chart2.ChartAreas[0].AxisX.TitleForeColor = Color.Yellow;
            chart2.ChartAreas[0].AxisY.LineColor = Color.Yellow;
            //To change the Colors of the interlacing lines you access them like so
            chart2.ChartAreas[0].AxisX.InterlacedColor = Color.Yellow;
            chart2.ChartAreas[0].AxisY.InterlacedColor = Color.Yellow;

            //If you are looking to change the color of the Grid Lines
            chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Black;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Black;



        }



        private void t_Tick(object sender, EventArgs e)
        {
            count++;
            p = new Pen(Color.Green, 1f);
            g = Graphics.FromImage(bmp);
            int tu = (u - lim) % 360;
            if (u >= 0 && u <= 180)
            {
                x = cx + (int)(HAND * Math.Sin(Math.PI * u / 180));
                y = cy - (int)(HAND * Math.Cos(Math.PI * u / 180));
            }
            else
            {
                x = cx - (int)(HAND * -Math.Sin(Math.PI * u / 180));
                y = cy - (int)(HAND * Math.Cos(Math.PI * u / 180));
            }

            if (tu >= 0 && tu <= 180)
            {
                tx = cx + (int)(HAND * Math.Sin(Math.PI * tu / 180));
                ty = cy - (int)(HAND * Math.Cos(Math.PI * tu / 180));
            }
            else
            {
                tx = cx - (int)(HAND * -Math.Sin(Math.PI * tu / 180));
                ty = cy - (int)(HAND * Math.Cos(Math.PI * tu / 180));
            }

            //draw circle
            g.DrawEllipse(p, 0, 0, WIDTH, HEIGHT);  //bigger circle
            g.DrawEllipse(p, 80, 80, WIDTH - 160, HEIGHT - 160);    //smaller circle
            g.DrawEllipse(p, 40, 40, WIDTH - 80, HEIGHT - 80);    //smaller circle

            //draw perpendicular line
            g.DrawLine(p, new Point(cx, 0), new Point(cx, HEIGHT)); // UP-DOWN
            g.DrawLine(p, new Point(0, cy), new Point(WIDTH, cy)); //LEFT-RIGHT

            //draw HAND
            g.DrawLine(new Pen(Color.Black, 1f), new Point(cx, cy), new Point(tx, ty));
            g.DrawLine(p, new Point(cx, cy), new Point(x, y));

            //load bitmap in picturebox1
            pictureBox10.Image = bmp;

            //dispose
            p.Dispose();
            g.Dispose();

            //update
            u++;
            if (u == 360)
            {
                u = 0;
            }
            label29.Text = DateTime.Now.ToString("HH:mm:ss"); //result 22:11:45


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }


        private void label14_Click(object sender, EventArgs e)
        {
            TabPage t = tabControl1.TabPages[0];
            tabControl1.SelectTab(t);
            label14.BackColor = Color.Yellow;
            label14.ForeColor = Color.Black;
            label9.BackColor = Color.Black;
            label9.ForeColor = Color.Yellow;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            File.WriteAllText("myFile.txt", String.Empty);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            TabPage t = tabControl1.TabPages[1];
            tabControl1.SelectTab(t);
            label9.BackColor = Color.Yellow;
            label9.ForeColor = Color.Black;
            label14.BackColor = Color.Black;
            label14.ForeColor = Color.Yellow;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                _serialPort.Write("reference");
            }
            catch
            {
                MessageBox.Show("Port açıq deyil");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            rownumb = 0;
        }

        

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }


        /*                     displaying obtained image                    */

        public Image LoadImage(string base64string)        {
            byte[] bytes = Convert.FromBase64String(base64string);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }


        public void setimagetobitmap()
        {

            string lines = File.ReadAllText("myFile.txt");
            image_text = image_text + lines + "\n";
            try
            {
                Image i = LoadImage(lines);
                i = resizeImage(i, new Size(480, 480));
                 pictureBox4.Image = i;
                string FilePath = "image"+numbpackets+".jpg";
                Image newimg = (Image)i.Clone();
                newimg.Save(FilePath, ImageFormat.Jpeg);
            }
            catch
            {
                //MessageBox.Show("Şəkil düzgün formatda deyil");
            }

        }

        private static Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }



        /*                displaying obtained image                    */

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Faylı Saxlamaq üçün qovluğu seçin";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fbd.SelectedPath;

                var writer = new System.IO.StreamWriter(sSelectedPath + "/9721_TLM_2019.csv", false, System.Text.Encoding.UTF8);
                writer.Write(csv_text);
                writer.Close();

                var writer2 = new System.IO.StreamWriter(sSelectedPath + "/9721_IMG_2019.csv", false, System.Text.Encoding.UTF8);
                writer2.Write(image_text);
                writer2.Close();
            }
        }

        private void aGauge1_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }
    }
}
