using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoMinMax
{
    class Program
    {

        static int[] p1Rep = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static int[] p2Rep = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static int[][] initialState = { p1Rep, p2Rep };
        static Move move;
        public static bool endGame = false; // :D
        static void Main(string[] args)
        {
            //1st player turn
            move = new Move();
            move.Representation = initialState;
            move.parent = null;
            move.homePawns = 0;
            generateChanceNodes(move, true);

        }

        static void generateChanceNodes(Move m, bool maxPlayer)
        {
            //6 possible chances -> 6 chance nodes
            int[] currentState;
            for (int i = 1; i <= 6; i++)
            {
                Move newMove = new Move();
                currentState = m.Representation[0];
                newMove.Representation[0] = currentState;
                newMove.parent = m;
                newMove.chanceNode = i;
                currentState = m.Representation[1];
                newMove.Representation[1] = currentState;
                m.addChild(newMove);
                checkNextStates(newMove, true);
            }
        }
        static void checkNextStates(Move m, bool maxPlayer)
        {
            List<int> pawnPositions1 = new List<int>();
            List<int> pawnPositions2 = new List<int>();
            int[] currentState;
            for (int i = 0; i < m.Representation[0].Length; i++)
            {
                if (m.Representation[0][i] == 1)
                {
                    pawnPositions1.Add(i); //add index of array
                }
                if (m.Representation[1][i] == 2)
                {
                    pawnPositions2.Add(i);
                }
            }

            if (maxPlayer)
            {
                //if chance state is 6 we can insert a pawn
                //for now only 
                if (m.chanceNode == 6 && m.Representation[0][0] == 0 && (pawnPositions1.Count + m.homePawns) != 4)
                {
                    //setting new representation
                    currentState = reinitialize(m.Representation[0]);
                    currentState[0] = 1;
                    Move nextMove = new Move();
                    nextMove.parent = m;
                    nextMove.Representation[0] = currentState;
                    currentState = m.Representation[1];
                    nextMove.Representation[1] = currentState;
                    nextMove.score = 5; //as the max score;
                    m.addChild(nextMove);
                }
                else
                {
                    //check number of pawns and their positions pawnPositions list
                    for (int i = 0; i < pawnPositions1.Count(); i++)
                    {
                        currentState = reinitialize(m.Representation[0]);
                       
                        // get active pawn i
                        int position = pawnPositions1.ElementAt(i);

                        Move nextState = new Move();
                        nextState.parent = m;
                        //check if it can destroy another pawn
                        if (checkIfCanAttack(position, m.chanceNode, pawnPositions2))
                        {
                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 1;
                            nextState.Representation[0] = currentState;

                            currentState = m.Representation[1];
                            int pos1 = (position + m.chanceNode) % 48;
                            //updating 2nd player table;
                            if (currentState[pos1] == 2)
                            {
                                currentState[pos1] = 0;
                                nextState.Representation[1] = currentState;
                            }
                            pos1 = Math.Abs(position - m.chanceNode);
                            if(currentState[pos1] == 2)
                            {
                                currentState[pos1] = 0;
                                nextState.Representation[1] = currentState;
                            }
                            nextState.score = 5;
                        }
                        //CHECK if this move sends a pawn home
                        else if(position + m.chanceNode > 47)
                        {
                            currentState[position] = 0;
                            nextState.Representation[0] = currentState;
                            nextState.homePawns += 1;
                            currentState = m.Representation[1];
                            nextState.Representation[1] = currentState;
                            nextState.score = 5;
                        }
                        else
                        {
                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 1;
                            nextState.Representation[0] = currentState;
                            currentState = m.Representation[1];
                            nextState.Representation[1] = currentState;
                            nextState.score = 2;
                        }
                        m.addChild(nextState);
                        if (nextState.homePawns == 4)
                            endGame = true;      
                    

                    }
                }
            }
            else
            {
                //min player
                //if chance state is 6 we can insert a pawn
                //for now only 
                if (m.chanceNode == 6 && m.Representation[1][0] == 0 && (pawnPositions2.Count + m.homePawns) != 4)
                {
                    //setting new representation
                    currentState = reinitialize(m.Representation[1]);
                    currentState[0] = 2;
                    Move nextMove = new Move();
                    nextMove.parent = m;
                    nextMove.Representation[1] = currentState;
                    currentState = m.Representation[0];
                    nextMove.Representation[0] = currentState;
                    nextMove.score = 5; //as the max score;
                    m.addChild(nextMove);
                }
                else
                {
                    //check number of pawns and their positions pawnPositions list
                    for (int i = 0; i < pawnPositions2.Count(); i++)
                    {
                        currentState = reinitialize(m.Representation[1]);

                        // get active pawn i
                        int position = pawnPositions2.ElementAt(i);

                        Move nextState = new Move();
                        nextState.parent = m;
                        //check if it can destroy another pawn
                        if (checkIfCanAttack(position, m.chanceNode, pawnPositions1))
                        {
                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 2;
                            nextState.Representation[1] = currentState;

                            currentState = m.Representation[0];
                            int pos1 = (position + m.chanceNode) % 48;
                            //updating 2nd player table;
                            if (currentState[pos1] == 1)
                            {
                                currentState[pos1] = 0;
                                nextState.Representation[0] = currentState;
                            }
                            pos1 = Math.Abs(position - m.chanceNode);
                            if (currentState[pos1] == 2)
                            {
                                currentState[pos1] = 0;
                                nextState.Representation[0] = currentState;
                            }
                            nextState.score = 5;
                            
                        }
                        //CHECK if this move sends a pawn home
                        else if (position + m.chanceNode > 47)
                        {
                            currentState[position] = 0;
                            nextState.Representation[1] = currentState;
                            nextState.homePawns += 1;
                            currentState = m.Representation[0];
                            nextState.Representation[0] = currentState;
                            nextState.score = 5;
                            
                        }
                        else
                        {
                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 2;
                            nextState.Representation[1] = currentState;
                            currentState = m.Representation[0];
                            nextState.Representation[0] = currentState;
                            nextState.score = 2;
                        }
                        m.addChild(nextState);
                        if (nextState.homePawns == 4)
                            endGame = true;


                    }
                }
            }
           
        }

        public static int[] reinitialize(int[] actualState)
        {
            int[] state = new int[48];
            for (int i = 0; i < 48; i++)
            {
                state[i] = actualState[i];
            }
            return state;
        }

        public static bool checkIfCanAttack(int position, int nodeValue, List<int> list2)
        {
            int value1 = (position + nodeValue) % 48;
            int value2 = Math.Abs((position - nodeValue)) % 48;
            if (list2.Contains(value1) || list2.Contains(value2))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }

}