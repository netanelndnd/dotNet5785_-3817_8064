using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

// Collection class for DistanceType enum
internal class DistanceTypeCollection : IEnumerable
{
    // Static readonly field containing all values of DistanceType enum
    static readonly IEnumerable<BO.DistanceType> s_enums =
       (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

    // Implementation of GetEnumerator to allow iteration over DistanceType values
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
