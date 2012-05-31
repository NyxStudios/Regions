using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace Regions
{
    [APIVersion( 1, 12 )]
    internal class Regions : TerrariaPlugin
    {
        private RegionManager RegionManager;
        private IDbConnection DB;
        private Commands comms;

        public Regions( Main game) : base( game )
        {
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
            comms = new Commands( RegionManager );
            Hooks.GameHooks.PostInitialize += OnPostInit;
            GetDataHandlers.TileEdit += OnTileEdit;
            TShockAPI.Commands.ChatCommands.Add(new Command("manageregion", comms.Region, "region"));
        }

        private void OnPostInit()
        {
            RegionManager.ReloadAllRegions();
        }

        private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
        {
            if (args.Player.AwaitingName)
            {
                var protectedregions = RegionManager.InAreaRegionName(args.X, args.Y);
                if (protectedregions.Count == 0)
                {
                    args.Player.SendMessage("Region is not protected", Color.Yellow);
                }
                else
                {
                    string regionlist = string.Join(",", protectedregions.ToArray());
                    args.Player.SendMessage("Region Name(s): " + regionlist, Color.Yellow);
                }
                args.Player.SendTileSquare(args.X, args.Y);
                args.Player.AwaitingName = false;
                args.Handled = true;
            }

            if (args.Handled)
            {
                return;
            }

            Region region = RegionManager.GetTopRegion(RegionManager.InAreaRegion(args.X, args.Y));
            if (!RegionManager.CanBuild(args.X, args.Y, args.Player))
            {
                if (((DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond) - args.Player.RPm) > 2000)
                {
                    args.Player.SendMessage("Region protected from changes.", Color.Red);
                    args.Player.RPm = DateTime.Now.Ticks/TimeSpan.TicksPerMillisecond;
                }
                args.Player.SendTileSquare(args.X, args.Y);
                args.Handled = true;
            }
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
