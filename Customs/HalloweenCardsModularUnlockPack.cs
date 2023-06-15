using KitchenData;
using KitchenLib.Customs;
using System.Collections.Generic;
using UnityEngine;

namespace ThoseWereTheDays.Customs
{
    public class UnlockSelectorMultiGroupChoice : IUnlockSelector
    {
        public List<UnlockGroup> Group1;
        public List<UnlockGroup> Group2;

        public UnlockOptions GetOptions(List<Unlock> candidates, HashSet<int> current_cards, UnlockRequest request)
        {
            UnlockOptions result = default;
            foreach (Unlock candidate in candidates)
            {
                if (Group1.Contains(candidate.UnlockGroup) || Group2.Contains(candidate.UnlockGroup))
                {
                    if (result.Unlock1 == null || (Group1.Contains(candidate.UnlockGroup) && !Group1.Contains(result.Unlock1.UnlockGroup)))
                    {
                        result.Unlock1 = candidate;
                    }
                    if (result.Unlock2 == null || (Group2.Contains(candidate.UnlockGroup) && !Group2.Contains(result.Unlock2.UnlockGroup)))
                    {
                        result.Unlock2 = candidate;
                    }
                }
            }
            return result;
        }
    }

    public class FilterSpecialCardsByType : IUnlockFilter
    {
        public bool AllowIfOnList;
        public List<CardType> Types = new List<CardType>();

        public bool ShouldBlockCard(Unlock candidate, HashSet<int> current_cards, UnlockRequest request)
        {
            if (candidate.UnlockGroup != UnlockGroup.Special)
                return false;
            return AllowIfOnList != Types.Contains(candidate.CardType);
        }
    }

    public class HalloweenCardsModularUnlockPack : CustomModularUnlockPack
    {
        public override string UniqueNameID => "halloweenCardsModularUnlockPack";

        public override List<IUnlockSet> Sets => new List<IUnlockSet>()
        {
            new UnlockSetAutomatic()
        };

        public override List<IUnlockFilter> Filter => new List<IUnlockFilter>()
        {
            new FilterBasic()
            {
                IgnoreUnlockability = false,
                IgnoreFranchiseTier = false,
                IgnoreDuplicateFilter = false,
                IgnoreRequirements = false,
                AllowBaseDishes = false
            },
            new FilterByType()
            {
                AllowIfOnList = true,
                Types = new List<CardType>()
                {
                    CardType.Default,
                    CardType.HalloweenTrick
                }
            },
            new FilterSpecialCardsByType
            {
                AllowIfOnList = true,
                Types = new List<CardType>()
                {
                    CardType.HalloweenTrick
                }
            }
        };

        public override List<IUnlockSorter> Sorters => new List<IUnlockSorter>()
        {
            new UnlockSorterShuffle(),
            new UnlockSorterPriority()
            {
                PriorityProbability = 0.5f,
                PrioritiseRequirements = false,
                Groups = new List<UnlockGroup>()
                {
                    UnlockGroup.Special
                },
                DishTypes = new List<DishType>()
                {
                    DishType.Main,
                    DishType.Extra
                }
            }
        };

        public override List<ConditionalOptions> ConditionalOptions => new List<ConditionalOptions>()
        {
            new ConditionalOptions()
            {
                Selector = new UnlockSelectorMultiGroupChoice()
                {
                    Group1 = new List<UnlockGroup>()
                    {
                        UnlockGroup.Special, UnlockGroup.Generic
                    },
                    Group2 = new List<UnlockGroup>()
                    {
                        UnlockGroup.Dish
                    }
                },
                Condition = new UnlockConditionRegular()
                {
                    DayInterval = 3,
                    DayOffset = 0,
                    DayMin = 1,
                    DayMax = -1,
                    TierRequired = -1
                }
            }
        };
    }
}
