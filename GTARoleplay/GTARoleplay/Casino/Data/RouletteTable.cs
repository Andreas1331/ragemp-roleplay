using GTANetworkAPI;
using GTARoleplay.Animations;
using GTARoleplay.Character;
using GTARoleplay.Library.Attachments;
using GTARoleplay.Library.Attachments.Data;
using GTARoleplay.Library.Extensions;
using GTARoleplay.Library.Tasks;
using GTARoleplay.Library.Tasks.Data;
using GTARoleplay.Money;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTARoleplay.Casino.Data
{
    public class RouletteTable
    {
        public Dictionary<Player, int> playersAtTable = new Dictionary<Player, int>();
        
        /* Private variables */
        private List<RouletteBet> rouletteBets = new List<RouletteBet>();
        private bool isSpinning = false;
        private bool isBettingAllowed = false;
        private GTANetworkAPI.Object tableObject;
        private GTANetworkAPI.Object ballObject;
        private RoulettePocket winningPocket;

        /* Constants */
        private const int maxPlayersAtTable = 4;
        private const int maxSingleBetAmount = 500;
        private readonly Random rnd = new Random();

        private readonly static List<RoulettePocket> pockets = new List<RoulettePocket>()
        {
            new RoulettePocket(1, "00", (RouletteBets.Split | RouletteBets.Basket)),
            new RoulettePocket(20, "0", (RouletteBets.Split | RouletteBets.Basket)),
            new RoulettePocket(38, "1", (RouletteBets.Split | RouletteBets.Basket | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.FirstDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(19, "2", (RouletteBets.Split | RouletteBets.Basket | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.SecondDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(34, "3", (RouletteBets.Split | RouletteBets.Basket | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.ThirdDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(15, "4", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.FirstDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(30, "5", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.SecondDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(11, "6", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.ThirdDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(26, "7", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.FirstDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(7, "8", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.SecondDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(22, "9", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.ThirdDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(3, "10", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.FirstDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(25, "11", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.SecondDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(6, "12", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.FirstColumn | RouletteBets.ThirdDozen | RouletteBets.Even | RouletteBets.Red)),
            new RoulettePocket(37, "13", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.FirstDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(18, "14", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.SecondDozen | RouletteBets.Even | RouletteBets.Red)),
            new RoulettePocket(33, "15", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.ThirdDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(14, "16", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.FirstDozen | RouletteBets.Even | RouletteBets.Red)),
            new RoulettePocket(29, "17", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.SecondDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(10, "18", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.ThirdDozen | RouletteBets.Even | RouletteBets.Red)),
            new RoulettePocket(8, "19", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.FirstDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(27, "20", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.SecondDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(12, "21", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.ThirdDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(31, "22", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.FirstDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(16, "23", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.SecondDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(35, "24", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.SecondColumn | RouletteBets.ThirdDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(4, "25", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.FirstDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(23, "26", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.SecondDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(2, "27", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.ThirdDozen | RouletteBets.Odd | RouletteBets.Red)),
            new RoulettePocket(21, "28", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.FirstDozen | RouletteBets.Even | RouletteBets.Black)),
            new RoulettePocket(5, "29", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.SecondDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(24, "30", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.ThirdDozen | RouletteBets.Even | RouletteBets.Red)),
            new RoulettePocket(9, "31", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.FirstDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(28, "32", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.SecondDozen | RouletteBets.Even | RouletteBets.Red)),
            new RoulettePocket(13, "33", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.ThirdDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(32, "34", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.FirstDozen | RouletteBets.Even | RouletteBets.Red)),
            new RoulettePocket(17, "35", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.SecondDozen | RouletteBets.Odd | RouletteBets.Black)),
            new RoulettePocket(36, "36", (RouletteBets.Split | RouletteBets.Street | RouletteBets.Corner | RouletteBets.SixLine | RouletteBets.ThirdColumn | RouletteBets.ThirdDozen | RouletteBets.Even | RouletteBets.Red)),
        };

        public RouletteTable(GTANetworkAPI.Object tableObj)
        {
            tableObject = tableObj;
            ballObject = NAPI.Object.CreateObject(0x53281C8, tableObj.Position, new Vector3(0, 0, 29), dimension: 0);
        }

        public bool CanPlayerSitAtTable(Player player)
        {
            return playersAtTable.Count < maxPlayersAtTable && !playersAtTable.ContainsKey(player);
        }

        public void SitPlayerAtTable(Player player)
        {
            if (player == null || tableObject == null)
                return;

            // Get a free chair index for the player
            int freeChair = GetFreeChair();

            // Sit at table
            playersAtTable.Add(player, freeChair);
            string chairBoneName = GetChairBoneName(freeChair);

            player.TriggerEvent("SitAtRouletteTable::Client", tableObject, ballObject);
            AnimationHandler.StartAnimation(player, "sit4");
            // Attach the player to the assigned seat
            EntityAttachment entAttach = new EntityAttachment(player, tableObject, chairBoneName, new Vector3(0, 0, 1), new Vector3(0, 0, 270), true, true, true, 0, true);
            AttachmentHandler.CreateNewAttachment(player, entAttach);

            // If the wheel is not spinning, and betting is false then this is the first player to sit down
            if (!isSpinning && !isBettingAllowed)
                OpenTableForBets();
        }

        public void LeaveTable(Player player)
        {
            if(playersAtTable != null && playersAtTable.ContainsKey(player))
            {
                // Find his current bets if he has any and remove them
                List<RouletteBet> plyBets = rouletteBets.Where(x => x.Better.Equals(player)).ToList();
                if (plyBets != null)
                    rouletteBets = rouletteBets.Except(plyBets).ToList();
                playersAtTable.Remove(player);
                AnimationHandler.StopAnimation(player);
                AttachmentHandler.RemoveAttachment(player);
            }
        }

        private void SpinWheel()
        {
            isBettingAllowed = false;
            isSpinning = true;
            winningPocket = GenerateWinningResult();
            Console.WriteLine("Next winning pocket is: " + winningPocket.DisplayNumber + "," + winningPocket.Index);

            // Create the winning animations based on the randomly selected pocket
            string wheelAnim = $"exit_{winningPocket.Index}_wheel";
            string ballAnim = $"exit_{winningPocket.Index}_ball";
            playersAtTable.Keys.ToList().ForEach(x => x?.TriggerEvent("StartRouletteWheel::Client", wheelAnim, ballAnim));
            // TODO: Add a delayed call once the ball is done spinning around
            TaskManager.ScheduleTask(new ScheduledTask(OnSpinFinished, 25000));
        }

        private void OnSpinFinished()
        {
            isSpinning = false;
            playersAtTable.Keys.ToList().ForEach(x => x?.SendChatMessage($"Winning pocket on roulette: {winningPocket.DisplayNumber}."));
            CheckForWinners();

            // Don't start another round if there's no players left at the table
            if(playersAtTable != null && playersAtTable.Count > 0)
            {
                OpenTableForBets();
            }
        }

        private void CheckForWinners()
        {
            List<Player> winningPlayers = new List<Player>();

            foreach (Player player in playersAtTable.Keys)
            {
                if (player == null)
                    return;

                // Get the players bets
                List<RouletteBet> bets = rouletteBets.Where(x => x.Better.Equals(player)).ToList();
                int totalEarnings = 0;
                bets?.ForEach(bet =>
                {
                    // If the bet has a matching flag with the winning pocket we might have won
                    if (winningPocket.WinningFlags.HasFlag(bet.BetFlag))
                    {
                        // Some flags requires further checks while others like the red,black,odd,even just need to match
                        // The .Contains() check will see if the winningfield value is part of the fields in the bet
                        if (bet.BetFlag.Equals(RouletteBets.Red) ||
                            bet.BetFlag.Equals(RouletteBets.Black) ||
                            bet.BetFlag.Equals(RouletteBets.Odd) ||
                            bet.BetFlag.Equals(RouletteBets.Even) ||
                            bet.BettingFields.Contains(winningPocket.DisplayNumber))
                        {
                            int payoutRatio = 0;
                            if(RoulettePocket.PayoutRatios.TryGetValue(bet.BetFlag, out payoutRatio))
                            {
                                totalEarnings += ((bet.Amount * payoutRatio) + bet.Amount);
                            }
                        }
                    }
                });
                if (totalEarnings > 0)
                {
                    // The player won
                    GTACharacter charData = player.GetUserData()?.ActiveCharacter;
                    MoneyHandler.GivePlayerMoney(charData, totalEarnings);
                    player.SendChatMessage($"You won ~g~${totalEarnings} ~s~ from roulette!");
                }
                else
                    player.SendChatMessage("~r~You didn't win on roulette.");
            }

            // Clear the bets so the next round can start
            rouletteBets.Clear();
        }

        private void OpenTableForBets()
        {
            playersAtTable.Keys.ToList().ForEach(x => x?.TriggerEvent("OpenRouletteForBets::Client"));
            isBettingAllowed = true;
            TaskManager.ScheduleTask(new ScheduledTask(SpinWheel, 60000));
        }

        public void PlayerPlaceBet(Player player, List<string> bettingFields, RouletteBets betFlag, int bettingAmount)
        {
            // Ignore his bet if the wheel is already in motion, or if the player found a way to bet more than the allowed maximum
            if (isSpinning || !isBettingAllowed || bettingAmount > maxSingleBetAmount)
                return;

            GTACharacter charData = player.GetUserData()?.ActiveCharacter;
            if (MoneyHandler.HasEnoughMoney(charData, bettingAmount))
            {
                MoneyHandler.TakePlayerMoney(charData, bettingAmount);
                RouletteBet bet = new RouletteBet(player, bettingAmount, bettingFields, betFlag);
                rouletteBets.Add(bet);
            }
        }

        public void RemoveBet(Player player, List<string> bettingFields)
        {
            if (player == null || bettingFields == null)
                return;

            // Find the roulettebet in question using the betting fields provided
            if(rouletteBets.Any(x => x.BettingFields.SequenceEqual(bettingFields) && x.Better.Equals(player)))
            {
                var bet = rouletteBets.FirstOrDefault(x => x.BettingFields.SequenceEqual(bettingFields) && x.Better.Equals(player));
                var charData = player.GetUserData()?.ActiveCharacter;
                MoneyHandler.GivePlayerMoney(charData, bet.Amount);
                rouletteBets.Remove(bet);
            }
        }

        private RoulettePocket GenerateWinningResult()
        {
            return pockets[rnd.Next(pockets.Count)];
        }

        private string GetChairBoneName(int index)
        {
            switch (index)
            {
                case 0:
                    return "Chair_Base_01";
                case 1:
                    return "Chair_Base_02";
                case 2:
                    return "Chair_Base_03";
                case 3:
                    return "Chair_Base_04";
                default:
                    return "";
            }
        }

        private int GetFreeChair()
        {
            int index = 0;
            while(playersAtTable.Any(x => x.Value.Equals(index)))
            {
                index++;
            }
            return index;
        }
    }

    [Flags]
    public enum RouletteBets
    {
        StraightUp = 1 << 0,
        Split = 1 << 1,
        Basket = 1 << 2,
        Street = 1 << 3,
        Corner = 1 << 4,

        SixLine = 1 << 5, //

        FirstColumn = 1 << 6,
        SecondColumn = 1 << 7,
        ThirdColumn = 1 << 8,


        FirstDozen = 1 << 9,
        SecondDozen = 1 << 10,
        ThirdDozen = 1 << 11,

        Odd = 1 << 12,
        Even = 1 << 13,
        Red = 1 << 14,
        Black = 1 << 15,
        
        OneToEigthteen = 1 << 16, // Lower number
        NineteenToThirtySix = 1 << 17 // High number
    }
}
