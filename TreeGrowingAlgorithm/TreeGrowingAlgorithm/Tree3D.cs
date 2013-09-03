using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace TreeGrowingAlgorithm
{

    public class Tree3DWithCache : Tree3D
    {

        public override void BuildTree()
        {
            image = new Color[Boundaries.Width, Boundaries.Height, Boundaries.Depth];

            base.BuildTree();
        }

        private Color[, ,] image;
        [ReadOnly(true)]
        [Browsable(false)]
        public Color[, ,] Image
        {
            get { return image; }
            set { image = value; }
        }

        public override void NextIteration()
        {
            foreach (Particle3D p in particles.ToList())
            {
                if (p.Life > 0)
                {
                    p.Next();

                    if (p.Location.X >= 0 && p.Location.X < Boundaries.Width && p.Location.Y >= 0 && p.Location.Y < Boundaries.Height && p.Location.Z >= 0 && p.Location.Z < Boundaries.Depth)
                    {
                        bool changed = image[(int)p.Location.X, (int)p.Location.Y, (int)p.Location.Z] != p.Color;

                        if (changed)
                        {
                            //if (image[(int)p.Location.X, (int)p.Location.Y] == Color.Empty)
                            //{
                            image[(int)p.Location.X, (int)p.Location.Y, (int)p.Location.Z] = p.Color;

                            OnPixelSet(p);
                        }
                    }


                }
            }

            // cleanup
            particles = particles.Where(p => p.Life > 0).ToList();
        }

    }

    /// <summary>
    /// A tree in 3D, the tree generation is pretty much the same as in 2D, except for the additional random rotation around the Y axis to
    /// shoot particles in a 3D space rather than 2D
    /// </summary>
    public class Tree3D : TreeBase
    {
        private Random rnd;

        public Tree3D()
        {
            rnd = new Random();
        }

        public Tree3D(Random rnd)
        {
            this.rnd = rnd;
        }

        protected List<Particle3D> particles = new List<Particle3D>(100000);
        /// <summary>
        /// The particles in currently used to build the tree
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        public List<Particle3D> Particles
        {
            get { return particles; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public Point3D InitialPosition { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        public Size3D Boundaries { get; set; }





        public virtual void BuildTree()
        {
            particles.Clear();


            Particle3D trunk = new Particle3D()
            {
                Location = InitialPosition,
                Direction = new Vector3D(0, 1, 0),
                Life = maxLife,
                MaxLife = maxLife,
                Color = trunkColor,

                Behaviour = TrunkBehaviour
            };

            particles.Add(trunk);
        }


        public virtual void NextIteration()
        {
            foreach (Particle3D p in particles.ToList())
            {
                if (p.Life > 0)
                {
                    p.Next();

                    if (p.Location.X >= 0 && p.Location.X < Boundaries.Width && p.Location.Y >= 0 && p.Location.Y < Boundaries.Height && p.Location.Z >= 0 && p.Location.Z < Boundaries.Depth)
                    {
                        OnPixelSet(p);
                    }
                }
            }

            // cleanup
            particles = particles.Where(p => p.Life > 0).ToList();
        }

        protected virtual void OnPixelSet(Particle3D p)
        {
            //bmp.SetPixel((int)p.Location.X, pic.Height - 1 - (int)p.Location.Y, p.Color);
            PixelSetHandler temp = PixelSet;
            if (temp != null)
                temp((int)p.OldLocation.X, (int)p.OldLocation.Y, (int)p.OldLocation.Z, (int)p.Location.X, (int)p.Location.Y, (int)p.Location.Z, p.Color);
            //}
        }

        public event PixelSetHandler PixelSet;
        public delegate void PixelSetHandler(int oldx, int oldy, int oldz, int x, int y, int z, Color c);

        /// <summary>
        /// Describes what happens in a trunk particle 
        /// </summary>
        /// <param name="trunk">The trunk particle</param>
        private void TrunkBehaviour(Particle3D trunk)
        {
            int depth = 1;

            // stimulate branching where life is low
            if (stimulateBranchingAtEndOfLife)
                BranchingBehaviour((float)LineairScaleTo((((float)trunk.MaxLife - trunk.Life) / (float)trunk.MaxLife), 0.0f, 1f) * branchingPercent, trunk, depth);
            else
                BranchingBehaviour(branchingPercent, trunk, depth);

            // leaves behaviour
            LeavesBehaviour(trunk);

            // zigzag the tree a little
            TrunkNudgeBehaviour(trunk);


            // make the tree thicker
            TrunkThickingBehaviour(trunk);

            // make the trunk split sometimes
            TrunkSplittingBehaviour(trunk);
        }

        /// <summary>
        /// Describes how a trunk splits
        /// </summary>
        /// <param name="trunk">The trunk particle</param>
        private void TrunkSplittingBehaviour(Particle3D trunk)
        {
            // trunk split
            if (rnd.NextDouble() < trunkSplitPercentage)
            {
                Vector3D newDirection = trunk.Direction.RotateZ(rnd.Next(-Math.PI, Math.PI))
                                                       .RotateY(rnd.Next(0, 2 * Math.PI));

                Particle3D secondTrunk = new Particle3D()
                {
                    Location = trunk.Location,
                    Direction = newDirection,
                    Color = trunk.Color,
                    Life = (int)(trunk.Life * trunkSplitSpawnLifeDecrease),
                    MaxLife = (int)(trunk.Life * trunkSplitSpawnLifeDecrease),
                    Behaviour = trunk.Behaviour
                };
                particles.Add(secondTrunk);

                if (secondTrunk.Direction.Y < 0)
                    secondTrunk.Direction.Y *= -1;

                trunk.Life = (int)(trunk.Life * trunkSplitOrigLifeDecrease);

                // TODO
                trunk.Direction = new Vector3D(-newDirection.X, newDirection.Y, newDirection.Z); //trunk.Direction.Rotate(-Math.PI / 2 * rnd.NextDouble());


                if (trunk.Direction.Y < 0)
                    trunk.Direction.Y *= -1;

            }
        }


        /// <summary>
        /// Nudge the tree to create a zig-zag effect (so that the tree isn't dead straight)
        /// </summary>
        /// <param name="trunk">A trunk particle</param>
        private void TrunkNudgeBehaviour(Particle3D trunk)
        {
            // subtle x nudgings
            if (rnd.NextDouble() < zigzagTrunkPercentage)
            {
                trunk.Direction = trunk.Direction.RotateZ(LineairScaleTo(rnd.NextDouble(), -zigzagTrunkStrength, zigzagTrunkStrength))
                                                 .RotateY(rnd.Next(0, 2 * Math.PI));

            }

            //if (trunk.Direction.Y < 1)
            //{
            //    trunk.Direction.Y += 0.1f;
            //}
            //if (trunk.Direction.Y > 1)
            //    trunk.Direction.Y = 1;

        }

        /// <summary>
        /// Make the trunk of the tree bigger
        /// </summary>
        /// <param name="trunk"></param>
        private void TrunkThickingBehaviour(Particle3D trunk)
        {
            // thicking
            int maxTrunkLife = (int)(((float)trunk.Life / (float)trunk.MaxLife) * (thickHeightRatio * trunk.MaxLife));

            double piTimes2=  (2 * Math.PI);
            var trunkRotatedOnZ = trunk.Direction.RotateZ(-Math.PI / 2);

            // spawn particles in every possible direction
            int nrSpawn = 720;
            for (int i = 0; i < nrSpawn; i++)
            {
                Particle3D t1 = new Particle3D()
                {
                    Location = trunk.Location,
                    Direction = trunkRotatedOnZ.RotateY((i / (float)nrSpawn) * piTimes2)
                                               .RotateZ((i / (float)nrSpawn) * piTimes2),
                    Life = maxTrunkLife,
                    MaxLife = maxTrunkLife,
                    Color = trunk.Color
                };
                particles.Add(t1);
            }


            //Particle3D t2 = new Particle3D()
            //{
            //    Location = trunk.Location,
            //    Direction = trunk.Direction.RotateZ(Math.PI / 2),
            //    Life = maxTrunkLife,
            //    MaxLife = maxTrunkLife,
            //    Color = trunk.Color
            //};
            //particles.Add(t2);
        }

        /// <summary>
        /// Describes the branching and general behaviour of a branch
        /// </summary>
        /// <param name="branchingPercent"></param>
        /// <param name="branch"></param>
        /// <param name="depth"></param>
        private void BranchingBehaviour(float branchingPercent, Particle3D t, int depth)
        {
            if (rnd.NextDouble() < branchingPercent)//* (Math.Pow(0.5f, depth)))
            {
                // 90 * 2 ^ depth-1
                // [-90, 90] degrees => depth =1
                // [-45, 45] degrees => depth = 2
                // [-22.5, 
                double narrower = (Math.Pow(branchNarrowing, depth));

                // branch in a random direction but decrease the range based on the depth of the recursion
                // still branch in a random direction around the Y axis
                Vector3D initialDirection = t.Direction.RotateZ(rnd.Next(-Math.PI * narrower, Math.PI * narrower))
                                                       .RotateY(rnd.Next(0, Math.PI * 2));

                Particle3D branch = new Particle3D()
                {
                    Location = t.Location,
                    Direction = initialDirection,
                    MaxLife = (int)(t.Life * branchSizeDecrease),
                    Life = (int)(t.Life * branchSizeDecrease),
                    Color = branchColor,

                };
                branch.Behaviour = b =>
                {
                    LeavesBehaviour(b);

                    BranchingBehaviour(branchingPercent, b, depth + 1);

                    // weight behaviour
                    if (applyWeightOnBranches)
                        b.Direction = new Vector3D(initialDirection.X, initialDirection.Y * LineairScaleTo((double)b.Life / (double)branch.MaxLife, -1f, 1f), initialDirection.Z);// +(2 * (((double)b.Life / (double)maxLife)) - 1);
                };

                particles.Add(branch);

            }
        }

        /// <summary>
        /// Describes the behaviour of spawning leaves on branches
        /// </summary>
        /// <param name="b"></param>
        private void LeavesBehaviour(Particle3D b)
        {
            if (b.Life < leavesPercentage * b.MaxLife)
            {
                // create leaves
                for (int i = 0; i < leavesNrSpawned; i++)
                {
                    Particle3D l = new Particle3D()
                    {
                        Direction = new Vector3D(rnd.Next(-1f, 1f), rnd.Next(-1f, 1f), rnd.Next(-1f, 1f)),
                        Location = b.Location,
                        Color = leavesColor,
                        Life = leafLife,
                        MaxLife = leafLife
                    };
                    particles.Add(l);
                }
            }
        }

        /// <summary>
        /// Lineairly scale the val to [min, max]
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static double LineairScaleTo(double val, double min, double max)
        {
            return min + val * (max - min);
        }
    }
}
