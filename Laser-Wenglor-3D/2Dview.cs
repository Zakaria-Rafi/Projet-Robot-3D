using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Baumer.OXApi;
using Baumer.OXApi.Types;
namespace Laser_Wenglor_3D
{
    public partial class _2Dview : Form
    {
        private Home FormMain = null;
        private Thread scanThread;
        bool runingthread = false;

        private float x;
        private float z;
        public _2Dview(Form pFormMain)
        {
            FormMain = pFormMain as Home;

            InitializeComponent();

            scanThread = new Thread(ScanThreadMethod);
            scanThread.Start();
            runingthread = true;
        }

        private void ScanThreadMethod()
        {
            DateTime timeGetInfoTemp = DateTime.Now;
            TimeSpan timeDiff = new TimeSpan();
            while (runingthread)
            {

                if (FormMain.WenglorConnected == true)
                {

                    if (FormMain.m_iScannerDataLen > 0)
                    {



                        timeDiff = DateTime.Now - timeGetInfoTemp;
                        if (timeDiff.TotalMilliseconds > 100)
                        {
                            timeGetInfoTemp = DateTime.Now;




                            ShowScanXZI();



                        }
                    }

                }

                if(FormMain.oxisconnected == true)
                {
                    var profile = FormMain.ox.GetProfile();

                    DisplayProfileData(profile);


                }




            }

        }


        private void ShowScanXZI()
        {
            try
            {
                double faktorX = 0;
                double faktorZ = 0;
                double faktorI = 0;

                int dwPixelSize = 1;
                int gridSizeZ = 50;
                double minZscan1 = double.MaxValue;
                double maxZscan1 = double.MinValue;

                Bitmap bitmapScan1 = new Bitmap(m_pictureBoxScanView.Width, m_pictureBoxScanView.Height);

                using (Graphics g1 = Graphics.FromImage(bitmapScan1))
                {
                    g1.FillRectangle(Brushes.Black, 0, 0, bitmapScan1.Width, bitmapScan1.Height);

                    faktorX = (double)(bitmapScan1.Width - dwPixelSize) / (double)FormMain.m_CScanView1_X_Range_At_End;
                    faktorZ = (double)(bitmapScan1.Height - dwPixelSize) / (double)FormMain.m_CScanView1_Z_Range;
                    faktorI = (double)(bitmapScan1.Height - dwPixelSize) / (double)1024;

                    for (int i = gridSizeZ; i <= FormMain.m_CScanView1_Z_Range; i += gridSizeZ)
                    {
                        int z = (int)(faktorZ * (double)(i - FormMain.m_CScanView1_Z_Start));
                        if (z >= bitmapScan1.Height)
                        {
                            break;
                        }

                        g1.DrawLine(Pens.Blue, 0, z, (int)(0.1 * bitmapScan1.Width), z);

                        g1.DrawString(i.ToString(), DefaultFont, Brushes.White, 0, z);
                    }

                    if (true)
                    {
                        for (int i = 0; i < FormMain.m_iScannerDataLen; i++)
                        {


                            int x = (int)(faktorX * (double)(FormMain.m_doX[i] + FormMain.m_CScanView1_X_Range_At_End / 2));


                            if ((int)(FormMain.m_doZ[i] - FormMain.m_CScanView1_Z_Start) > 0)
                            {
                                int z = (int)(faktorZ * (double)(FormMain.m_doZ[i] - FormMain.m_CScanView1_Z_Start));
                                if (z >= bitmapScan1.Height)
                                {
                                    z = bitmapScan1.Height - 1;
                                }
                                else if (z < 0)
                                {
                                    z = 0;
                                }
                                if (x >= 0 && x < bitmapScan1.Width)
                                {
                                    bitmapScan1.SetPixel(x, z, Color.White);
                                }




                                if (FormMain.m_doZ[i] < minZscan1)
                                {
                                    minZscan1 = FormMain.m_doZ[i];
                                }

                                if (FormMain.m_doZ[i] > maxZscan1)
                                {
                                    maxZscan1 = FormMain.m_doZ[i];
                                }

                            }
                        }




                    }

                    if (true)
                    {
                        for (int i = 0; i < FormMain.m_iScannerDataLen; i++)
                        {
                            int x = (int)(faktorX * (double)(FormMain.m_doX[i] + FormMain.m_CScanView1_X_Range_At_End / 2));
                            int intensity = (int)(faktorI * (1024 - (double)FormMain.m_iIntensity[i]));
                            if (intensity >= 0 && intensity < bitmapScan1.Height && x >= 0 && x < bitmapScan1.Width)
                            {
                                bitmapScan1.SetPixel(x, intensity, Color.Yellow);
                            }
                        }
                    }



                }



                m_pictureBoxScanView.Image = bitmapScan1;
                m_pictureBoxScanView.Invalidate();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de Scan 1: " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }



        private void DisplayProfileData(Profile profile)
        {
            try
            {

                float maxXValue = 140;
                float maxZValue = 110;
                float pre = profile.Precision;
                using (Bitmap image = new Bitmap(m_pictureBoxScanView.Width, m_pictureBoxScanView.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        graphics.Clear(Color.Black);
                        for (int i = 0; i < profile.Length; i++)
                        {
                            x = (profile.X[i] + profile.XStart) / pre;
                            z = profile.Z[i] / pre;


                            float xPos = (x + 70) * m_pictureBoxScanView.Width / maxXValue;
                            float zPos = (z + 5) * m_pictureBoxScanView.Height / maxZValue;

                            image.SetPixel((int)xPos, (int)zPos, Color.Red);
                        }

                    }

                    
                        m_pictureBoxScanView.Image = (System.Drawing.Image)image.Clone();

                   

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de Scan : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }



        private void _2Dview_FormClosing(object sender, FormClosingEventArgs e)
        {
            runingthread = false;
            scanThread = null;
        }
    }
}
