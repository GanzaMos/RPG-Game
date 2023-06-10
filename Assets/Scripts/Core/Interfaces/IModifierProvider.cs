using System.Collections.Generic;

namespace RPG.Core
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifier(Stat stat);
        IEnumerable<float> GetPercentageModifier(Stat stat);
    }
}