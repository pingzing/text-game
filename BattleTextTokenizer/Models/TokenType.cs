using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTextTokenizer.Models
{
    public enum TokenType
    {
        Character,
        String,
        Pause,
        ChangeTiming,
        Stop,
        ClearScreen
    }
}
