using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace Regions
{
    internal class Regions : TerrariaPlugin
    {
        private RegionManager RegionManager;
        private IDbConnection DB;

        public delegate void InitializedD();
        public static event InitializedD Initialized;
        private Commands comms;

        public Regions( Main game) : base( game )
        {
            comms = new Commands();
        }

        public override string Author
        {
            get { return "Team Nyx"; }
        }

        public override string Description
        {
            get { return "Region Control"; }
        }

        public override string Name
        {
            get { return "Regions"; }
        }

        public override Version Version
        {
            get { return TShock.VersionNum; }
        }

        public override void Initialize()
        {
            DB = TShock.DB;
            RegionManager = new RegionManager(DB);

            Hooks.GameHooks.PostInitialize += OnPostInit;
            TShockAPI.Commands.ChatCommands.Add(new Command("manageregion", comms.Region, "region"));
        }

        private void OnPostInit()
        {
            RegionManager.ReloadAllRegions();
            Initialized();
        }

        protected override void Dispose(bool disposing)
        {
            if( disposing )
            {
                Hooks.GameHooks.PostInitialize -= OnPostInit;
            }

            base.Dispose(disposing);
        }

        public RegionManager GetRegionManager()
        {
            return RegionManager;
        }
    }
}
