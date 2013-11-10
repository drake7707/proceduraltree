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
    public partial class Tree2DForm : Form
    {

        private Bitmap bmp;
        private Graphics bmpG;

        public Tree2DForm()
        {
            InitializeComponent();

            // create a new tree at the start position and with the boundaries of the picturebox
            tree = new Tree2D()
            {
                InitialPosition = new Point2D(pic.Width / 2, 0),
                Boundaries = new Size(pic.Width, pic.Height)
            };

            tree.PixelSet += (oldx, oldy, x, y, c) =>
            {
                // when a pixel is set, daw the path from the old position to the new one
                
                    using (Pen p = new Pen(c, 2f))
                    {
                        if (oldx != default(int) && oldy != default(int))
                            bmpG.DrawLine(p, new Point(oldx, tree.Boundaries.Height - 1 - oldy), new Point(x, tree.Boundaries.Height - 1 - y));
                    }
                
                //bmp.SetPixel(x, tree.Boundaries.Height - 1 - y, c);
            };
            propGrid.SelectedObject = tree;

        }

        private Tree2D tree;

        private bool stop;

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (btnGenerate.Tag != null && (bool)btnGenerate.Tag == true)
            {
                // btnGenerate is acting as stop button, stop
                stop = true;
            }
            else
            {
                // standard generate button 

                btnGenerate.Text = "Stop";
                btnGenerate.Tag = true;

                stop = false;

                if (bmp != null)
                {
                    bmpG.Dispose();
                    bmp.Dispose();
                }

                // create a new bitmap
                bmp = new Bitmap(pic.Width, pic.Height);
                bmpG = Graphics.FromImage(bmp);

                pic.Image = bmp;

                
                tree.Boundaries = new Size(pic.Width, pic.Height);

                tree.InitialPosition = new Point2D(tree.Boundaries.Width / 2, 0); // new Point2(pic.Width / 2, 0);

                // initialize the tree
                tree.BuildTree();

                // max 1000 iterations
                for (int i = 0; i < 1000; i++)
                {
                    if (stop)
                        break;

                    lblParticleAmount.Text = "Iteration " + i + " - Amount of particles: " + tree.Particles.Count;

                    if (tree.Particles.Count <= 0)
                        break;

                    // go to the next iteration
                    tree.NextIteration();

                    // refresh
                    pic.Invalidate();
                    // the tree building could be done on a seperate thread, with the pixelset events stored in a queue
                    // and consumed on the gui thread in a timer but for now I just use a doevents.
                    Application.DoEvents();
                }

                btnGenerate.Text = "Generate";
                btnGenerate.Tag = false;
            }

            
        }
    }
}
