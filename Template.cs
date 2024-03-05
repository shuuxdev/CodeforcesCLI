


using System.Globalization;
using System.Text;

namespace Problem
{
    //###CIN_LIBRARY###//

    public class TemplateGoesHere
    {
        public void Main()
        {
            int t = Cin.NextInt();
            for (int i = 1; i <= t; ++i)
            {
                CaseStart(i);
                Solve();
                CaseEnd(i);
            }
        }

        public static void CaseStart(int i)
        {
            Console.WriteLine("###CASE_START[{0}]###", i);
        }

        public static void CaseEnd(int i)
        {
            Console.WriteLine("###CASE_END[{0}]###", i);
        }
        
        static void Solve()
        {
            int n = Cin.NextInt();
        }
    }    
}
