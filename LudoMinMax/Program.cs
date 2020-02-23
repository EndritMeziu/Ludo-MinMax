using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LudoMinMax
{
    class Program
    {

        static int[] p1Rep = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static int[] p2Rep = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static int homepawns1 = 0;
        static int homepawns2 = 0;
        static int[][] initialState = { p1Rep, p2Rep };
        static Move move;
        static bool turn;
        public static bool endGame = false; // :D
        static void Main(string[] args)
        {
            //1st player turn
            move = new Move();
            move.Representation = initialState;
            move.parent = null;
            turn = true;
            move.homePawns = 0;
            int count = 0;
            while(!endGame)
            { 
                Move m = generateChanceNodes(move, turn);                
                Random roll = new Random();
                Thread.Sleep(20);
                int value = roll.Next(1, 7); //random roll between 1 and 6
                Console.WriteLine("Turn: " + turn);
                if (value == 6)
                    turn = !turn;
                Console.WriteLine("Roll: " + value);
                foreach(Move child in m.getChilds())
                {
                    //check for rolled chance node
                    if(child.chanceNode == value)
                    {
                        //call to minmax
                        move = choseChildNode(child);
                        string[,] matrix = transformRepresentation(move.Representation);
                        if (move.score == 4 && turn == true)
                        {

                            homepawns1++;
                        }
                        if (move.score == 4 && turn == false)
                        {
                            homepawns2++;
                        }
                        printMatrix(matrix);
                        Console.ReadLine();
                        turn = !turn;
                        count++;
                        break;
                    }
                }
            }
            
        }

        static void setHomePawns(int[,] matrix)
        {
            
        }

        static void printMatrix(string[,] matrix)
        {
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        

        static void printRepresentation(int[][] representation)
        {
            for(int i=0;i<representation.Count();i++)
            {
                for(int j=0;j<representation[i].Length;j++)
                {
                    Console.Write(representation[i][j] + " ");
                }
                Console.WriteLine();
            }
        }

        static Move choseChildNode(Move m)
        {
            int value = 0;
            Move nextState = m;
            foreach(Move child in m.getChilds())
            {
                if(child.score >= value)
                {
                    value = child.score;
                    nextState = child;
                }
            }
            return nextState;
        }

        static Move generateChanceNodes(Move m, bool maxPlayer)
        {
            //6 possible chances -> 6 chance nodes
            int[] currentState;
            for (int i = 1; i <= 6; i++)
            {
                Move newMove = new Move();
                currentState = reinitialize(m.Representation[0]);
                newMove.Representation[0] = currentState;
                newMove.parent = m;
                newMove.chanceNode = i;
                
                currentState = reinitialize(m.Representation[1]);
                newMove.Representation[1] = currentState;
                m.addChild(newMove);
                checkNextStates(newMove, maxPlayer);
            }
            return m;
        }
        static void checkNextStates(Move m, bool maxPlayer)
        {
            List<int> pawnPositions1 = new List<int>();
            List<int> pawnPositions2 = new List<int>();
            int[] currentState;
            int[] oponentState;
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
                if (m.chanceNode == 6 && m.Representation[0][0] == 0 && (pawnPositions1.Count + homepawns1) != 4)
                {
                    //setting new representation
                    currentState = reinitialize(m.Representation[0]);
                    currentState[0] = 1;
                    Move nextMove = new Move();
                    nextMove.parent = m;
                    nextMove.Representation[0] = currentState;
                    oponentState = reinitialize(m.Representation[1]);
                    nextMove.Representation[1] = oponentState;
                    nextMove.score = 5; //as the max score;
                    m.addChild(nextMove);
                }
                else
                {
                    //check number of pawns and their positions pawnPositions list
                    for (int i = 0; i < pawnPositions1.Count(); i++)
                    {
                        currentState = reinitialize(m.Representation[0]);
                        oponentState = reinitialize(m.Representation[1]);
                        // get active pawn i
                        int position = pawnPositions1.ElementAt(i);

                        //check if it can destroy another pawn
                        if (checkIfCanAttack(position, m.chanceNode, pawnPositions2))
                        {
                            Move nextState = new Move();
                            nextState.parent = m;
                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 1;
                            nextState.Representation[0] = currentState;

                             
                            int pos1 = (position + m.chanceNode + 24) % 48;
                            //updating 2nd player table;
                            if (oponentState[pos1] == 2)
                            {
                                oponentState[pos1] = 0;
                                nextState.Representation[1] = oponentState;
                            }
                            oponentState = reinitialize(m.Representation[1]);
                            pos1 = (position + m.chanceNode - 24);
                            if(pos1 > 0) { 
                                if(oponentState[pos1] == 2)
                                {
                                    oponentState[pos1] = 0;
                                    nextState.Representation[1] = oponentState;
                                }
                            }
                            nextState.score = 5;
                            m.addChild(nextState);
                      
                        }
                        //CHECK if this move sends a pawn home
                        else if(position + m.chanceNode > 47)
                        {
                            Move nextState = new Move();
                            nextState.parent = m;
                            currentState[position] = 0;
                            nextState.Representation[0] = currentState;

                            oponentState = reinitialize(m.Representation[1]);
                            nextState.Representation[1] = oponentState;
                            nextState.score = 4;
                            m.addChild(nextState);
                            MessageBox.Show("Pawn home by " + maxPlayer + " " +homepawns1);
                            
                        }
                        else
                        {
                            Move nextState = new Move();
                            nextState.parent = m;

                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 1;
                            nextState.Representation[0] = currentState;
                            
                            oponentState = reinitialize(m.Representation[1]);
                            nextState.Representation[1] = oponentState;
                            nextState.score = 2;
                            m.addChild(nextState);
                        }
                       
                        if (homepawns1 == 4 || homepawns2 == 4)
                            endGame = true;      
                    

                    }
                }
            }
            else
            {
                //min player
                //if chance state is 6 we can insert a pawn
                //for now only 
                if (m.chanceNode == 6 && m.Representation[1][0] == 0 && (pawnPositions2.Count + homepawns2) != 4)
                {
                    //setting new representation
                    currentState = reinitialize(m.Representation[1]);
                    currentState[0] = 2;
                    Move nextMove = new Move();
                    nextMove.parent = m;
                    nextMove.Representation[1] = currentState;
                    oponentState = reinitialize(m.Representation[0]);
                    nextMove.Representation[0] = oponentState;
                    nextMove.score = 5; //as the max score;
                    m.addChild(nextMove);
                }
                else
                {
                    //check number of pawns and their positions pawnPositions list
                    for (int i = 0; i < pawnPositions2.Count(); i++)
                    {
                        currentState = reinitialize(m.Representation[1]);
                        oponentState = reinitialize(m.Representation[0]);
                        // get active pawn i
                        int position = pawnPositions2.ElementAt(i);

                        
                        //check if it can destroy another pawn
                        if (checkIfCanAttack(position, m.chanceNode, pawnPositions1))
                        {
                            Move nextState = new Move();
                            nextState.parent = m;
                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 2;
                            nextState.Representation[1] = currentState;

                            oponentState = reinitialize(m.Representation[0]);
                            int pos1 = (position + m.chanceNode + 24) % 48;
                            //updating 2nd player table;
                            if (oponentState[pos1] == 1)
                            {
                                oponentState[pos1] = 0;
                                nextState.Representation[0] = oponentState;
                            }
                            pos1 = (position - 24 + m.chanceNode);
                            if(pos1 > 0) { 
                                if (oponentState[pos1] == 1)
                                {
                                    oponentState[pos1] = 0;
                                    nextState.Representation[0] = oponentState;
                                }
                            }
                            nextState.score = 5;
                            m.addChild(nextState);

                        }
                        //CHECK if this move sends a pawn home
                        else if(position + m.chanceNode > 47)
                        {
                            Move nextState = new Move();
                            nextState.parent = m;
                            currentState[position] = 0;
                            nextState.Representation[1] = currentState;
                            
                            oponentState = reinitialize(m.Representation[0]);
                            nextState.Representation[0] = oponentState;
                            nextState.score = 4;
                            m.addChild(nextState);
                            MessageBox.Show("Pawn home by " + maxPlayer+ " "+homepawns2);

                        }
                        else
                        {
                            Move nextState = new Move();
                            nextState.parent = m;
                            currentState[position] = 0;
                            currentState[position + m.chanceNode] = 2;
                            nextState.Representation[1] = currentState;
                            
                            oponentState = reinitialize(m.Representation[0]);
                            nextState.Representation[0] = oponentState;
                            nextState.score = 2;
                            m.addChild(nextState);
                        }
                        
                        if (homepawns1 == 4 || homepawns2 == 4) { 
                            endGame = true;
                            Console.WriteLine("Game Ended");
                        }


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
            int value1 = (position + nodeValue + 24) % 48;
            int value2 = (position + nodeValue - 24) % 48;
            if (list2.Contains(value1) || (list2.Contains(value2) && value2 > 0))
            {
                    return true;
            }
            else
            {
                return false;
            }
            
        }


        static string[,] transformRepresentation(int[][] representation)
        {
            string[,] transformed = new string[13, 13];
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    transformed[i, j] = " ";
                }
            }

            int[] firstPlayer = representation[0];
            int[] secondPlayer = representation[1];


            Dictionary<int, Tuple<int, int>> map = new Dictionary<int, Tuple<int, int>>();
            Dictionary<int, Tuple<int, int>> map2 = new Dictionary<int, Tuple<int, int>>();

            for (int i = 12; i > 6; i--)
                map.Add(12 - i, new Tuple<int, int>(i, 5));// [0 - 5]

            int offset = 2;
            for (int i = 4; i >= 0; i--)
            {
                map.Add(i + offset, new Tuple<int, int>(7, i));//[6-10]
                offset += 2;
            }
            map.Add(11, new Tuple<int, int>(6, 0)); //[11]

            for (int i = 0; i < 6; i++)
                map.Add(12 + i, new Tuple<int, int>(5, i)); //[12 -17]

            offset = 0;
            for (int i = 4; i >= 0; i--)
            {
                map.Add(18 + offset, new Tuple<int, int>(i, 5)); //[18-22]
                offset += 1;
            }

            map.Add(23, new Tuple<int, int>(0, 6)); //[23]

            for (int i = 0; i < 6; i++)
                map.Add(24 + i, new Tuple<int, int>(i, 7)); //[24-29]

            for (int i = 8; i <= 12; i++)
                map.Add(22 + i, new Tuple<int, int>(5, i)); //[30-34]

            map.Add(35, new Tuple<int, int>(6, 12)); // [35]

            offset = 0;
            for (int i = 12; i >= 7; i--)
            {
                map.Add(36 + offset, new Tuple<int, int>(7, i)); // [36-41]
                offset += 1;
            }

            for (int i = 8; i <= 12; i++)
                map.Add(34 + i, new Tuple<int, int>(i, 7)); // [42-46]

            map.Add(47, new Tuple<int, int>(12, 6));

            for (int i = 0; i < map.Count; i++)
            {
                Tuple<int, int> position = map[i];
                int x = position.Item1;
                int y = position.Item2;
                transformed[x, y] = "O";
            }
            for (int i = 1; i < 12; i++)
            {
                if (i < 5)
                    transformed[i, 6] = "H";
                if (i > 7)
                    transformed[i, 6] = "H";
            }

            for (int i = 0; i < 6; i++)
                map2.Add(i, new Tuple<int, int>(i, 7)); //[0 - 5]

            for (int i = 8; i <= 12; i++)
                map2.Add(i - 2, new Tuple<int, int>(5, i)); //[6-10]

            map2.Add(11, new Tuple<int, int>(6, 12)); //[11]

            offset = 12;
            for (int i = 12; i > 6; i--)
            {
                map2.Add(offset, new Tuple<int, int>(7, i)); //[12 - 17]
                offset += 1;
            }

            offset = 18;
            for (int i = 8; i <= 12; i++)
            {
                map2.Add(offset, new Tuple<int, int>(i, 7)); //[18 - 22]
                offset += 1;
            }

            map2.Add(23, new Tuple<int, int>(12, 6)); //[23]

            offset = 24;
            for (int i = 12; i > 6; i--)
            {
                map2.Add(offset, new Tuple<int, int>(i, 5)); //[24 - 29]
                offset += 1;
            }

            offset = 30;
            for (int i = 4; i >= 0; i--)
            {
                map2.Add(offset, new Tuple<int, int>(7, i)); //[30 - 34]
                offset += 1;
            }
            map2.Add(35, new Tuple<int, int>(6, 0)); //[35]

            for (int i = 0; i < 6; i++)
                map2.Add(36 + i, new Tuple<int, int>(5, i)); //[36 - 41]

            offset = 42;
            for (int i = 4; i >= 0; i--)
            {
                map2.Add(offset, new Tuple<int, int>(i, 5)); //[42 - 46]
                offset += 1;
            }
            map2.Add(47, new Tuple<int, int>(0, 6)); //[47]


            List<int> indexes1 = new List<int>();
            List<int> indexes2 = new List<int>();
            for (int i = 0; i < firstPlayer.Length; i++)
            {
                if (firstPlayer[i] == 1)
                    indexes1.Add(i);
                if (secondPlayer[i] == 2)
                    indexes2.Add(i);
            }

            for (int i = 0; i < indexes1.Count; i++)
            {
                int value = indexes1[i];
                Tuple<int, int> position = map[value];
                int x = position.Item1;
                int y = position.Item2;
                transformed[x, y] = "1";
            }
            for (int i = 0; i < indexes2.Count; i++)
            {
                int value = indexes2[i];
                Tuple<int, int> position = map2[value];
                int x = position.Item1;
                int y = position.Item2;
                transformed[x, y] = "2";
            }

            return transformed;
        }
    }

}