using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleLogic
{
    public class ClassWithConditionalLogic
    {
        public int MultiplyInSillyWay(int a, int b)
        {
            var result = 0;
            for (int i = 0; i < a; i++)
                result += b;

            return result;
        }
    }
}