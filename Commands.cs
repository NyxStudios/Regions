using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace Regions
{
    internal class Commands
    {
        private RegionManager Regions;

        public Commands( RegionManager r )
        {
            Regions = r;
        }

        public void Region(CommandArgs args)
        {
            string cmd = "help";
            if (args.Parameters.Count > 0)
            {
                cmd = args.Parameters[0].ToLower();
            }
            switch (cmd)
            {
                case "name":
                    {
                        {
                            args.Player.SendInfoMessage("Hit a block to get the name of the region");
                            args.Player.AwaitingName = true;
                        }
                        break;
                    }
                case "set":
                    {
                        int choice = 0;
                        if (args.Parameters.Count == 2 &&
                            int.TryParse(args.Parameters[1], out choice) &&
                            choice >= 1 && choice <= 2)
                        {
                            args.Player.SendInfoMessage("Hit a block to Set Point " + choice);
                            args.Player.AwaitingTempPoint = choice;
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region set [1/2]");
                        }
                        break;
                    }
                case "define":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            if (!args.Player.TempPoints.Any(p => p == Point.Zero))
                            {
                                string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                                var x = Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X);
                                var y = Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y);
                                var width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X);
                                var height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y);

                                if (Regions.AddRegion(x, y, width, height, regionName, args.Player.UserAccountName,
                                                             Main.worldID.ToString()))
                                {
                                    args.Player.TempPoints[0] = Point.Zero;
                                    args.Player.TempPoints[1] = Point.Zero;
                                    args.Player.SendSuccessMessage("Set region " + regionName);
                                }
                                else
                                {
                                    args.Player.SendWarningMessage("Region " + regionName + " already exists");
                                }
                            }
                            else
                            {
                                args.Player.SendErrorMessage("Points not set up yet.");
                            }
                        }
                        else
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region define [name]");
                        break;
                    }
                case "protect":
                    {
                        if (args.Parameters.Count == 3)
                        {
                            string regionName = args.Parameters[1];
                            if (args.Parameters[2].ToLower() == "true")
                            {
                                if (Regions.SetRegionState(regionName, true))
                                    args.Player.SendSuccessMessage("Protected region " + regionName);
                                else
                                    args.Player.SendErrorMessage("Could not find specified region");
                            }
                            else if (args.Parameters[2].ToLower() == "false")
                            {
                                if (Regions.SetRegionState(regionName, false))
                                    args.Player.SendSuccessMessage("Unprotected region " + regionName);
                                else
                                    args.Player.SendErrorMessage("Could not find specified region");
                            }
                            else
                                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region protect [name] [true/false]");
                        }
                        else
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region protect [name] [true/false]");
                        break;
                    }
                case "delete":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                            if (Regions.DeleteRegion(regionName))
                                args.Player.SendSuccessMessage("Deleted region " + regionName);
                            else
                                args.Player.SendErrorMessage("Could not find specified region");
                        }
                        else
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region delete [name]");
                        break;
                    }
                case "clear":
                    {
                        args.Player.TempPoints[0] = Point.Zero;
                        args.Player.TempPoints[1] = Point.Zero;
                        args.Player.SendInfoMessage("Cleared temp area");
                        args.Player.AwaitingTempPoint = 0;
                        break;
                    }
                case "allow":
                    {
                        if (args.Parameters.Count > 2)
                        {
                            string playerName = args.Parameters[1];
                            string regionName = "";

                            for (int i = 2; i < args.Parameters.Count; i++)
                            {
                                if (regionName == "")
                                {
                                    regionName = args.Parameters[2];
                                }
                                else
                                {
                                    regionName = regionName + " " + args.Parameters[i];
                                }
                            }
                            if (TShock.Users.GetUserByName(playerName) != null)
                            {
                                if (Regions.AddNewUser(regionName, playerName))
                                {
                                    args.Player.SendSuccessMessage("Added user " + playerName + " to " + regionName);
                                }
                                else
                                    args.Player.SendErrorMessage("Region " + regionName + " not found");
                            }
                            else
                            {
                                args.Player.SendErrorMessage("Player " + playerName + " not found");
                            }
                        }
                        else
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region allow [name] [region]");
                        break;
                    }
                case "remove":
                    if (args.Parameters.Count > 2)
                    {
                        string playerName = args.Parameters[1];
                        string regionName = "";

                        for (int i = 2; i < args.Parameters.Count; i++)
                        {
                            if (regionName == "")
                            {
                                regionName = args.Parameters[2];
                            }
                            else
                            {
                                regionName = regionName + " " + args.Parameters[i];
                            }
                        }
                        if (TShock.Users.GetUserByName(playerName) != null)
                        {
                            if (Regions.RemoveUser(regionName, playerName))
                            {
                                args.Player.SendSuccessMessage("Removed user " + playerName + " from " + regionName);
                            }
                            else
                                args.Player.SendErrorMessage("Region " + regionName + " not found");
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Player " + playerName + " not found");
                        }
                    }
                    else
                        args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region remove [name] [region]");
                    break;
                case "allowg":
                    {
                        if (args.Parameters.Count > 2)
                        {
                            string group = args.Parameters[1];
                            string regionName = "";

                            for (int i = 2; i < args.Parameters.Count; i++)
                            {
                                if (regionName == "")
                                {
                                    regionName = args.Parameters[2];
                                }
                                else
                                {
                                    regionName = regionName + " " + args.Parameters[i];
                                }
                            }
                            if (TShock.Groups.GroupExists(group))
                            {
                                if (Regions.AllowGroup(regionName, group))
                                {
                                    args.Player.SendSuccessMessage("Added group " + group + " to " + regionName);
                                }
                                else
                                    args.Player.SendErrorMessage("Region " + regionName + " not found");
                            }
                            else
                            {
                                args.Player.SendErrorMessage("Group " + group + " not found");
                            }
                        }
                        else
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region allow [group] [region]");
                        break;
                    }
                case "removeg":
                    if (args.Parameters.Count > 2)
                    {
                        string group = args.Parameters[1];
                        string regionName = "";

                        for (int i = 2; i < args.Parameters.Count; i++)
                        {
                            if (regionName == "")
                            {
                                regionName = args.Parameters[2];
                            }
                            else
                            {
                                regionName = regionName + " " + args.Parameters[i];
                            }
                        }
                        if (TShock.Groups.GroupExists(group))
                        {
                            if (Regions.RemoveGroup(regionName, group))
                            {
                                args.Player.SendSuccessMessage("Removed group " + group + " from " + regionName);
                            }
                            else
                                args.Player.SendErrorMessage("Region " + regionName + " not found");
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Group " + group + " not found");
                        }
                    }
                    else
                        args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region removeg [group] [region])");
                    break;
                case "list":
                    {
                        //How many regions per page
                        const int pagelimit = 15;
                        //How many regions per line
                        const int perline = 5;
                        //Pages start at 0 but are displayed and parsed at 1
                        int page = 0;


                        if (args.Parameters.Count > 1)
                        {
                            if (!int.TryParse(args.Parameters[1], out page) || page < 1)
                            {
                                args.Player.SendErrorMessage(string.Format("Invalid page number ({0})", page));
                                return;
                            }
                            page--; //Substract 1 as pages are parsed starting at 1 and not 0
                        }

                        var regions = Regions.ListAllRegions(Main.worldID.ToString());

                        // Are there even any regions to display?
                        if (regions.Count == 0)
                        {
                            args.Player.SendWarningMessage("There are currently no regions defined.");
                            return;
                        }

                        //Check if they are trying to access a page that doesn't exist.
                        int pagecount = regions.Count / pagelimit;
                        if (page > pagecount)
                        {
                            args.Player.SendWarningMessage(string.Format("Page number exceeds pages ({0}/{1})", page + 1, pagecount + 1));
                            return;
                        }

                        //Display the current page and the number of pages.
                        args.Player.SendSuccessMessage(string.Format("Current Regions ({0}/{1}):", page + 1, pagecount + 1));

                        //Add up to pagelimit names to a list
                        var nameslist = new List<string>();
                        for (int i = (page * pagelimit); (i < ((page * pagelimit) + pagelimit)) && i < regions.Count; i++)
                        {
                            nameslist.Add(regions[i].Name);
                        }

                        //convert the list to an array for joining
                        var names = nameslist.ToArray();
                        for (int i = 0; i < names.Length; i += perline)
                        {
                            args.Player.SendInfoMessage(string.Join(", ", names, i, Math.Min(names.Length - i, perline)));
                        }

                        if (page < pagecount)
                        {
                            args.Player.SendInfoMessage(string.Format("Type /region list {0} for more regions.", (page + 2)));
                        }

                        break;
                    }
                case "info":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                            Region r = Regions.GetRegionByName(regionName);

                            if (r == null)
                            {
                                args.Player.SendErrorMessage("Region {0} does not exist");
                                break;
                            }

                            args.Player.SendSuccessMessage(r.Name + ": P: " + r.DisableBuild + " X: " + r.Area.X + " Y: " + r.Area.Y + " W: " +
                                                    r.Area.Width + " H: " + r.Area.Height);
                            foreach (int s in r.AllowedIDs)
                            {
                                var user = TShock.Users.GetUserByID(s);
                                args.Player.SendSuccessMessage(r.Name + ": " + (user != null ? user.Name : "Unknown"));
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region info [name]");
                        }

                        break;
                    }
                case "z":
                    {
                        if (args.Parameters.Count == 3)
                        {
                            string regionName = args.Parameters[1];
                            int z = 0;
                            if (int.TryParse(args.Parameters[2], out z))
                            {
                                if (Regions.SetZ(regionName, z))
                                    args.Player.SendInfoMessage("Region's z is now " + z);
                                else
                                    args.Player.SendErrorMessage("Could not find specified region");
                            }
                            else
                                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region z [name] [#]");
                        }
                        else
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region z [name] [#]");
                        break;
                    }
                case "resize":
                case "expand":
                    {
                        if (args.Parameters.Count == 4)
                        {
                            int direction;
                            switch (args.Parameters[2])
                            {
                                case "u":
                                case "up":
                                    {
                                        direction = 0;
                                        break;
                                    }
                                case "r":
                                case "right":
                                    {
                                        direction = 1;
                                        break;
                                    }
                                case "d":
                                case "down":
                                    {
                                        direction = 2;
                                        break;
                                    }
                                case "l":
                                case "left":
                                    {
                                        direction = 3;
                                        break;
                                    }
                                default:
                                    {
                                        direction = -1;
                                        break;
                                    }
                            }
                            int addAmount;
                            int.TryParse(args.Parameters[3], out addAmount);
                            if (Regions.resizeRegion(args.Parameters[1], addAmount, direction))
                            {
                                args.Player.SendSuccessMessage("Region Resized Successfully!");
                                Regions.ReloadAllRegions();
                            }
                            else
                            {
                                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region resize [regionname] [u/d/l/r] [amount]");
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region resize [regionname] [u/d/l/r] [amount]1");
                        }
                        break;
                    }
                case "help":
                default:
                    {
                        args.Player.SendInfoMessage("Avialable region commands:");
                        args.Player.SendInfoMessage("/region set [1/2] /region define [name] /region protect [name] [true/false]");
                        args.Player.SendInfoMessage("/region name (provides region name)");
                        args.Player.SendInfoMessage("/region delete [name] /region clear (temporary region)");
                        args.Player.SendInfoMessage("/region allow [name] [regionname]");
                        args.Player.SendInfoMessage("/region resize [regionname] [u/d/l/r] [amount]");
                        break;
                    }
            }
        }
    }
}
