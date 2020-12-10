using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Network
{
    public static class CallbackCode
    {
        public const int IsMasterPartySynced = 100;
        public const int IsSyncSlavePartyTaskDone = 101;
        public const int IsSlaveCombatManagerInited = 102;
        public const int IsSlaveOnBattleStartedCommandEnded = 103;

        public const int IsSlavePartySynced = 200;
    }
}
