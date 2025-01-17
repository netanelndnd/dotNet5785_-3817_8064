using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    // Collection class for CallType enum
    internal class CallTypeCollection : IEnumerable
    {
        // Static readonly field containing all values of CallType enum
        static readonly IEnumerable<BO.CallType> s_enums =
           (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

        // Implementation of GetEnumerator to allow iteration over CallType values
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class CallstatusCollection : IEnumerable
    {
        // Static readonly field containing all values of CallType enum
        static readonly IEnumerable<BO.ClosedCallInListFields> s_enums =
           (Enum.GetValues(typeof(BO.ClosedCallInListFields)) as IEnumerable<BO.ClosedCallInListFields>)!;

        // Implementation of GetEnumerator to allow iteration over CallType values
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    // Collection class for DistanceType enum
    internal class DistanceTypeCollection : IEnumerable
    {
        // Static readonly field containing all values of DistanceType enum
        static readonly IEnumerable<BO.DistanceType> s_enums =
           (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

        // Implementation of GetEnumerator to allow iteration over DistanceType values
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    // Collection class for VolunteerRoles enum
    internal class VolunteerRolesCollection : IEnumerable
    {
        // Static readonly field containing all values of VolunteerRoles enum
        static readonly IEnumerable<BO.VolunteerRole> s_enums =
           (Enum.GetValues(typeof(BO.VolunteerRole)) as IEnumerable<BO.VolunteerRole>)!;

        // Implementation of GetEnumerator to allow iteration over VolunteerRoles values
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }



}
