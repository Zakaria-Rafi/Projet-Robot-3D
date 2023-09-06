using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph;


namespace Laser_Wenglor_3D
{
    public partial class _3Dview : Form
    {
        private Home mainform = null;
        Camera cam;
        Texture texture = new Texture();
        float angleX = 90.0f;
        float angleY = 0.0f;
        float angleZ = 90.0f;

        public _3Dview(Home Pmainform)
        {
            mainform = Pmainform as Home;
            InitializeComponent();


            SharpGL.OpenGL gl = this.openGLControl1.OpenGL;

            //gl.Enable(OpenGL.GL_TEXTURE_2D);


            float fov = 70.0f,
                aspect = (float)openGLControl1.Width / (float)openGLControl1.Height,
                zNear = 0.1f,
                zFar = 100.0f;
            Vertex eyeVertex = new Vertex(2.0f, 2.0f, 2.0f);
            Vertex centerVertex = new Vertex(0.0f, 0.0f, 0.0f);
            Vertex upVertex = new Vertex(0.0f, 1.0f, 0.0f);

            cam = new Camera(gl, fov, aspect, zNear, zFar, eyeVertex, centerVertex, upVertex);
        }

        double x = 0, y = 0, z = 0, xhbm = 0, ilaser = 0;
        double minZ = double.MinValue; 
        double maxZ = double.MaxValue;
        int frequencyCpt;
        int valuelimit = 0;
        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {

            SharpGL.OpenGL gl = this.openGLControl1.OpenGL;
            //color here
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            cam.Look();

            gl.Disable(OpenGL.GL_TEXTURE_2D);

            PlaneSurfaceRenderer psr = new PlaneSurfaceRenderer(16);
            psr.render(gl);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            texture.Bind(gl);

       

            if (mainform.WenglorConnected == true)
            {
                frequencyCpt = (int)mainform.frequencyHzscan1;
            }
            if (mainform.oxisconnected == true)
            {
                frequencyCpt = mainform.frequency;
            }

            if(frequencyCpt <= 10)
            {


                valuelimit = 10;

            }
            else if(frequencyCpt >= 11)
            {

                valuelimit = 60;

            }


            DrawPoints(gl);
        }

        private void DrawPoints(SharpGL.OpenGL gl)
        {
            gl.PointSize(0.5f);
            gl.PushMatrix();
            gl.Scale(-0.05f, -0.05f, -0.05f);


            if (mainform.xflag == true)
            {
                gl.Rotate(90.0f, 1.0f, 0.0f, 0.0f);
                gl.Rotate(0.0f, 0.0f, 1.0f, 0.0f);
                gl.Rotate(0.0f, 0.0f, 0.0f, 1.0f);
            }
            else
            {

                gl.Rotate(angleX, 1.0f, 0.0f, 0.0f);
                gl.Rotate(angleY, 0.0f, 1.0f, 0.0f);
                gl.Rotate(angleZ, 0.0f, 0.0f, 1.0f);

            }



            gl.Begin(OpenGL.GL_POINTS);



            int i;
            for (i = 0; i < mainform.Data.Count; i += valuelimit)
            {

                if (mainform.xflag == true)
                {
                    x = -mainform.Data[i].X;
                    y = mainform.Data[i].Xhbm;
                    z = -mainform.Data[i].Z;
                    xhbm = mainform.Data[i].Yhbm;
                    ilaser = mainform.Data[i].Ilaser;


                    if (z < minZ)
                        minZ = z;
                    if (z > maxZ)
                        maxZ = z;

                }
                else
                {
                    if (mainform.checkBox1Wenglor.Checked)
                    {


                        x = -mainform.Data[i].X;
                        y = mainform.Data[i].Yhbm - mainform.minY;
                        z = -mainform.Data[i].Z;
                        xhbm = mainform.Data[i].Xhbm;
                        ilaser = mainform.Data[i].Ilaser;




                        if (z < minZ)
                            minZ = z;
                        if (z > maxZ)
                            maxZ = z;

                    }

                    if (mainform.checkBox1Baumer.Checked)
                    {


                        x = mainform.Data[i].X;
                        y = mainform.Data[i].Yhbm - mainform.minY;
                        z = mainform.Data[i].Z;
                        xhbm = mainform.Data[i].Xhbm;
                        ilaser = mainform.Data[i].Ilaser;




                        if (z < minZ)
                            minZ = z;
                        if (z > maxZ)
                            maxZ = z;

                    }



                }






                double normalizedZ = (z - minZ) / (maxZ - minZ);
                double blueComponent = normalizedZ;
                double greenComponent = 1.0 - normalizedZ;
                double redComponent = 0.0;

                gl.Color((float)redComponent, (float)greenComponent, (float)blueComponent);


                if (mainform.checkBox1Baumer.Checked)
                {

                    gl.Vertex(x + xhbm + mainform.valueX, y + mainform.valueY, z + mainform.valueZ);


                }
                else
                {
                    if (mainform.xflag == true)
                    {
                        gl.Vertex(-x + xhbm +mainform.valueX , y + mainform.valueY, z + mainform.valueZ);


                    }
                    else
                    {

                        gl.Vertex(x + xhbm + mainform.valueX, y + mainform.valueY, z + mainform.valueZ);

                    }


                }



            }


            gl.End();
            gl.PopMatrix();

        }



        private void openGLControl1_MouseEnter(object sender, EventArgs e)
        {
            this.MouseWheel += Form1_MouseWheel;
        }

        private void openGLControl1_MouseLeave(object sender, EventArgs e)
        {
            this.MouseWheel -= Form1_MouseWheel;

        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control && sender == this)
            {
                //Console.WriteLine(e.Delta);
                if (e.Delta > 0)
                {
                    cam.ZoomIn();
                }
                else if (e.Delta < 0)
                {
                    cam.ZoomOut();
                }

                cam.Look();
            }
        }
        private int previousMouseX;
        private int previousMouseY;

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys != Keys.Control)
            {
                return;
            }

            int deltaX = e.X - previousMouseX;
            int deltaY = e.Y - previousMouseY;

            if (deltaX > 0)
            {
                cam.GoRight();
            }
            else if (deltaX < 0)
            {
                cam.GoLeft();
            }

            if (deltaY > 0)
            {
                cam.GoDown();
            }
            else if (deltaY < 0)
            {
                cam.GoUp();
            }

            cam.Look();

            previousMouseX = e.X;
            previousMouseY = e.Y;
        }
    }
}
