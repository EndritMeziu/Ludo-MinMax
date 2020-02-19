using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoMinMax
{
    class Move
    {
        public int[][] Representation;
        public Move parent;
        public int score;
        public int chanceNode;
        public int homePawns;
        public List<Move> childs;
        
        public Move()
        {
            Representation = new int[2][];
            childs = new List<Move>();
        }

        public void addChild(Move m)
        {
            this.childs.Add(m);
        }

        public Move getParent()
        {
            return this.parent;
        }

        public List<Move> getChilds()
        {
            return this.childs;
        }
    }
}
