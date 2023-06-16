using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace ThoseWereTheDays
{
    [UpdateInGroup(typeof(CustomerCreationGroup), OrderLast = true)]
    public class AssignHalloweenOrders : RestaurantSystem, IModSystem
    {
        Entity singleton;
        CSetting setting;
        TwitchNameList _twitchNameList;

        EntityQuery Customers;
        protected override void Initialise()
        {
            base.Initialise();
            Customers = GetEntityQuery(new QueryHelper()
                .All(typeof(CCustomer))
                .None(typeof(CHalloweenOrder)));
            _twitchNameList = base.World?.GetExistingSystem<TwitchNameList>();
        }

        protected override void OnUpdate()
        {
            if (!TryGetSingletonEntity<SKitchenLayout>(out singleton) ||
                !Require(singleton, out setting) || 
                setting.RestaurantSetting != 82131534 ||    // To be replaced with KitchenLib reference once RestaurantSettingReferences is added.
                _twitchNameList == null)
            {
                Main.LogInfo($"AssignHalloweenOrders FAIL");
                Main.LogInfo($"singleton = {(singleton == default ? "default" : singleton.Index)}");
                Main.LogInfo($"setting = {setting.RestaurantSetting}");
                Main.LogInfo($"_twitchNameList = {_twitchNameList?.ToString() ?? "null"}");
                return;
            }
            using NativeArray<Entity> entities = Customers.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                if (_twitchNameList.IsTrick(entity))
                {
                    Set(entity, CHalloweenOrder.Trick);
                    continue;
                }
                if (_twitchNameList.IsTreat(entity))
                {
                    Set(entity, CHalloweenOrder.Treat);
                    continue;
                }
            }
        }
    }
}
