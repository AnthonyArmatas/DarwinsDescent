using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarwinsDescent
{
    public interface StateMachineFlags
    {
        int HorizontalSpeedHash { get; }
        int VerticalSpeedHash { get; }
        int GroundedHash { get; }
        int RespawnParaHash { get; }
        int HurtHash { get; }
        int DeadHash { get; }

        int GetExtendedHash(string HashName);
    }
}
