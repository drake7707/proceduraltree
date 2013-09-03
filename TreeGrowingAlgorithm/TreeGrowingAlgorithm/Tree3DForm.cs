using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cell3DVisualizer;

namespace TreeGrowingAlgorithm
{
    public partial class Tree3DForm : Form
    {

        private Bitmap bmp;

        private I3DControl wnd3d;

        public Tree3DForm()
        {
            InitializeComponent();

            tree = new Tree3DWithCache()
            {
                MaxLife = 64,
                ThickHeightRatio = 0.05f,
                BranchingPercent = 0.3f

            };

            tree.PixelSet += (oldx, oldy, oldz, x, y, z, c) =>
            {
                //using (Graphics g = Graphics.FromImage(bmp))
                //{
                //    using (Pen p = new Pen(c, 2f))
                //    {
                //        if(oldx != default(int) && oldy != default(int))
                //            g.DrawLine(p, new Point(oldx, tree.Boundaries.Height - 1 - oldy), new Point(x, tree.Boundaries.Height - 1 - y));
                //    }
                //}
                bmp.SetPixel(x, tree.Boundaries.Height - 1 - y, c);
            };
            propGrid.SelectedObject = tree;


            // TODO create a decent 3d control
            System.Windows.Forms.Integration.ElementHost host = new System.Windows.Forms.Integration.ElementHost();
            host.Dock = DockStyle.Fill;
            wnd3d = Cell3DVisualizer.WindowManager.CreateControl();
            host.Child = (System.Windows.UIElement)wnd3d;
            pnl3Dhost.Controls.Add(host);

            wnd3d.SetCameraPosition(64, 0, -64);
            wnd3d.SetCameraLookDirection(-64, 0, 64);

            
            
            //wnd3d.ShowWindow();
        }

        private Tree3DWithCache tree;

        private bool stop;

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            

            if (btnGenerate.Tag != null && (bool)btnGenerate.Tag == true)
            {
                stop = true;
            }
            else
            {
                tabControl1.SelectedIndex = 0;
                Application.DoEvents();

                btnGenerate.Text = "Stop";
                btnGenerate.Tag = true;

                stop = false;
                bmp = new Bitmap(pic.Width, pic.Height);
                pic.Image = bmp;

                
                tree.Boundaries = new Size3D(128,128,128);// new Size(pic.Width, pic.Height);

                tree.InitialPosition = new Point3D(tree.Boundaries.Width / 2, 0, tree.Boundaries.Depth / 2); // new Point2(pic.Width / 2, 0);
                tree.BuildTree();

                for (int i = 0; i < 1000; i++)
                {
                    if (stop)
                        break;

                    lblParticleAmount.Text = "Iteration " + i + " - Amount of particles: " + tree.Particles.Count;

                    if (tree.Particles.Count <= 0)
                        break;

                    tree.NextIteration();

                    pic.Invalidate();
                    Application.DoEvents();
                }

                btnGenerate.Text = "Generate";
                btnGenerate.Tag = false;

                tabControl1.SelectedIndex = 1;
            }

            wnd3d.Cells = new System.Windows.Media.Color[tree.Boundaries.Width, tree.Boundaries.Height, tree.Boundaries.Depth];


            System.Windows.Media.Color nullColor = System.Windows.Media.Colors.Transparent;
            for (int j = 0; j < tree.Boundaries.Height; j++)
            {
                for (int i = 0; i < tree.Boundaries.Width; i++)
                {
                    for (int k = 0; k < tree.Boundaries.Depth; k++)
                    {
                        var c = tree.Image[i, j, k];
                        if (c != Color.Empty)
                            wnd3d.Cells[i, tree.Boundaries.Height - 1 - j, k] = System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
                        else
                            wnd3d.Cells[i, tree.Boundaries.Height - 1 - j, k] = nullColor;
                    }
                }
            }
            wnd3d.Render();

        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 0)
                wnd3d.ZoomIn();
            else
                wnd3d.ZoomOut();
            
        }
    }

    
}
