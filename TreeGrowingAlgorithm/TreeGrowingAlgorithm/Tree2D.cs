using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace TreeGrowingAlgorithm
{
    public class Tree2D : TreeBase
    {
        private Random rnd = new Random();

        private List<Particle2D> particles = new List<Particle2D>(100000);
        
        /// <summary>
        /// The particles in currently used to build the tree
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        public List<Particle2D> Particles
        {
            get { return particles; }
        }

      

        [ReadOnly(true)]
        [Browsable(false)]
        public Point2D InitialPosition { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        public Size Boundaries { get; set; }

        public void BuildTree()
        {
            // clear all current particles
            particles.Clear();

            // make a new array
            image = new Color[Boundaries.Width, Boundaries.Height];

            // create a trunk particle that will spawn the trunk
            Particle2D trunk = new Particle2D()
            {
                Location = InitialPosition,
                Direction = new Vector2D(0, 1),
                Life = maxLife,
                MaxLife = maxLife,
                Color = trunkColor,

                Behaviour = TrunkBehaviour
            };
            particles.Add(trunk);
        }

        private Color[,] image;
        [ReadOnly(true)]
        [Browsable(false)]
        public Color[,] Image
        {
            get { return image; }
            set { image = value; }
        }


        public void NextIteration()
        {
            // iterate over copy of all particles
            foreach (Particle2D p in particles.ToArray())
            {
                // if the particle still lives
                if (p.Life > 0)
                {
                    // advance the particle
                    p.Next();

                    // if it's within the boundaries
                    if (p.Location.X >= 0 && p.Location.X < Boundaries.Width && p.Location.Y >= 0 && p.Location.Y < Boundaries.Height)
                    {
                        bool changed = image[(int)p.Location.X, (int)p.Location.Y] != p.Color;
                        // if the value at the current position has been changed
                        if (changed)
                        {
                            // set the color of the particle on the position
                            image[(int)p.Location.X, (int)p.Location.Y] =  p.Color;
                            // raise the pixel set event
                            OnPixelSet(p);
                        }
                    }


                }
                else // else it's dead, remove the particle
                    particles.Remove(p);
            }
        }

        /// <summary>
        /// Raises the Pixel set with the given particle
        /// </summary>
        /// <param name="p"></param>
        protected virtual void OnPixelSet(Particle2D p)
        {
            PixelSetHandler temp = PixelSet;
            if (temp != null)
                temp((int)p.OldLocation.X, (int)p.OldLocation.Y, (int)p.Location.X, (int)p.Location.Y, p.Color);
        }

        public event PixelSetHandler PixelSet;
        public delegate void PixelSetHandler(int oldx, int oldy, int x, int y, Color c);




        /// <summary>
        /// Describes what happens in a trunk particle 
        /// </summary>
        /// <param name="trunk">The trunk particle</param>
        private void TrunkBehaviour(Particle2D trunk)
        {
            int depth = 1;

            // stimulate branching where life is low
            if (stimulateBranchingAtEndOfLife)
                BranchingBehaviour((float)LineairScaleTo((((float)trunk.MaxLife - trunk.Life) / (float)trunk.MaxLife), 0.0f, 1f) * branchingPercent, trunk, depth);
            else
                BranchingBehaviour(branchingPercent, trunk, depth);

            // leaves behaviour
            LeavesBehaviour(trunk);

            // make the tree thicker
            TrunkThickingBehaviour(trunk);

            // zigzag the tree a little
            TrunkNudgeBehaviour(trunk);

            // make the trunk split sometimes
            TrunkSplittingBehaviour(trunk);
        }

        /// <summary>
        /// Describes how a trunk splits
        /// </summary>
        /// <param name="trunk">The trunk particle</param>
        private void TrunkSplittingBehaviour(Particle2D trunk)
        {
            // trunk split
            if (rnd.NextDouble() < trunkSplitPercentage)
            {
                // spawn a branch of the trunk in a direction random between [-pi, pi]
                Vector2D newDirection = trunk.Direction.Rotate(rnd.Next(-Math.PI, Math.PI));

                Particle2D secondTrunk = new Particle2D()
                {
                    Location = trunk.Location,
                    Direction = newDirection,
                    Color = trunk.Color,
                    // reduce its max life with the specified decrease
                    Life = (int)(trunk.Life * trunkSplitSpawnLifeDecrease),
                    MaxLife = (int)(trunk.Life * trunkSplitSpawnLifeDecrease),
                    Behaviour = trunk.Behaviour
                };
                particles.Add(secondTrunk);

                // if the trhunk if going down, flip it (trunks don't grow down)
                if (secondTrunk.Direction.Y < 0)
                    secondTrunk.Direction.Y *= -1;

                // reduce the original trunk's life (because splitting has a cost)
                trunk.Life = (int)(trunk.Life * trunkSplitOrigLifeDecrease);
                // mirror the new trunk direction to give to the original trunk
                trunk.Direction = new Vector2D(-newDirection.X, newDirection.Y); //trunk.Direction.Rotate(-Math.PI / 2 * rnd.NextDouble());

                // again, check if it's going down and if so flip it
                if (trunk.Direction.Y < 0)
                    trunk.Direction.Y *= -1;

            }
        }


        /// <summary>
        /// Nudge the tree to create a zig-zag effect (so that the tree isn't dead straight)
        /// </summary>
        /// <param name="trunk">A trunk particle</param>
        private void TrunkNudgeBehaviour(Particle2D trunk)
        {
            // subtle x nudgings
            if (rnd.NextDouble() < zigzagTrunkPercentage)
            {
                // rotate the growing vector by a random zigzag strength factor
                trunk.Direction = trunk.Direction.Rotate(LineairScaleTo(rnd.NextDouble(), -zigzagTrunkStrength, zigzagTrunkStrength));
            }

            if (trunk.Direction.Y < 1)
                trunk.Direction.Y += 0.1f;

            if (trunk.Direction.Y > 1)
                trunk.Direction.Y = 1;

        }

        /// <summary>
        /// Make the trunk of the tree bigger
        /// </summary>
        /// <param name="trunk"></param>
        private void TrunkThickingBehaviour(Particle2D trunk)
        {
            // thicking
            int maxTrunkLife = (int)(((float)trunk.Life / (float)trunk.MaxLife) * (thickHeightRatio * trunk.MaxLife));

            // spawn 2 particles perpendicular to the trunk growing vector
            // their life values are based on the thick-height ratio
            Particle2D t1 = new Particle2D()
            {
                Location = trunk.Location,
                Direction = trunk.Direction.Rotate(-Math.PI / 2),
                Life = maxTrunkLife,
                MaxLife = maxTrunkLife,
                Color = trunk.Color
            };
            particles.Add(t1);

            Particle2D t2 = new Particle2D()
            {
                Location = trunk.Location,
                Direction = trunk.Direction.Rotate(Math.PI / 2),
                Life = maxTrunkLife,
                MaxLife = maxTrunkLife,
                Color = trunk.Color
            };
            particles.Add(t2);
        }

        /// <summary>
        /// Describes the branching and general behaviour of a branch
        /// </summary>
        /// <param name="branchingPercent"></param>
        /// <param name="branch"></param>
        /// <param name="depth"></param>
        private void BranchingBehaviour(float branchingPercent, Particle2D t, int depth)
        {
            if (rnd.NextDouble() < branchingPercent)//* (Math.Pow(0.5f, depth)))
            {
                // 90 * 2 ^ depth-1
                // [-90, 90] degrees => depth =1
                // [-45, 45] degrees => depth = 2
                // [-22.5, 
                double narrower = (Math.Pow(branchNarrowing, depth));

                // create a branch
                Vector2D initialDirection = t.Direction.Rotate(rnd.Next(-Math.PI * narrower, Math.PI * narrower));

                Particle2D branch = new Particle2D()
                {
                    Location = t.Location,
                    Direction = initialDirection,
                    // the life is based on the the life where it's splitting from (branch or trunk) but reduce the life of the new branch by a factor 
                    MaxLife = (int)(t.Life * branchSizeDecrease),
                    Life = (int)(t.Life * branchSizeDecrease),
                    Color = branchColor,

                };
                // define the behaviour of the branch
                branch.Behaviour = b =>
                        {
                            // branches should sprout leaves
                            LeavesBehaviour(b);

                            // branches can branch again
                            BranchingBehaviour(branchingPercent, b, depth + 1);

                            // weight behaviour
                            if (applyWeightOnBranches)
                                b.Direction = new Vector2D(initialDirection.X, initialDirection.Y * LineairScaleTo((double)b.Life / (double)branch.MaxLife, -1f, 1f));// +(2 * (((double)b.Life / (double)maxLife)) - 1);
                        };

                particles.Add(branch);
            }
        }

        /// <summary>
        /// Describes the behaviour of spawning leaves on branches
        /// </summary>
        /// <param name="b"></param>
        private void LeavesBehaviour(Particle2D b)
        {
            // if the branch doesn't have a lot of life left (based on the leaves percentage)
            if (b.Life < leavesPercentage * b.MaxLife)
            {
                // create leaf particles
                for (int i = 0; i < leavesNrSpawned; i++)
                {
                    // leaves have no particular behaviour, they just grow until its life is done
                    Particle2D l = new Particle2D()
                    {
                        Direction = new Vector2D(rnd.Next(-1f, 1f), rnd.Next(-1f, 1f)),
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
