namespace MemoryGame.Domain.Models
{
    public static class ScoreCalculator
    {
        //cards = total number of cards in the game -> 2* pairCount
        //seconds = total time taken to complete the game in seconds
        //attempts = total number of attempts made to complete the game (2 flips = 1 attempt)
        /// <summary>
        /// Calculates a score based on the total number of cards, time taken, and attempts made in a game.
        /// </summary>
        /// <remarks>The score is determined based on the parameters, which represent
        /// the game's difficulty (amount of pairs), duration, and Attempts. All inputs must be positive ints.</remarks>
        /// <param name="cards">The total number of cards in the game. Must be greater than 0.</param>
        /// <param name="seconds">The total time taken to complete the game, in seconds. Must be greater than 0.</param>
        /// <param name="attempts">The total number of attempts made to complete the game, where 2 flips equal 1 attempt. Must be greater than
        /// 0.</param>
        /// <returns>An integer representing the calculated performance score. Returns 0 if any of the parameters are invalid.</returns>
        public static int Calculate(int cards, int seconds, int attempts)
        {
            if (cards <= 0 || seconds <= 0 || attempts <= 0)
            {
                return 0; //Invalid parameters
            }

            //weight based on number of cards, more cards = more reward
            double cardWeight = cards * cards;

            //time and attempts are penalties, less time and attempts = higher score
            double difficulty = seconds * attempts;

            //Base score calculation as per the formula provided by the assignmentin the github
            double rawScore = cardWeight / difficulty * 1000;

            //round down to nearest int
            return (int)Math.Floor(rawScore);



        }
    }
}
