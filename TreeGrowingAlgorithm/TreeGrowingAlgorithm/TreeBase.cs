using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace TreeGrowingAlgorithm
{
    public class TreeBase
    {
        
        private int randomSeed;
        [Description(@"Sets a fixed seed for the rng. If 0, it will choose a random seed")]
        public int RandomSeed
        {
            get { return randomSeed; }
            set
            {
                if (randomSeed != value)
                {
                    randomSeed = value;
                    OnRandomSeedChanged();
                }
            }
        }

        protected virtual void OnRandomSeedChanged()
        {
        }


        #region trunk
        protected int maxLife = 400;
        /// <summary>
        /// Determines how much life the tree trunk starts with
        /// </summary>
        [Description(@"Determines how much life the growing vector of the tree trunk starts with")]
        [Category("Trunk")]
        public int MaxLife
        {
            get { return maxLife; }
            set { maxLife = value; }
        }

        protected float thickHeightRatio = 0.02f;
        /// <summary>
        /// Determines how thick the trunk will be based on its height (life)
        /// E.g The max life of a trunk of 400 with a thickHeightRatio of 0.02 will yield
        /// a tree with a trunk of 16 wide ( 2 * thickHeightRatio * max life)
        /// </summary>
        [Description(@"Determines how thick the trunk will be based on its height (life). 
E.g The max life of a trunk of 400 with a thickHeightRatio of 0.02 will yield a tree with a trunk of 16 wide ( 2 * thickHeightRatio * max life)")]
        [Category("Trunk")]
        public float ThickHeightRatio
        {
            get { return thickHeightRatio; }
            set { thickHeightRatio = value; }
        }

        protected float trunkSplitPercentage = 0.01f;
        /// <summary>
        /// Determines how much chanche there is a trunk splits in each iteration
        /// </summary>
        [Description(@"Determines how much chanche there is a trunk splits in each iteration")]
        [Category("Trunk")]
        public float TrunkSplitPercentage
        {
            get { return trunkSplitPercentage; }
            set { trunkSplitPercentage = value; }
        }

        protected float trunkSplitSpawnLifeDecrease = 0.5f;
        /// <summary>
        /// Determines how much life the growing vector of the newly created trunk spawn should have
        /// </summary>
        [Description(@"Determines how much life the growing vector of the newly created trunk spawn should have.")]
        [Category("Trunk")]
        public float TrunkSplitSpawnLifeDecrease
        {
            get { return trunkSplitSpawnLifeDecrease; }
            set { trunkSplitSpawnLifeDecrease = value; }
        }

        protected float trunkSplitOrigLifeDecrease = 1f;
        /// <summary>
        /// Determines how much % the life should be from the remaining life of the original trunk growing vector
        /// </summary>
        [Description(@"Determines how much % the life should be from the remaining life of the original trunk growing vector.")]
        [Category("Trunk")]
        public float TrunkSplitOrigLifeDecrease
        {
            get { return trunkSplitOrigLifeDecrease; }
            set { trunkSplitOrigLifeDecrease = value; }
        }


        protected float zigzagTrunkPercentage = 0.1f;
        /// <summary>
        /// Determines how much chanche there is a trunk gets nudges into a horizontal position
        /// This prevents the trunk being dead straight
        /// </summary>
        [Description(@"Determines how much chanche there is a trunk gets nudges into a horizontal position. This prevents the trunk being dead straight")]
        [Category("Trunk")]
        public float ZigzagTrunkPercentage
        {
            get { return zigzagTrunkPercentage; }
            set { zigzagTrunkPercentage = value; }
        }

        protected float zigzagTrunkStrength = (float)Math.PI / 10; //0.2f;
        /// <summary>
        /// Determines the angle (in degrees) of the nudge
        /// </summary>
        [Description(@"Determines the angle of the nudge")]
        [Category("Trunk")]
        public float ZigzagTrunkStrength
        {
            get { return zigzagTrunkStrength * (360f / ((float)Math.PI * 2)); }
            set { zigzagTrunkStrength = value / (360f / ((float)Math.PI * 2)); }
        }

        protected bool stimulateBranchingAtEndOfLife = true;
        /// <summary>
        /// If true, branching will have an increasing chanche when the growing vector of the trunk reaches its end of life
        /// </summary>
        [Description(@"If true, branching will have an increasing chanche when the growing vector of the trunk reaches its end of life")]
        [Category("Trunk")]
        public bool StimulateBranchingAtEndOfLife
        {
            get { return stimulateBranchingAtEndOfLife; }
            set { stimulateBranchingAtEndOfLife = value; }
        }


        #endregion

        #region branches
        protected float branchingPercent = 0.2f;
        /// <summary>
        /// Determines the chanche that a branch or trunk will branch
        /// </summary>
        [Description(@"Determines the chanche that a branch or trunk will branch.")]
        [Category("Branches")]
        public float BranchingPercent
        {
            get { return branchingPercent; }
            set { branchingPercent = value; }
        }


        protected float branchSizeDecrease = 0.75f;
        /// <summary>
        /// Determines how much life the branch will spawn with based on the life of the parent branch or trunk
        /// E.g a branch size decrease of 0.75 will yield a maximum life of 0.75 * life of parent branch of the 
        /// growing vector
        /// </summary>
        [Description(@"Determines how much life the branch will spawn with based on the life of the parent branch or trunk. 
E.g a branch size decrease of 0.75 will yield a maximum life of 0.75 * life of parent branch of the growing vector")]
        [Category("Branches")]
        public float BranchSizeDecrease
        {
            get { return branchSizeDecrease; }
            set { branchSizeDecrease = value; }
        }

        protected float branchNarrowing = 0.5f;
        /// <summary>
        /// Determines when a trunk or branch spawns a new branch what the range of the angle the growing vector can be in
        /// E.g branching starts at a trunk with range [-90°,90°], with a branchNarrowing factor of 0.5, the branches that 
        /// spawn from that branch (recursive) will be in [-45°, 45°] angle of the original growing vector of the branch that
        /// spawns the new branches.
        /// </summary>
        [Description(
@"Determines when a trunk or branch spawns a new branch what the range of the angle the growing vector can be in
E.g branching starts at a trunk with range [-90°,90°], with a branchNarrowing factor of 0.5, the branches that spawn from that branch (recursive) will be in [-45°, 45°] angle of the original growing vector of the branch that spawns the new branches.")]
        [Category("Branches")]
        public float BranchNarrowing
        {
            get { return branchNarrowing; }
            set { branchNarrowing = value; }
        }


        protected bool applyWeightOnBranches = true;
        /// <summary>
        /// If true applies a weight to the branches so they bend (exponentially) downwards
        /// </summary>
        [Description("If true applies a weight to the branches so they bend (exponentially) downwards")]
        [Category("Branches")]
        public bool ApplyWeightOnBranches
        {
            get { return applyWeightOnBranches; }
            set { applyWeightOnBranches = value; }
        }

        #endregion

        #region leaves
        protected float leavesPercentage = 0.2f;
        /// <summary>
        /// Determines when leaves should start growing
        /// E.g a leave percentage of 0.2 will spawn leaves when the life of a branch is below 0.2
        /// and will continue to spawn leaves at each iteration until the branch growing particle is dead
        /// </summary>
        [Description(@"Determines when leaves should start growing. 
E.g a leave percentage of 0.2 will spawn leaves when the life of a branch is below 0.2 and will continue to spawn leaves at each iteration until the branch growing particle is dead")]
        [Category("Leaves")]
        public float LeavesPercentage
        {
            get { return leavesPercentage; }
            set { leavesPercentage = value; }
        }

        protected int leavesNrSpawned = 2;

        /// <summary>
        /// The amount of leave growing vectors spawned (each iteration) when leaves have to grow
        /// </summary>
        [Description(@"The amount of leave growing vectors spawned (each iteration) when leaves have to grow")]
        [Category("Leaves")]
        public int NumberOfLeavesSpawned
        {
            get { return leavesNrSpawned; }
            set { leavesNrSpawned = value; }
        }

        protected int leafLife = 5;
        /// <summary>
        /// The initial life of the growing vector of a leaf
        /// </summary>
        [Description(@"The initial life of the growing vector of a leaf")]
        [Category("Leaves")]
        public int LeafLife
        {
            get { return leafLife; }
            set { leafLife = value; }
        }
        #endregion

        #region colors
        protected Color trunkColor = Color.Brown;

        /// <summary>
        /// Color of the trunk
        /// </summary>
        [Description(@"Color of the trunk")]
        [Category("Trunk")]
        public Color TrunkColor
        {
            get { return trunkColor; }
            set { trunkColor = value; }
        }

        protected Color branchColor = Color.Brown;

        /// <summary>
        /// Color of the branches
        /// </summary>
        [Description(@"Color of the branches")]
        [Category("Branches")]
        public Color BranchColor
        {
            get { return branchColor; }
            set { branchColor = value; }
        }


        protected Color leavesColor = Color.ForestGreen;

        /// <summary>
        /// Color of the leaves
        /// </summary>
        [Description(@"Color of the leaves")]
        [Category("Leaves")]
        public Color LeavesColor
        {
            get { return leavesColor; }
            set { leavesColor = value; }
        }
        #endregion
    }
}
