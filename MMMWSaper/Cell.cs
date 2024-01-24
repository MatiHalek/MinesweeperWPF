using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMMWSaper
{
    public class Cell
    {
        public uint PositionX { get; set; }
        public uint PositionY { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsBomb { get; set; }

        public Cell(uint positionX, uint positionY, bool isRevealed, bool isFlagged, bool isBomb)
        {
            PositionX = positionX;
            PositionY = positionY;
            IsRevealed = isRevealed;
            IsFlagged = isFlagged;
            IsBomb = isBomb;
        }
        public Cell() { }
    }
}
