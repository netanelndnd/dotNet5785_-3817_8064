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





}
