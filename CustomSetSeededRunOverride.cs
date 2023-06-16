﻿using Kitchen;
using KitchenMods;
using System;
using System.Linq;
using Unity.Entities;

namespace ThoseWereTheDays
{
    [UpdateBefore(typeof(SetSeededRunOverride))]
    public class CustomSetSeededRunOverride : FranchiseSystem, IModSystem
    {
        private EntityQuery SeedFixers;

        private EntityQuery SettingSelectors;

        private EntityQuery SeededLayoutPedestal;

        private EntityQuery LayoutPedestal;

        protected override void Initialise()
        {
            base.Initialise();
            SeedFixers = GetEntityQuery(typeof(CSeededRunInfo));
            SettingSelectors = GetEntityQuery(typeof(CSettingSelector));
            SeededLayoutPedestal = GetEntityQuery(typeof(SSeededLayoutPedestal));
            LayoutPedestal = GetEntityQuery(typeof(SLayoutPedestal));
        }

        protected override void OnUpdate()
        {
            if (SeedFixers.IsEmpty || !Has<SLayoutPedestal>() || !Has<SSeededLayoutPedestal>())
            {
                return;
            }
            int setting_id = CSettingSelector.IDFromQuery(SettingSelectors);
            if (!SettingLayoutProfiles.TryGetValidLayoutIDs(setting_id, out int[] valid_layouts))
            {
                return;
            }
            EntityContext entityContext = new EntityContext(base.EntityManager);
            CSeededRunInfo cSeededRunInfo = SeedFixers.First<CSeededRunInfo>();
            Entity singletonEntity = SeededLayoutPedestal.GetSingletonEntity();
            Entity singletonEntity2 = LayoutPedestal.GetSingletonEntity();
            bool isSeedOverride = cSeededRunInfo.IsSeedOverride;
            entityContext.Ensure<CHideView>(singletonEntity2, isSeedOverride);
            entityContext.Ensure<CPreventItemTransfer>(singletonEntity2, isSeedOverride);
            entityContext.Ensure<CHideView>(singletonEntity, !isSeedOverride);
            entityContext.Ensure<SSelectedLayoutPedestal>(singletonEntity, isSeedOverride);
            entityContext.Ensure<SSelectedLayoutPedestal>(singletonEntity2, !isSeedOverride);
            if (!isSeedOverride || !Require(singletonEntity, out CItemHolder comp))
            {
                return;
            }
            Seed fixedSeed = cSeededRunInfo.FixedSeed;
            if (Require<CSetting>((Entity)comp, out CSetting comp2))
            {
                if (comp2.FixedSeed == fixedSeed && comp2.RestaurantSetting == setting_id)
                {
                    return;
                }
                base.EntityManager.DestroyEntity(comp.HeldItem);
            }
            SettingLayoutProfiles.ReplaceAssetReferences(valid_layouts);
            Entity entity = new LayoutSeed(fixedSeed).GenerateMap(base.EntityManager, setting_id);
            SettingLayoutProfiles.RestoreAssetReferences();
            base.EntityManager.SetComponentData(singletonEntity, (CItemHolder)entity);
            base.EntityManager.SetComponentData(entity, (CHeldBy)singletonEntity);
        }
    }
}