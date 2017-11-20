using System;
using TestInterfaces;

namespace TestAssemblyA
{
    public class MockPluginA: ITestPluginA
    {
        public void DoStuff()
        {
            throw new NotImplementedException();
        }
    }

    public class MockPluginB : ITestPluginA
    {
        public void DoStuff()
        {
            throw new NotImplementedException();
        }
    }


}
