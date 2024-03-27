using GTANetworkAPI;
using GTARoleplay.Casino.Data;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Wheel.Containers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTARoleplay.Casino
{
    public class RouletteHandler
    {
        public static readonly Dictionary<GTANetworkAPI.Object, RouletteTable> AllRouletteTables = new Dictionary<GTANetworkAPI.Object, RouletteTable>();

        public RouletteHandler()
        {
            NAPI.ClientEvent.Register<Player, string, int, int>("OnPlayerBetRoulette::Server", this, PlayerRouletteBet);
            NAPI.ClientEvent.Register<Player, string>("OnPlayerRemoveBetRoulette::Server", this, PlayerRemoveRouletteBet);
            NAPI.ClientEvent.Register<Player>("OnPlayerLeaveRouletteTable::Server", this, PlayerLeaveRouletteTable);
        }

        public void PlayerRouletteBet(Player player, string bettingFields, int _betFlag, int betAmount)
        {
            try
            {
                RouletteTable table = AllRouletteTables.Values.FirstOrDefault(x => x.playersAtTable.ContainsKey(player));
                if(table != null)
                {
                    RouletteBets betFlag = (RouletteBets)_betFlag;
                    table.PlayerPlaceBet(player, NAPI.Util.FromJson<List<string>>(bettingFields), betFlag, betAmount);
                }
            } catch(Exception exp)
            {
                // TODO: log the exception
            }
        }

        public void PlayerRemoveRouletteBet(Player player, string bettingFields)
        {
            try
            {
                RouletteTable table = AllRouletteTables.Values.FirstOrDefault(x => x.playersAtTable.ContainsKey(player));
                if (table != null)
                {
                    table.RemoveBet(player, NAPI.Util.FromJson<List<string>>(bettingFields));
                }
            }
            catch (Exception exp)
            {
                // TODO: log the exception
            }
        }

        public void PlayerLeaveRouletteTable(Player player)
        {
            var table = AllRouletteTables.Values.FirstOrDefault(x => x.playersAtTable.ContainsKey(player));
            table?.LeaveTable(player);
        }

        public static void CreateRouletteInteractionWheel(Player player, GTANetworkAPI.Object tableObj)
        {
            RouletteTable rTable = GetRouletteTableFromObj(tableObj);

            PrimaryInteractionWheel wheel = new PrimaryInteractionWheel(0, "Roulette table", ConsoleKey.E);
            List<object> slices = new List<object>()
                {
                    new WheelSliceAction("Join", () => {
                        if (!rTable.CanPlayerSitAtTable(player))
                        {
                            player.SendChatMessage("There's not enough room at this table, or you're already sitting at this table.");
                            return;
                        }
                        rTable.SitPlayerAtTable(player);
                    })
                };
            wheel.Slices = slices;
            wheel.Display(player);
        }

        private static RouletteTable GetRouletteTableFromObj(GTANetworkAPI.Object tableObj)
        {
            // Figure out if the RouletteTable has been instantiated or not
            bool exists = AllRouletteTables.ContainsKey(tableObj);
            RouletteTable rTable = exists ? AllRouletteTables[tableObj] : new RouletteTable(tableObj);
            if (!exists)
                AllRouletteTables.Add(tableObj, rTable);
            return rTable;
        }
    }
}
