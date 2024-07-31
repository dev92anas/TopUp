using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopUp.Application.Enums
{
    public enum LookupsGroups
    {
        TopUpOptions = 1,
        TopUpLimitConfigs = 2,
    }

    public enum TopUpLimitConfigs
    {
        VerifiedUserMonthlyTopUp = 11,
        UnVerifiedUserMonthlyTopUp = 12,
        MaxTotalTopUpLimit = 13
    }
}
